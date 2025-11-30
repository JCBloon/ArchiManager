package io.github.jnetbloon.amproject.model.exceptions;

public class MoreThanOneFoundException extends RuntimeException {
    public MoreThanOneFoundException(String message) {
        super(message);
    }
}
