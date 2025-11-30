package io.github.jnetbloon.amproject.rest.dtos;

import java.util.List;

public class ClientDto {
    private Long id;
    private String dni;
    private String name;
    private String surname1;
    private String surname2;
    private String phone;
    private String address;
    private List<SimplifiedProjectDto> projectList;

    public ClientDto(Long id, String dni, String name, String surname1, String surname2, String phone, String address, List<SimplifiedProjectDto> projects) {
        this.id = id;
        this.dni = dni;
        this.name = name;
        this.surname1 = surname1;
        this.surname2 = surname2;
        this.phone = phone;
        this.address = address;
        this.projectList = projects;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
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

    public List<SimplifiedProjectDto> getProjectList() {
        return projectList;
    }

    public void setProjectList(List<SimplifiedProjectDto> projectList) {
        this.projectList = projectList;
    }
}
