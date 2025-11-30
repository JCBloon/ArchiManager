package io.github.jnetbloon.amproject.model.entities;

import org.springframework.data.domain.Slice;

import java.util.List;

public interface CustomizedClientDao {
    public List<Client> findClientsByKeywords(String name, String surname1, String surname2, String dni);
    public Slice<Client> searchClientsByKeywords(String name, String surname1, String surname2, String dni, int page, int size, String columnOrder, String wayToOrder);
}
