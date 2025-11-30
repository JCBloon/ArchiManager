package io.github.jnetbloon.amproject.rest.dtos;

import jakarta.validation.constraints.*;

public class ProjectParamsDto {
    @Size(max = 100, message = "{validator.project.title}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9 ,.;_\\-]+$",
            message = "{validator.project.title.format}"
    )
    private String title;

    @NotNull(message = "{validator.project.expedientnumber}")
    @Size(min = 1, max = 5, message = "{validator.project.expedientnumber}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9 _\\-]+$",
            message = "{validator.project.expedientnumber.format}"
    )
    private String expedientNumber;

    @NotNull(message = "{validator.project.cadastralreference}")
    @Size(min = 14, max = 20, message = "{validator.project.cadastralreference}")
    @Pattern(
            regexp = "^[A-Za-z0-9 ]+$",
            message = "{validator.project.cadastralreference.format}"
    )
    private String cadastralReference;

    @Min(value = 1900, message = "{validator.project.year.min}")
    @Max(value = 2100, message = "{validator.project.year.max}")
    private Integer year;

    @PositiveOrZero(message = "{validator.project.archivenumber.positive}")
    @Max(value = 999, message = "{validator.project.archivenumber.max}")
    private Integer archiveNumber;

    @Size(max = 1000, message = "{validator.project.comment}")
    @Pattern(
            regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9 ,.;_\\-]+$",
            message = "{validator.project.comment.format}"
    )
    private String comment;

    private Long clientId;

    public ProjectParamsDto(String title, String expedientNumber, String cadastralReference, Integer year, Integer archiveNumber, String comment, Long clientId) {
        this.title = title;
        this.expedientNumber = expedientNumber;
        this.cadastralReference = cadastralReference;
        this.year = year;
        this.archiveNumber = archiveNumber;
        this.comment = comment;
        this.clientId = clientId;
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

    public String getCadastralReference() {
        return cadastralReference;
    }

    public void setCadastralReference(String cadastralReference) {
        this.cadastralReference = cadastralReference;
    }

    public Integer getYear() {
        return year;
    }

    public void setYear(Integer year) {
        this.year = year;
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

    public Long getClientId() {
        return clientId;
    }

    public void setClientId(Long clientId) {
        this.clientId = clientId;
    }
}
