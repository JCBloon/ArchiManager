package io.github.jnetbloon.amproject.model.entities;

import jakarta.persistence.*;

import java.util.List;
import java.util.Objects;

@Entity
public class Client {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    private String dni;
    private String name;
    private String surname1;
    private String surname2;
    private String phone;
    private String address;
    @ManyToMany (fetch = FetchType.LAZY)
    @JoinTable (
            name = "associated",
            joinColumns = @JoinColumn(name = "client_id"),
            inverseJoinColumns = @JoinColumn(name = "project_id")
    )
    private List<Project> projects;

    public Client() {}

    public Client(String dni, String name, String surname1, String surname2, String phone, String address) {
        this.dni = dni;
        this.name = name;
        this.surname1 = surname1;
        this.surname2 = surname2;
        this.phone = phone;
        this.address = address;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getDni() { return dni; }

    public void setDni(String dni) { this.dni = dni; }

    public String getName() { return name; }

    public void setName(String name) { this.name = name; }

    public String getSurname1() { return surname1; }

    public void setSurname1(String surname1) { this.surname1 = surname1; }

    public String getSurname2() { return surname2; }

    public void setSurname2(String surname2) { this.surname2 = surname2; }

    public String getPhone() { return phone; }

    public void setPhone(String phone) { this.phone = phone; }

    public String getAddress() { return address; }

    public void setAddress(String address) { this.address = address; }

    public List<Project> getProjects() {
        return projects;
    }

    public void setProjects(List<Project> projects) {
        this.projects = projects;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Client client = (Client) o;
        return Objects.equals(id, client.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }

}
