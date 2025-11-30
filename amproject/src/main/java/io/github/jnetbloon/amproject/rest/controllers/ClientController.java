package io.github.jnetbloon.amproject.rest.controllers;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.exceptions.DataAlreadyAdded;
import io.github.jnetbloon.amproject.model.exceptions.FormatError;
import io.github.jnetbloon.amproject.model.exceptions.InstanceNotFoundException;
import io.github.jnetbloon.amproject.model.exceptions.MoreThanOneFoundException;
import io.github.jnetbloon.amproject.model.services.Block;
import io.github.jnetbloon.amproject.model.services.ClientService;
import io.github.jnetbloon.amproject.rest.common.BlockDto;
import io.github.jnetbloon.amproject.rest.dtos.ClientParamsDto;
import io.github.jnetbloon.amproject.rest.dtos.ClientConversor;
import io.github.jnetbloon.amproject.rest.dtos.ClientDto;
import io.github.jnetbloon.amproject.rest.dtos.SimplifiedClientDto;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/clients")
public class ClientController {

    private int STATIC_SIZE_PAGE = 10;

    @Autowired
    private ClientService clientService;

    @PostMapping
    public ClientDto createClient(@Validated @RequestBody ClientParamsDto clientParamsDto)
                                    throws DataAlreadyAdded, FormatError {
        Client client = new Client(clientParamsDto.getDni(), clientParamsDto.getName(), clientParamsDto.getSurname1(),
                clientParamsDto.getSurname2(), clientParamsDto.getPhone(), clientParamsDto.getAddress());
        return ClientConversor.toClientDto(clientService.createClient(client));
    }

    @GetMapping
    @RequestMapping("/find")
    public ClientDto findClient(@RequestParam(required = false) String name, @RequestParam(required = false) String surname1,
                                @RequestParam(required = false) String surname2, @RequestParam(required = false) String dni)
                                    throws InstanceNotFoundException, MoreThanOneFoundException {
        return ClientConversor.toClientDto(clientService.findClient(name, surname1, surname2, dni));
    }

    @GetMapping
    @RequestMapping("/search")
    public BlockDto<SimplifiedClientDto> searchClient(@RequestParam(required = false) String name, @RequestParam(required = false) String surname1,
                                                                @RequestParam(required = false) String surname2, @RequestParam(required = false) String dni,
                                                                @RequestParam(defaultValue ="0") int page, @RequestParam(required = false) String columnOrder,
                                                                @RequestParam(required = false) String wayToOrder) {
        Block<Client> clients = clientService.searchClient(name, surname1, surname2, dni, page, STATIC_SIZE_PAGE, columnOrder, wayToOrder);
        return new BlockDto<>(ClientConversor.toSimplifiedClientDtos(clients.getItems()), clients.isHasNext());
    }

    /*
        // NOTA: RequestMapping es para evitar el uso compartido de ruta, si se quiere compartir,
        como en este caso con /id, update y delete, especificar ruta en el Mapping del método de la petición
     */
    @PutMapping("/{id}")
    public ClientDto updateClient(@PathVariable Long id, @Validated @RequestBody ClientParamsDto clientParamsDto)
                                    throws InstanceNotFoundException, DataAlreadyAdded, FormatError {
        Client client = new Client(clientParamsDto.getDni(), clientParamsDto.getName(), clientParamsDto.getSurname1(),
                clientParamsDto.getSurname2(), clientParamsDto.getPhone(), clientParamsDto.getAddress());
        client.setId(id);
        Client clientUpdated = clientService.updateClient(client);
        return ClientConversor.toClientDto(clientUpdated);
    }

    @DeleteMapping("/{id}")
    public void deleteClient(@PathVariable Long id) throws InstanceNotFoundException {
        clientService.deleteClient(id);
    }

}
