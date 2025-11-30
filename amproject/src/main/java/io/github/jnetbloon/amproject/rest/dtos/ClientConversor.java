package io.github.jnetbloon.amproject.rest.dtos;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.entities.Project;

import java.util.ArrayList;
import java.util.List;

public class ClientConversor {
    // Debe ser privada para evitar que se creen más instancias
    private ClientConversor() {}

    public static ClientDto toClientDto(Client client) {
        return new ClientDto(client.getId(), client.getDni(), client.getName(), client.getSurname1(), client.getSurname2(), client.getPhone(), client.getAddress(),
                ProjectConversor.toSimplifiedProjectDtos(client.getProjects()));
    }

    public static SimplifiedClientDto toSimplifiedClientDto(Client client) {
        return new SimplifiedClientDto(client.getId(), client.getDni(), client.getName(),  client.getSurname1(), client.getSurname2(), client.getPhone(), client.getAddress());
    }

    public static List<SimplifiedClientDto> toSimplifiedClientDtos(List<Client> clients) {
        if (clients == null) return new ArrayList<>();
        List<SimplifiedClientDto> simplifiedClientDtos = new ArrayList<>();
        for(Client client : clients)
            simplifiedClientDtos.add(toSimplifiedClientDto(client));
        return simplifiedClientDtos;
    }

}
