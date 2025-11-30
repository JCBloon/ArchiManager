package io.github.jnetbloon.amproject.model.entities;

import org.springframework.data.domain.Slice;

import java.util.List;

public interface CustomizedProjectDao {
    public List<Project> findprojectsByKeywords(String title, String expedientNumber, Integer year, String cadastralReference);
    public Slice<Project> searchProjectByKeywords(String title, String expedientNumber, Integer year, String cadastralReference, int page, int size, String columnOrder, String wayToOrder);
}
