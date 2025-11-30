package io.github.jnetbloon.amproject.model.entities;

import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface ClientDao extends JpaRepository<Client, Long>, CustomizedClientDao {
    Optional<Client> findByDni(String dni);
}
