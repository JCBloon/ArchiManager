package io.github.jnetbloon.amproject.model.services;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.exceptions.DataAlreadyAdded;
import io.github.jnetbloon.amproject.model.exceptions.FormatError;
import io.github.jnetbloon.amproject.model.exceptions.InstanceNotFoundException;
import io.github.jnetbloon.amproject.model.exceptions.MoreThanOneFoundException;

public interface ClientService {
    public Client createClient(Client client) throws DataAlreadyAdded, FormatError;
    public Client findClient(String name, String surname1, String surname2, String dni) throws MoreThanOneFoundException, InstanceNotFoundException;
    public Block<Client> searchClient(String name, String surname1, String surname2, String dni, int page, int size, String columnOrder, String wayToOrder);
    public Client updateClient(Client client) throws InstanceNotFoundException, DataAlreadyAdded, FormatError;
    public void deleteClient(Long id) throws InstanceNotFoundException;
}
