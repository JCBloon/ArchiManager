package io.github.jnetbloon.amproject.rest.dtos;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

public class ClientParamsDto {
    /*
    // NOTA: Necesitas mandar lo mismo para ADD y UPDATE, por lo que son los
    mismos parámetros, en caso contrario usar interfaces. Ejemplo:

    public interface AddValidations {}
    public interface UpdateValidations {}

    //En las validaciones:
    @NotNull(groups = AddValidations.class)
    @Max(100, groups = {AddValidations.class, UpdateValidations.class})

    // Para usar uno u otro en el Controlador, emplear
    @Validated({ClientParamsDto.AddValidations.class}) @RequestBody ClientDto clientDto
    @Validated({ClientParamsDto.UpdateValidations.class}) @RequestBody ClientDto clientDto
     */
    @NotNull(message = "{validator.client.dni}")
    @Size(min = 9, max = 9, message = "{validator.client.dni}")
    @Pattern(
            regexp = "^[0-9]{8}[A-Za-z]$|^[A-Za-z][0-9]{8}$",
            message = "{validator.client.dni.format}"
    )
    private String dni;

    @NotNull(message = "{validator.client.name}")
    @Size(max = 40, message = "{validator.client.name}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ.,; \\-]+$",
            message = "{validator.client.name.letterformat}"
    )
    private String name;

    @Size(max = 40, message = "{validator.client.surname1}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$",
            message = "{validator.client.surname1.letterformat}"
    )
    private String surname1;

    @Size(max = 40, message = "{validator.client.surname2}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$",
            message = "{validator.client.surname2.letterformat}"
    )
    private String surname2;

    @Size(max = 15, message = "{validator.client.phone}")
    @Pattern(
            regexp = "^[+]?[0-9]+$",
            message = "{validator.client.phone.format}"
    )
    private String phone;

    @Size(max = 100, message = "{validator.client.address}")
    private String address;

    public ClientParamsDto(String dni, String name, String surname1, String surname2, String phone, String address) {
        this.dni = dni;
        this.name = name;
        this.surname1 = surname1;
        this.surname2 = surname2;
        this.phone = phone;
        this.address = address;
    }

    public String getDni() {
        return dni;
    }

    public void setDni(String dni) {
        this.dni = dni;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getSurname1() {
        return surname1;
    }

    public void setSurname1(String surname1) {
        this.surname1 = surname1;
    }

    public String getSurname2() {
        return surname2;
    }

    public void setSurname2(String surname2) {
        this.surname2 = surname2;
    }

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }
}
