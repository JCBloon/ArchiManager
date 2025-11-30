package io.github.jnetbloon.amproject.model.services;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.entities.ClientDao;
import io.github.jnetbloon.amproject.model.entities.Project;
import io.github.jnetbloon.amproject.model.entities.ProjectDao;
import io.github.jnetbloon.amproject.model.exceptions.*;
import org.springframework.context.MessageSource;
import org.springframework.context.i18n.LocaleContextHolder;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Slice;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import javax.imageio.ImageIO;
import javax.swing.text.html.Option;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Service
@Transactional
public class ProjectServiceImpl implements ProjectService {

    @Autowired
    private MessageSource messageSource;

    @Autowired
    private ProjectDao projectDao;

    @Autowired
    private ClientDao clientDao;

    // Project image
    private String getExtension(String fileName) {
        if (fileName.lastIndexOf(".") == -1)
            return null;

        return fileName.substring(fileName.lastIndexOf("."));
    }

    public ImageBlock getImage(Long id) throws InstanceNotFoundException, ImageError {
        Optional<Project> optionalProject = projectDao.findById(id);
        if (optionalProject.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        Project project = optionalProject.get();
        String imageName = project.getId().toString();

        String userHome = System.getProperty("user.home");
        Path imagePath = Paths.get(userHome, "ArchiManager", "images", imageName + ".png");
        if (!Files.exists(imagePath)) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.noimage",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }
        String contentType;
        try {
            contentType = Files.probeContentType(imagePath);
        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.imagecorrupted",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }


        byte[] imageData;
        try {
            imageData = Files.readAllBytes(imagePath);
        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.read",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }

        return new ImageBlock(contentType, imageData);
    }

    public void updateImage(Long id, MultipartFile imageFile) throws InstanceNotFoundException, ImageError {
        Optional<Project> optionalProject = projectDao.findById(id);
        if (optionalProject.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        Project project = optionalProject.get();

        if(imageFile != null) {
            setImageToProject(imageFile, project);
        } else {
            deleteImage(project);
        }
    }

    private void setImageToProject(MultipartFile imageFile, Project project) throws ImageError {
        // Almacenar imagen
        // Acceder-crear carpeta
        Path uploadDir;
        try {
            String userHome = System.getProperty("user.home");
            uploadDir = Paths.get(userHome, "ArchiManager", "images");
            if(!Files.exists(uploadDir)) {
                Files.createDirectories(uploadDir);
            }
        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.dir",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }

        // Validar nombre de archivo
        String fileOriginalName = imageFile.getOriginalFilename();
        if(fileOriginalName == null) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.filename",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }

        // Comprobar fichero
        String extension = getExtension(fileOriginalName);
        if (extension == null) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.rawfile",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }
        extension = extension.toLowerCase();
        switch (extension) {
            case ".png":
            case ".jpg":
            case ".jpeg":
                break;
            default:
                String errorMessage = messageSource.getMessage("service.project.imageerror.invalidtype",
                        null, LocaleContextHolder.getLocale());
                throw new ImageError(errorMessage);
        }

        // Leer imagen original
        BufferedImage bufferedImage;
        try (InputStream input = imageFile.getInputStream()) {
            bufferedImage = ImageIO.read(input);
            if (bufferedImage == null) {
                String errorMessage = messageSource.getMessage("service.project.imageerror.imagecorrupted",
                        null, LocaleContextHolder.getLocale());
                throw new ImageError(errorMessage);
            }
        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.read",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }


        // Convertir a png y guardar
        Path destinationPath = Paths.get(uploadDir.toString(), project.getId().toString() + ".png");
        try {
            // Borrar imágen anterior
            deleteImage(project);
            ImageIO.write(bufferedImage, "png", destinationPath.toFile());

        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.imagecopy",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }
    }

    private void deleteImage(Project project) {
        String userHome = System.getProperty("user.home");
        Path imagePath = Paths.get(userHome, "ArchiManager", "images", project.getId().toString() + ".png");
        if (!Files.exists(imagePath)) {
            //La imágen ya fue borrada o no tiene
            return;
        }
        try {
            Files.delete(imagePath);
        } catch (IOException e) {
            String errorMessage = messageSource.getMessage("service.project.imageerror.delete",
                    null, LocaleContextHolder.getLocale());
            throw new ImageError(errorMessage);
        }
    }

    // Project Data

    public Project saveProject(Project project) {
        Optional<Project> projectOptional = projectDao.findByExpedientNumber(project.getExpedientNumber());

        if (projectOptional.isPresent()) {
            // Si estas creando un proyecto (Id = null) o modificando uno pero no es el
            // mismo objeto (project != projectOptional), ese proyecto ya fue añadido
            if (project.getId() == null || !project.getId().equals(projectOptional.get().getId())) {
                String errorMessage = messageSource.getMessage("service.project.dataalreadyadded",
                        new Object[]{project.getExpedientNumber()}, LocaleContextHolder.getLocale());
                throw new DataAlreadyAdded(errorMessage);
            }
        }

        try {
            return projectDao.save(project);
        } catch (DataIntegrityViolationException e) {
            String errorMessage = messageSource.getMessage("service.formaterror",
                    new Object[]{e.getMessage()}, LocaleContextHolder.getLocale());
            throw new FormatError(errorMessage);
        }
    }

    public Project createProject(Project project, Long clientId) throws InstanceNotFoundException, DataAlreadyAdded, FormatError, ImageError {
        if (clientId == null) {
            String errorMessage = messageSource.getMessage("service.formaterror",
                    new Object[]{"You must assign a client"}, LocaleContextHolder.getLocale());
            throw new FormatError(errorMessage);
        }
        Optional<Client> client = clientDao.findById(clientId);
        if (client.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        // Añadir proyecto
        Project projectCreated = saveProject(project);

        // Meter cliente a los proyectos
        List<Client> projectClients = new ArrayList<>();
        projectClients.add(client.get());
        projectCreated.setClients(projectClients);
        // Meter proyecto al cliente
        if (client.get().getProjects() == null) {
            client.get().setProjects(new ArrayList<>());
        }
        client.get().getProjects().add(projectCreated);

        return projectCreated;
    }

    public Project assignClientToProject(Long projectId, Long clientId) throws InstanceNotFoundException, ClientAlreadyAssigned {
        Optional<Client> client = clientDao.findById(clientId);
        if (client.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        Optional<Project> project = projectDao.findById(projectId);
        if (project.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        Project projectFound = project.get();
        List<Client> projectClients = projectFound.getClients();
        if(projectClients.contains(client.get())) {
            String errorMessage = messageSource.getMessage("service.project.clientalreadyassigned",
                    null, LocaleContextHolder.getLocale());
            throw new ClientAlreadyAssigned(errorMessage);
        }
        if(client.get().getProjects() == null) {
            client.get().setProjects(new ArrayList<>());
        }
        client.get().getProjects().add(projectFound);

        projectClients.add(client.get());
        projectFound.setClients(projectClients);
        Project updatedProject = projectDao.save(projectFound);

        return updatedProject;
    }

    public Optional<Project> unassignClientFromProject(Long projectId, Long clientId) throws InstanceNotFoundException, ClientNotAssigned {
        Optional<Client> client = clientDao.findById(clientId);
        if (client.isEmpty())  {
            String errorMessage = messageSource.getMessage("service.client.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        Optional<Project> project = projectDao.findById(projectId);
        if (project.isEmpty())  {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        Project projectFound = project.get();
        List<Client> projectClients = projectFound.getClients();
        if(!projectClients.contains(client.get())) {
            String errorMessage = messageSource.getMessage("service.project.clientnotassigned",
                    null, LocaleContextHolder.getLocale());
            throw new ClientNotAssigned(errorMessage);
        }

        client.get().getProjects().remove(projectFound);

        projectClients.remove(client.get());
        if (projectClients.isEmpty()) {
            projectDao.delete(projectFound);
            return Optional.empty();
        }
        projectFound.setClients(projectClients);
        Project updatedProject = projectDao.save(projectFound);

        return Optional.of(updatedProject);
    }

    public Project updateProject(Project project) throws InstanceNotFoundException, DataAlreadyAdded, FormatError, ImageError {
        // Verificar que el proyecto exista
        Optional<Project> projectOptional = projectDao.findById(project.getId());
        if(projectOptional.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        Project updateProject = projectOptional.get();
        updateProject.setTitle(project.getTitle());
        updateProject.setExpedientNumber(project.getExpedientNumber());
        updateProject.setYear(project.getYear());
        updateProject.setCadastralReference(project.getCadastralReference());
        updateProject.setArchiveNumber(project.getArchiveNumber());
        updateProject.setComment(project.getComment());

        return saveProject(updateProject);
    }

    @Transactional(readOnly = true)
    public Project findProject(String title, String expedientNumber, Integer year, String cadastralReference) throws MoreThanOneFoundException, InstanceNotFoundException {
        List<Project> projects = projectDao.findprojectsByKeywords(title, expedientNumber, year, cadastralReference);
        if(projects.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }
        if (projects.size() > 1) {
            String errorMessage = messageSource.getMessage("service.morethanonefoundexception",
                    null, LocaleContextHolder.getLocale());
            throw new MoreThanOneFoundException(errorMessage);
        }

        return projects.get(0);
    }

    @Transactional(readOnly = true)
    public Block<Project> searchProject(String title, String expedientNumber, Integer year, String cadastralReference, int page, int size, String columnOrder, String wayToOrder) {
        Slice<Project> projectSlice = projectDao.searchProjectByKeywords(title, expedientNumber, year, cadastralReference, page, size, columnOrder, wayToOrder);
        return new Block<Project>(projectSlice.getContent(), projectSlice.hasNext());
    }

    public void deleteProject(Long id) throws InstanceNotFoundException, ImageError {
        Optional<Project> optionalProject = projectDao.findById(id);
        if(optionalProject.isEmpty()) {
            String errorMessage = messageSource.getMessage("service.project.instancenotfound",
                    null, LocaleContextHolder.getLocale());
            throw new InstanceNotFoundException(errorMessage);
        }

        Project project = optionalProject.get();

        List<Client> clients = project.getClients();
        for (Client client : clients) {
            client.getProjects().remove(project);
        }

        deleteImage(project);

        projectDao.deleteById(id);
    }

}
