package io.github.jnetbloon.amproject.model.entities;

import jakarta.persistence.*;

import java.util.List;

@Entity
public class Project {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    private String title;
    @Column(name="expedient_number")
    private String expedientNumber;
    private Integer year;
    @Column(name="cadastral_reference")
    private String cadastralReference;
    @Column(name="archive_number")
    private Integer archiveNumber;
    private String comment;
    @ManyToMany(mappedBy = "projects")
    private List<Client> clients;

    public Project() {}

    public Project(String title, String expedientNumber, Integer year, String cadastralReference, Integer archiveNumber, String comment) {
        this.title = title;
        this.expedientNumber = expedientNumber;
        this.year = year;
        this.cadastralReference = cadastralReference;
        this.archiveNumber = archiveNumber;
        this.comment = comment;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getExpedientNumber() {
        return expedientNumber;
    }

    public void setExpedientNumber(String expedientNumber) {
        this.expedientNumber = expedientNumber;
    }

    public Integer getYear() {
        return year;
    }

    public void setYear(Integer year) {
        this.year = year;
    }

    public String getCadastralReference() {
        return cadastralReference;
    }

    public void setCadastralReference(String cadastralReference) {
        this.cadastralReference = cadastralReference;
    }

    public Integer getArchiveNumber() {
        return archiveNumber;
    }

    public void setArchiveNumber(Integer archiveNumber) {
        this.archiveNumber = archiveNumber;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }

    public List<Client> getClients() {
        return clients;
    }

    public void setClients(List<Client> clients) {
        this.clients = clients;
    }
}
