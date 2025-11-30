package io.github.jnetbloon.amproject.rest.controllers;

import io.github.jnetbloon.amproject.model.entities.Project;
import io.github.jnetbloon.amproject.model.exceptions.*;
import io.github.jnetbloon.amproject.model.services.Block;
import io.github.jnetbloon.amproject.model.services.ImageBlock;
import io.github.jnetbloon.amproject.model.services.ProjectService;
import io.github.jnetbloon.amproject.rest.common.BlockDto;
import io.github.jnetbloon.amproject.rest.common.ErrorDto;
import io.github.jnetbloon.amproject.rest.dtos.ProjectConversor;
import io.github.jnetbloon.amproject.rest.dtos.ProjectDto;
import io.github.jnetbloon.amproject.rest.dtos.ProjectParamsDto;
import io.github.jnetbloon.amproject.rest.dtos.SimplifiedProjectDto;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.time.LocalDateTime;
import java.util.Optional;

@RestController
@RequestMapping("/projects")
public class ProjectController {

    private final int STATIC_SIZE_PAGE = 10;

    @Autowired
    private ProjectService projectService;

    @ExceptionHandler(ClientAlreadyAssigned.class)
    public ResponseEntity<?> handleClientAlreadyAssigned(ClientAlreadyAssigned ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.CONFLICT.value(),
                HttpStatus.CONFLICT.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.CONFLICT);
    }

    @ExceptionHandler(ClientNotAssigned.class)
    public ResponseEntity<?> handleClientNotAssigned(ClientNotAssigned ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.CONFLICT.value(),
                HttpStatus.CONFLICT.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.CONFLICT);
    }

    @ExceptionHandler(ImageError.class)
    public ResponseEntity<?> handleImageError(ImageError ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.BAD_REQUEST.value(),
                HttpStatus.BAD_REQUEST.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.BAD_REQUEST);
    }

    // Peticiones

    @GetMapping("/image/{id}")
    public ResponseEntity<byte[]> getProjectImage(@PathVariable Long id) throws InstanceNotFoundException, ImageError {
        ImageBlock imageBlock = projectService.getImage(id);
        return ResponseEntity.ok().contentType(MediaType.parseMediaType(imageBlock.getImageContentType())).body(imageBlock.getImageData());
    }

    @PutMapping("/image/{id}")
    public void setProjectImage(@PathVariable Long id, @RequestPart(required = false) MultipartFile imageFile) throws InstanceNotFoundException, ImageError {
        projectService.updateImage(id, imageFile);
    }

    @PostMapping
    public ProjectDto createProject(@Validated @RequestBody ProjectParamsDto projectParamsDto)
                                    throws InstanceNotFoundException, DataAlreadyAdded, ImageError {
        Project project = new Project(projectParamsDto.getTitle(), projectParamsDto.getExpedientNumber(),
                projectParamsDto.getYear(), projectParamsDto.getCadastralReference(),
                projectParamsDto.getArchiveNumber(), projectParamsDto.getComment());
        return ProjectConversor.toProjectDto(projectService.createProject(project, projectParamsDto.getClientId()));
    }

    @PutMapping
    @RequestMapping("/assign")
    public ProjectDto assignProject(@RequestParam Long projectId, @RequestParam Long clientId)
                                    throws InstanceNotFoundException, ClientAlreadyAssigned {
        return ProjectConversor.toProjectDto(projectService.assignClientToProject(projectId, clientId));
    }

    @PutMapping
    @RequestMapping("/unassign")
    public ResponseEntity<ProjectDto> unassignProject(@RequestParam Long projectId, @RequestParam Long clientId)
                                    throws InstanceNotFoundException, ClientNotAssigned {
        Optional<Project> projectOptional = projectService.unassignClientFromProject(projectId, clientId);
        if (projectOptional.isEmpty())
            return new ResponseEntity<>(HttpStatus.OK);
        return ResponseEntity.ok(ProjectConversor.toProjectDto(projectOptional.get()));
    }

    @GetMapping
    @RequestMapping("/find")
    public ProjectDto findProject(@RequestParam(required = false) String title, @RequestParam(required = false) String expedientNumber,
                                  @RequestParam(required = false) Integer year, @RequestParam(required = false) String cadastralReference)
                                    throws InstanceNotFoundException, MoreThanOneFoundException {
        return ProjectConversor.toProjectDto(projectService.findProject(title, expedientNumber, year, cadastralReference));
    }

    @GetMapping
    @RequestMapping("/search")
    public BlockDto<SimplifiedProjectDto> searchProject(@RequestParam(required = false) String title, @RequestParam(required = false) String expedientNumber,
                                                        @RequestParam(required = false) Integer year, @RequestParam(required = false) String cadastralReference,
                                                        @RequestParam(defaultValue = "0") int page, @RequestParam(required = false) String columnOrder,
                                                        @RequestParam(required = false) String wayToOrder) {
        Block<Project> projects = projectService.searchProject(title, expedientNumber, year, cadastralReference, page, STATIC_SIZE_PAGE, columnOrder, wayToOrder);
        return new BlockDto<>(ProjectConversor.toSimplifiedProjectDtos(projects.getItems()), projects.isHasNext());
    }

    @PutMapping("/{id}")
    public ResponseEntity<ProjectDto> updateProject(@PathVariable Long id, @Validated @RequestBody ProjectParamsDto projectParamsDto)
                                                throws InstanceNotFoundException, DataAlreadyAdded {
        Project project = new Project(projectParamsDto.getTitle(), projectParamsDto.getExpedientNumber(), projectParamsDto.getYear(), projectParamsDto.getCadastralReference(), projectParamsDto.getArchiveNumber(), projectParamsDto.getComment());
        project.setId(id);
        return ResponseEntity.ok(ProjectConversor.toProjectDto(projectService.updateProject(project)));
    }

    @DeleteMapping("/{id}")
    public void deleteProject(@PathVariable Long id) throws InstanceNotFoundException, ImageError {
        projectService.deleteProject(id);
    }
}
