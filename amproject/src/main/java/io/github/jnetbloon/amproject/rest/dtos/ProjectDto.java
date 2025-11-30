package io.github.jnetbloon.amproject.rest.dtos;

import io.github.jnetbloon.amproject.model.entities.Client;
import jakarta.persistence.Column;
import jakarta.persistence.ManyToMany;

import java.util.List;

public class ProjectDto {
    private Long id;
    private String title;
    private String expedientNumber;
    private Integer year;
    private String cadastralReference;
    private Integer archiveNumber;
    private String comment;
    private List<SimplifiedClientDto> clientList;

    public ProjectDto(Long id, String title, String expedientNumber, Integer year, String cadastralReference, Integer archiveNumber, String comment, List<SimplifiedClientDto> clientList) {
        this.id = id;
        this.title = title;
        this.expedientNumber = expedientNumber;
        this.year = year;
        this.cadastralReference = cadastralReference;
        this.archiveNumber = archiveNumber;
        this.comment = comment;
        this.clientList = clientList;
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

    public List<SimplifiedClientDto> getClientList() {
        return clientList;
    }

    public void setClientList(List<SimplifiedClientDto> clientList) {
        this.clientList = clientList;
    }
}
