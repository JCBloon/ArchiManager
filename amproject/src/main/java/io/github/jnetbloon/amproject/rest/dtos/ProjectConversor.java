package io.github.jnetbloon.amproject.rest.dtos;

import io.github.jnetbloon.amproject.model.entities.Project;

import java.util.ArrayList;
import java.util.List;

public class ProjectConversor {
    private ProjectConversor() {}

    public static ProjectDto toProjectDto(Project project) {
        return new ProjectDto(project.getId(), project.getTitle(), project.getExpedientNumber(), project.getYear(),
                project.getCadastralReference(), project.getArchiveNumber(), project.getComment(),
                ClientConversor.toSimplifiedClientDtos(project.getClients()));
    }

    public static SimplifiedProjectDto toSimplifiedProjectDto(Project project) {
        return new SimplifiedProjectDto(project.getId(), project.getTitle(), project.getExpedientNumber(), project.getYear(),
                project.getCadastralReference(), project.getArchiveNumber(), project.getComment());
    }

    public static List<SimplifiedProjectDto> toSimplifiedProjectDtos(List<Project> projects) {
        if(projects == null) return new ArrayList<>();
        List<SimplifiedProjectDto> projectDtos = new ArrayList<>();
        for (Project project : projects)
            projectDtos.add(toSimplifiedProjectDto(project));
        return projectDtos;
    }
}
