package io.github.jnetbloon.amproject.model.entities;

import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface ProjectDao extends JpaRepository<Project, Long>, CustomizedProjectDao {
    public List<Project> findByClients_dni(String dni);
    public Optional<Project> findByExpedientNumber(String expedientNumber);
}
