package io.github.jnetbloon.amproject.model.services;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.entities.ClientDao;
import io.github.jnetbloon.amproject.model.entities.Project;
import io.github.jnetbloon.amproject.model.entities.ProjectDao;
import io.github.jnetbloon.amproject.model.exceptions.DataAlreadyAdded;
import io.github.jnetbloon.amproject.model.exceptions.FormatError;
import io.github.jnetbloon.amproject.model.exceptions.InstanceNotFoundException;
import io.github.jnetbloon.amproject.model.exceptions.MoreThanOneFoundException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.MessageSource;
import org.springframework.context.i18n.LocaleContextHolder;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.data.domain.Slice;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.sql.SQLException;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class ClientServiceImpl implements  ClientService {

    @Autowired
    private MessageSource messageSource;

    @Autowired
    private ClientDao clientDao;

    @Autowired
    private ProjectDao projectDao;

    public Client saveClient(Client client) {
        Optional<Client> clientOptional = clientDao.findByDni(client.getDni());
        if(clientOptional.isPresent()) {
            // Si estas creando un cliente (Id = null) o modificando uno pero no es el
            // mismo objeto (client != clientOptional), ese cliente ya fue añadido
            if (client.getId() == null || !client.getId().equals(clientOptional.get().getId())) {
                String errorMessage = messageSource.getMessage("service.client.dataalreadyadded",
                        new Object[]{client.getDni()}, LocaleContextHolder.getLocale());
                throw new DataAlreadyAdded(errorMessage);
            }
        }

        try {
            return clientDao.save(client);
        } catch (DataIntegrityViolationException e) {
            String errorMessage = messageSource.getMessage("service.formaterror",
                    new Object[]{e.getMessage()}, LocaleContextHolder.getLocale());
            throw new FormatError(errorMessage);
        }
    }

    public static String dnicifFormatter(String input) {
        // Verifica formato y retorna la letra en mayúscula
        if (input == null || input.length() != 9) return null;
        String inputFormatted = input.toUpperCase();

        if (Character.isDigit(input.charAt(0))) {
            // DNI
            String numbers = inputFormatted.substring(0, 8);
            char letter = inputFormatted.charAt(8);

            if (!numbers.matches("\\d{8}")) return null;
            if (!Character.isLetter(letter)) return null;

            return numbers + letter;
        } else if (Character.isAlphabetic(inputFormatted.charAt(0))) {
            // CIF
            String numbers = inputFormatted.substring(1, 9);
            char letter = inputFormatted.charAt(0);

            if (!numbers.matches("\\d{8}")) return null;
            if (!Character.isLetter(letter)) return null;

            return letter + numbers;
        } else {
            return null;
        }
    }

    public static boolean isValidDNICIF(String input) {
        // ASUME UN FORMATO VÁLIDO. Comprueba la letra del dni
        if (input == null || input.length() != 9) return false;
        if (Character.isDigit(input.charAt(0))) {
            // DNI
            char DNIletter = input.charAt(input.length() - 1);
            String letters = "TRWAGMYFPDXBNJZSQVHLCKE";
            String numbers = input.substring(0, input.length() - 1);

            char calculatedLetter = letters.charAt(Integer.parseInt(numbers) % 23);
            return DNIletter == calculatedLetter;
        } else return true; //CIF
    }

    public Client createClient(Client client) throws DataAlreadyAdded, FormatError {
        String formatedDNICIF = dnicifFormatter(client.getDni());
        if (formatedDNICIF == null) {
            String errorMessage = messageSource.getMessage("service.client.invaliddnicif",
                    null, LocaleContextHolder.getLocale());
            throw new FormatError(errorMessage);
        }
        if(!isValidDNICIF(formatedDNICIF)) {
            String errorMessage = messageSource.getMessage("service.client.invalidletterdni",
                    null, LocaleContextHolder.getLocale());
            throw new FormatError(errorMessage);
        }
        client.setDni(formatedDNICIF);

        return saveClient(client);
    }


    @Transactional(readOnly = true)
    public Client findClient(String name, String surname1, String surname2, String dni) throws MoreThanOneFoundException, InstanceNotFoundException {
        List<Client> clients = clientDao.findClientsByKeywords(name, surname1, surname2, dni);
        if(clients.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        if (clients.size() > 1) {
            String errorMessage = messageSource.getMessage("service.morethanonefoundexception",
                    null, LocaleContextHolder.getLocale());
            throw new MoreThanOneFoundException(errorMessage);
        }


        return clients.get(0);
    }

    @Transactional(readOnly = true)
    public Block<Client> searchClient(String name, String surname1, String surname2, String dni, int page, int size, String columnOrder, String wayToOrder) {
        Slice<Client> clientSlice = clientDao.searchClientsByKeywords(name, surname1, surname2, dni, page, size, columnOrder, wayToOrder);
        return new Block<Client>(clientSlice.getContent(), clientSlice.hasNext());
    }

    public Client updateClient(Client client) throws InstanceNotFoundException, DataAlreadyAdded {
        //Verificar que el cliente exista
        Optional<Client> clientOptional = clientDao.findById(client.getId());
        if(clientOptional.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        Client updateClient = clientOptional.get();
        updateClient.setDni(client.getDni());
        updateClient.setName(client.getName());
        updateClient.setSurname1(client.getSurname1());
        updateClient.setSurname2(client.getSurname2());
        updateClient.setAddress(client.getAddress());
        updateClient.setPhone(client.getPhone());


        return saveClient(updateClient);
    }

    public void deleteClient(Long id) throws InstanceNotFoundException {
        Optional<Client> client = clientDao.findById(id);

        if(client.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        List<Project> projects = projectDao.findByClients_dni(client.get().getDni());
        for(Project project : projects)
            if(project.getClients().size() == 1)
                projectDao.delete(project);
            else {
                project.getClients().remove(client.get());
                projectDao.save(project);
            }

        clientDao.deleteById(id);
    }


}
