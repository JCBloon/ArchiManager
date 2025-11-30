package io.github.jnetbloon.amproject.model.entities;

import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.persistence.Query;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Slice;
import org.springframework.data.domain.SliceImpl;

import java.util.ArrayList;
import java.util.List;

public class CustomizedClientDaoImpl implements CustomizedClientDao {

    @PersistenceContext
    private EntityManager entityManager;

    public List<Client> findClientsByKeywords(String name, String surname1, String surname2, String dni) {
        if (name == null &&  surname1 == null && surname2 == null && dni == null)
            return new ArrayList<>();

        // Usar 1=1 no queda elegante, pero ahorramos recursos y concatenamos correctamente las condiciones
        StringBuilder queryString = new StringBuilder("SELECT c FROM Client c WHERE 1=1");

        if (name != null) queryString.append(" AND LOWER(c.name) LIKE LOWER(:name)");
        if (surname1 != null) queryString.append(" AND LOWER(c.surname1) LIKE LOWER(:surname1)");
        if (surname2 != null) queryString.append(" AND LOWER(c.surname2) LIKE LOWER(:surname2)");
        if (dni != null) queryString.append(" AND LOWER(c.dni) LIKE LOWER(:dni)");

        Query query = entityManager.createQuery(queryString.toString()).setMaxResults(2);

        if (name != null) query.setParameter("name", "%" + name + "%");
        if (surname1 != null) query.setParameter("surname1", "%" + surname1 + "%");
        if (surname2 != null) query.setParameter("surname2", "%" + surname2 + "%");
        if (dni != null) query.setParameter("dni", "%" + dni + "%");

        return (List<Client>) query.getResultList();
    }

    public Slice<Client> searchClientsByKeywords(String name, String surname1, String surname2, String dni, int page, int size, String columnOrder, String wayToOrder) {
        StringBuilder queryString = new StringBuilder("SELECT c FROM Client c WHERE 1=1");

        if (name != null) queryString.append(" AND LOWER(c.name) LIKE LOWER(:name)");
        if (surname1 != null) queryString.append(" AND LOWER(c.surname1) LIKE LOWER(:surname1)");
        if (surname2 != null) queryString.append(" AND LOWER(c.surname2) LIKE LOWER(:surname2)");
        if (dni != null) queryString.append(" AND LOWER(c.dni) LIKE LOWER(:dni)");

        if (columnOrder != null) {
            String mappedColumn = null;
            if (columnOrder.equalsIgnoreCase("name")) {
                mappedColumn = "name";
            } else if (columnOrder.equalsIgnoreCase("surname1")) {
                mappedColumn = "surname1";
            } else if (columnOrder.equalsIgnoreCase("surname2")) {
                mappedColumn = "surname2";
            } else if (columnOrder.equalsIgnoreCase("dni")) {
                mappedColumn = "dni";
            }

            if (mappedColumn != null) {
                String direction = "ASC";
                if ("desc".equalsIgnoreCase(wayToOrder)) {
                    direction = "DESC";
                }

                queryString.append(" ORDER BY c.").append(mappedColumn).append(" ").append(direction).append(" NULLS LAST");
            }
        }

        // Sumarle 1 para comprobar que haya más disponibles
        Query query = entityManager.createQuery(queryString.toString()).setFirstResult(page*size).setMaxResults(size+1);

        if (name != null) query.setParameter("name", "%" + name + "%");
        if (surname1 != null) query.setParameter("surname1", "%" + surname1 + "%");
        if (surname2 != null) query.setParameter("surname2", "%" + surname2 + "%");
        if (dni != null) query.setParameter("dni", "%" + dni + "%");

        List<Client> clients = query.getResultList();
        boolean hasNext = clients.size() == size + 1;
        if(hasNext) {
            clients = clients.subList(0,size);
        }

        return new SliceImpl<>(clients, PageRequest.of(page, size), hasNext);
    }
}
