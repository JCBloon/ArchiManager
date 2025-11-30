package io.github.jnetbloon.amproject.model.entities;

import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.persistence.Query;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Slice;
import org.springframework.data.domain.SliceImpl;

import java.util.ArrayList;
import java.util.List;

public class CustomizedProjectDaoImpl implements  CustomizedProjectDao {

    @PersistenceContext
    private EntityManager entityManager;

    public List<Project> findprojectsByKeywords(String title, String expedientNumber, Integer year, String cadastralReference) {
        if(title == null && expedientNumber == null && year == null && cadastralReference == null)
            return new ArrayList<>();

        StringBuilder queryString = new StringBuilder("SELECT p FROM Project p WHERE 1=1");

        if (title != null) queryString.append(" AND LOWER(p.title) LIKE LOWER(:title)");
        if (expedientNumber != null) queryString.append(" AND LOWER(p.expedientNumber) LIKE LOWER(:expedientNumber)");
        if (year != null) queryString.append(" AND p.year = :year");
        if (cadastralReference != null) queryString.append(" AND LOWER(p.cadastralReference) LIKE LOWER(:cadastralReference)");

        Query query = entityManager.createQuery(queryString.toString()).setMaxResults(2);

        if (title != null) query.setParameter("title", "%" + title + "%");
        if (expedientNumber != null) query.setParameter("expedientNumber", "%" + expedientNumber + "%");
        if (year != null) query.setParameter("year", year);
        if (cadastralReference != null) query.setParameter("cadastralReference", "%" + cadastralReference + "%");

        return (List<Project>) query.getResultList();
    }

    public Slice<Project> searchProjectByKeywords(String title, String expedientNumber, Integer year, String cadastralReference, int page, int size, String columnOrder, String wayToOrder) {
        StringBuilder queryString = new StringBuilder("SELECT p FROM Project p WHERE 1=1");

        if (title != null) queryString.append(" AND LOWER(p.title) LIKE LOWER(:title)");
        if (expedientNumber != null) queryString.append(" AND LOWER(p.expedientNumber) LIKE LOWER(:expedientNumber)");
        if (year != null) queryString.append(" AND p.year = :year");
        if (cadastralReference != null) queryString.append(" AND LOWER(p.cadastralReference) LIKE LOWER(:cadastralReference)");

        if (columnOrder != null) {
            String mappedColumn = null;

            if (columnOrder.equalsIgnoreCase("title")) {
                mappedColumn = "title";
            } else if (columnOrder.equalsIgnoreCase("expedientNumber")) {
                mappedColumn = "expedientNumber";
            } else if (columnOrder.equalsIgnoreCase("year")) {
                mappedColumn = "year";
            } else if (columnOrder.equalsIgnoreCase("cadastralReference")) {
                mappedColumn = "cadastralReference";
            }

            if (mappedColumn != null) {
                String direction = "ASC";
                if ("desc".equalsIgnoreCase(wayToOrder)) {
                    direction = "DESC";
                }

                queryString.append(" ORDER BY p.").append(mappedColumn).append(" ").append(direction).append(" NULLS LAST");
            }
        }

        Query query = entityManager.createQuery(queryString.toString()).setFirstResult(page*size).setMaxResults(size + 1);

        if (title != null) query.setParameter("title", "%" + title + "%");
        if (expedientNumber != null) query.setParameter("expedientNumber", "%" + expedientNumber + "%");
        if (year != null) query.setParameter("year", year);
        if (cadastralReference != null) query.setParameter("cadastralReference", "%" + cadastralReference + "%");

        List<Project> projects = query.getResultList();
        boolean hasNext = projects.size() == size + 1;
        if (hasNext) {
            projects = projects.subList(0, size);
        }

        return new SliceImpl<>(projects, PageRequest.of(page, size), hasNext);
    }
}
