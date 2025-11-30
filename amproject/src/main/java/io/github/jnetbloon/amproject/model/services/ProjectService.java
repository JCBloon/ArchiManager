package io.github.jnetbloon.amproject.model.services;

import io.github.jnetbloon.amproject.model.entities.Project;
import io.github.jnetbloon.amproject.model.exceptions.*;
import org.springframework.web.multipart.MultipartFile;

import java.awt.*;
import java.util.Optional;

public interface ProjectService {
    public ImageBlock getImage(Long id) throws InstanceNotFoundException, ImageError;
    public void updateImage(Long id, MultipartFile imageFile) throws InstanceNotFoundException, ImageError;
    public Project createProject(Project project, Long clientId) throws InstanceNotFoundException, DataAlreadyAdded, FormatError, ImageError;
    public Project assignClientToProject(Long projectId, Long clientId) throws InstanceNotFoundException, ClientAlreadyAssigned;
    public Optional<Project> unassignClientFromProject(Long projectId, Long clientId) throws InstanceNotFoundException, ClientNotAssigned;
    public Project findProject(String title, String expedientNumber, Integer year, String cadastralReference) throws MoreThanOneFoundException, InstanceNotFoundException;
    public Block<Project> searchProject(String title, String expedientNumber, Integer year, String cadastralReference, int page, int size, String columnOrder, String wayToOrder);
    public Project updateProject(Project project) throws InstanceNotFoundException, DataAlreadyAdded, FormatError, ImageError;
    public void deleteProject(Long id) throws InstanceNotFoundException, ImageError;
}
