package io.github.jnetbloon.amproject;

import io.github.jnetbloon.amproject.model.entities.Client;
import io.github.jnetbloon.amproject.model.entities.Project;
import io.github.jnetbloon.amproject.model.exceptions.*;
import io.github.jnetbloon.amproject.model.services.ClientService;
import io.github.jnetbloon.amproject.model.services.ProjectService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

@SpringBootTest
@Transactional
@ActiveProfiles("test")
public class ProjectServiceTest {

    @Autowired
    private ClientService clientService;

    @Autowired
    private ProjectService projectService;

    private Client clientTestJoaquin;
    private Client clientTestEnrique;

    public Project createTestProject(String title, String expedientNumber, Integer year, String cadastralReference, Integer archiveNumber, String comment) {
        return new Project(title, expedientNumber, year, cadastralReference, archiveNumber, comment);
    }

    @BeforeEach
    public void setUp() {
        clientTestJoaquin = new Client("87654321X", "Joaquín Manuel", "García", "Méndez", "837289374", "Madrid, Avenida Desengaño 21");
        clientTestEnrique = new Client("12345678Z", "Enrique", "Pastor", "González", "923728372", "Madrid, Mirador de Montepinar - BAJO B");
        clientService.createClient(clientTestJoaquin);
        clientService.createClient(clientTestEnrique);
    }

    @Test
    void createProjectTest() {
        Project projectOne = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());

        Project projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(projectOne.getExpedientNumber(), projectFound.getExpedientNumber());
        assertEquals(clientTestJoaquin.getDni(), projectFound.getClients().get(0).getDni());

        assertThrows(DataAlreadyAdded.class, () -> projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId()));
    }

    @Test
    void assignClientTest() {
        Project projectOne = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());
        Project projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(1, projectFound.getClients().size());

        assertThrows(ClientAlreadyAssigned.class, () -> projectService.assignClientToProject(projectOne.getId(), clientTestJoaquin.getId()));

        projectService.assignClientToProject(projectOne.getId(), clientTestEnrique.getId());

        projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(2, projectFound.getClients().size());
    }

    @Test
    void unassignClientTest() {
        Project projectOne = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());
        projectService.assignClientToProject(projectOne.getId(), clientTestEnrique.getId());
        Project projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(2, projectFound.getClients().size());

        projectService.unassignClientFromProject(projectOne.getId(), clientTestEnrique.getId());
        projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(1, projectFound.getClients().size());

        assertThrows(ClientNotAssigned.class, () ->  projectService.unassignClientFromProject(projectOne.getId(), clientTestEnrique.getId()));

        // Un proyecto sin clientes asignados debe estar borrado
        projectService.unassignClientFromProject(projectOne.getId(), clientTestJoaquin.getId());
        assertThrows(InstanceNotFoundException.class, () -> projectService.findProject(null, "10S", null, null));
    }

    @Test
    void findProjectTest() {
        assertThrows(InstanceNotFoundException.class, () -> projectService.findProject(null, "10S", null, null));

        Project projectOne = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());

        Project projectFound = projectService.findProject("dificio", null, null, null);
        assertEquals(projectOne.getExpedientNumber(), projectFound.getExpedientNumber());

        projectFound = projectService.findProject(null, "0S", null, null);
        assertEquals(projectOne.getExpedientNumber(), projectFound.getExpedientNumber());

        projectFound = projectService.findProject(null, null, 2025, null);
        assertEquals(projectOne.getExpedientNumber(), projectFound.getExpedientNumber());

        projectFound = projectService.findProject(null, null, null, "34-5");
        assertEquals(projectOne.getExpedientNumber(), projectFound.getExpedientNumber());

        Project projectTwo = projectService.createProject(createTestProject("Chalet ejemplo" , "34C", 2025, "5678-2345", 316, "Comentario del chalet"), clientTestJoaquin.getId());

        projectFound = projectService.findProject(null, "34", null, null);
        assertEquals(projectTwo.getExpedientNumber(), projectFound.getExpedientNumber());

        assertNotEquals(projectOne, projectTwo);
        assertThrows(MoreThanOneFoundException.class, () -> projectService.findProject(null, null, 2025, null));
        assertThrows(InstanceNotFoundException.class, () -> projectService.findProject(null, "Inexistente", null, null));
        assertThrows(InstanceNotFoundException.class, () -> projectService.findProject(null, null, null, null));
    }

    @Test
    void searchProjectTest() {
        List<Project> projects = projectService.searchProject("edificio", null,2025,  null, 0, 2, null, null).getItems();
        assertEquals(0, projects.size());

        projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());
        projectService.createProject(createTestProject("Chalet ejemplo" , "34C", 2025, "5678-2345", 316, "Comentario del chalet"), clientTestJoaquin.getId());

        projects = projectService.searchProject("edificio", null,  null, null, 0, 2, null, null).getItems();
        assertEquals(1, projects.size());

        projects = projectService.searchProject("EJEMPLO", null,  null, null, 0, 2, null, null).getItems();
        assertEquals(2, projects.size());
        assertNotEquals(projects.get(0).getExpedientNumber(), projects.get(1).getExpedientNumber());

        // Probar size
        projects = projectService.searchProject("EJEMPLO", null,  null, null, 0, 1, null, null).getItems();
        assertEquals(1, projects.size());

        // Probar resto de parámetros
        projects = projectService.searchProject(null, "10", null, null,  0, 2, null, null).getItems();
        assertEquals(1, projects.size());

        projects = projectService.searchProject(null, null,  2025, null,  0, 2, null, null).getItems();
        assertEquals(2, projects.size());

        projects = projectService.searchProject(null, null,  null, "234",  0, 2, null, null).getItems();
        assertEquals(2, projects.size());

        // Búsqueda sin filtros (devuelve todos los proyectos)
        projects = projectService.searchProject(null, null, null,  null,0, 4, null, null).getItems();
        assertEquals(2, projects.size());
    }

    @Test
    void updateProjectTest() {
        Project project = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());

        String title = "Chalet ejemplo";
        String expedientNumber = "34C";
        Integer year = 2024;
        String cadastralReference = "5678-2345";
        Integer archiveNumber = 123;
        String comment = "Chalet de playa con piscina privada";

        project.setTitle(title);
        project.setExpedientNumber(expedientNumber);
        project.setYear(year);
        project.setCadastralReference(cadastralReference);
        project.setArchiveNumber(archiveNumber);
        project.setComment(comment);

        projectService.updateProject(project);
        Project projectFound = projectService.findProject("chalet", null, null, null);

        assertEquals(title, projectFound.getTitle());
        assertEquals(expedientNumber, projectFound.getExpedientNumber());
        assertEquals(year, projectFound.getYear());
        assertEquals(cadastralReference, projectFound.getCadastralReference());
        assertEquals(archiveNumber, projectFound.getArchiveNumber());
        assertEquals(comment, projectFound.getComment());
    }


    @Test
    void deleteProjectTest() {
        Project projectOne = projectService.createProject(createTestProject("Edificio ejemplo", "10S", 2025, "1234-5678", 212, "Comentario"), clientTestJoaquin.getId());
        Project projectFound = projectService.findProject(null, "10S", null, null);
        assertEquals(projectOne.getId(), projectFound.getId());

        projectService.deleteProject(projectFound.getId());

        assertThrows(InstanceNotFoundException.class, () -> projectService.findProject(null, "10S", null, null));
    }

}
