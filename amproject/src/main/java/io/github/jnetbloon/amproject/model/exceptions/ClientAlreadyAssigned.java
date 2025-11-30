package io.github.jnetbloon.amproject.model.exceptions;

public class ClientAlreadyAssigned extends RuntimeException {
    public ClientAlreadyAssigned(String message) {
        super(message);
    }
}
