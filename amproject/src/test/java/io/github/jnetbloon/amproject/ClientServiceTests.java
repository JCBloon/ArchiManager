package io.github.jnetbloon.amproject;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.exceptions.DataAlreadyAdded;
import io.github.jnetbloon.amproject.model.exceptions.InstanceNotFoundException;
import io.github.jnetbloon.amproject.model.exceptions.MoreThanOneFoundException;
import io.github.jnetbloon.amproject.model.services.ClientService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

@SpringBootTest
@Transactional
@ActiveProfiles("test")
class ClientServiceTests {

    @Autowired
    private ClientService clientService;

    private Client createTestClient(String dni, String name, String surname1, String surname2, String phone, String address) {
        return new Client(dni, name, surname1, surname2, phone, address);
    }

    @Test
    void createClientTest() {
        Client clientJoaquin = clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", null, null, null));

        Client clientFound = clientService.findClient("Joaquín", null, null, null);
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        assertThrows(DataAlreadyAdded.class, () -> clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", null, null, null)));
    }

    @Test
    void findClientTest() {
        assertThrows(InstanceNotFoundException.class, () -> clientService.findClient("Joaquín", null, null, null));

        Client clientJoaquin = clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", "Estévez", null, null));

        Client clientFound = clientService.findClient("Joaquin", null, null, null);
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        clientFound = clientService.findClient(null, "Cañ", null, null);
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        clientFound = clientService.findClient(null, null, "ste", null);
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        clientFound = clientService.findClient(null, null, null, "456");
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        Client clientJoaquinManuel = clientService.createClient(createTestClient("87654321X", "Joaquín Manuel", "García", null, null, null));

        clientFound = clientService.findClient("Joaquín Ma", null, null, null);
        assertEquals(clientJoaquinManuel.getDni(),  clientFound.getDni());

        assertNotEquals(clientJoaquinManuel, clientJoaquin);
        assertThrows(MoreThanOneFoundException.class, () -> clientService.findClient("Joaquín", null, null, null));
        assertThrows(InstanceNotFoundException.class, () -> clientService.findClient(null, null, null, null));
        assertThrows(InstanceNotFoundException.class, () -> clientService.findClient("Paco", null, null, null));
    }

    @Test
    void searchClientTest() {
        List<Client> clients = clientService.searchClient("Joaquín", null, null, null, 0, 1, null, null).getItems();
        assertEquals(0, clients.size());

        Client clientJoaquin = clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", "Estévez", null, null));
        Client clientJoaquinManuel = clientService.createClient(createTestClient("87654321X", "Joaquín Manuel", "García", null, null, null));

        clients = clientService.searchClient("Joaquín", null, null, null, 0, 1, null, null).getItems();
        assertEquals(1, clients.size());
        assertEquals(clients.get(0).getDni(), clientJoaquin.getDni());

        clients = clientService.searchClient("Joaquín", null, null, null, 0, 2, null, null).getItems();
        assertEquals(2, clients.size());
        assertFalse((clients.get(0).getDni()).equals(clients.get(1).getDni()));

        clients = clientService.searchClient("Joaquín", null, null, null, 0, 3, null, null).getItems();
        assertEquals(2, clients.size());

        clients = clientService.searchClient(null, null, null, null, 0, 2, null, null).getItems();
        assertTrue(clients.size() == 2);
    }

    @Test
    void updateClientTest() {
        Client clientJoaquin = clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", "Estévez", null, null));
        clientJoaquin.setDni("87654321X");
        clientJoaquin.setName("Joaquín Manuel");
        clientJoaquin.setSurname1("García");
        clientJoaquin.setSurname2("Méndez");
        clientJoaquin.setPhone("837289374");
        clientJoaquin.setAddress("Madrid, Avenida Desengaño 21");

        Client updatedClient = clientService.updateClient(clientJoaquin);
        assertEquals(clientJoaquin.getDni(), updatedClient.getDni());
        assertEquals(clientJoaquin.getName(), updatedClient.getName());
        assertEquals(clientJoaquin.getSurname1(), updatedClient.getSurname1());
        assertEquals(clientJoaquin.getSurname2(), updatedClient.getSurname2());
        assertEquals(clientJoaquin.getPhone(), updatedClient.getPhone());
        assertEquals(clientJoaquin.getAddress(), updatedClient.getAddress());
    }

    @Test
    void deleteClientTest() {
        Client clientJoaquin = clientService.createClient(createTestClient("12345678Z", "Joaquín", "Cañer", "Estévez", null, null));

        Client clientFound = clientService.findClient("Joaquín", null, null, null);
        assertEquals(clientJoaquin.getDni(),  clientFound.getDni());

        clientService.deleteClient(clientFound.getId());

        assertThrows(InstanceNotFoundException.class, () -> clientService.findClient("Joaquín", null, null, null));
    }
}
