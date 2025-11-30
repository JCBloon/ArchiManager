package io.github.jnetbloon.amproject.rest.controllers;

import io.github.jnetbloon.amproject.model.exceptions.*;
import io.github.jnetbloon.amproject.rest.common.ErrorDto;
import io.github.jnetbloon.amproject.rest.common.FieldErrorDto;
import jakarta.persistence.Entity;
import jakarta.servlet.http.HttpServletRequest;
import org.apache.tomcat.util.http.fileupload.impl.InvalidContentTypeException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import java.time.LocalDateTime;
import java.util.List;

@RestControllerAdvice
public class CommonController {

    /*
    // OTRA FORMA DE RESOLVER LA EXPECION
    // Es menos flexible, no puedes modificar el mensaje de error del encabezado
    @ExceptionHandler(MethodArgumentNotValidException.class)
    @ResponseStatus(HttpStatus.BAD_REQUEST)
    public ErrorDto handleMethodArgumentNotValidException(MethodArgumentNotValidException ex) {
        List<FieldErrorDto> fieldErrors = ex.getBindingResult().getFieldErrors().stream().map(error -> new FieldErrorDto(error.getField(), error.getDefaultMessage())).toList();
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.BAD_REQUEST.value(), HttpStatus.BAD_REQUEST.getReasonPhrase(), ;
        return error;
    }*/

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<?> handleMethodArgumentNotValidException(MethodArgumentNotValidException ex, HttpServletRequest request) {
        // Java 16+ permite toList(), si no se usaría .collect(Collectors.toList())
        List<FieldErrorDto> fieldErrors = ex.getBindingResult().getFieldErrors().stream()
                .map(error -> new FieldErrorDto(error.getField(), error.getDefaultMessage())).toList();
        // Recordatorio: Obtener la uri de la petición para saber la ruta relativa al recurso, no la ruta completa
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.BAD_REQUEST.value(),
                HttpStatus.BAD_REQUEST.getReasonPhrase(), "Invalid Request", request.getRequestURI(), fieldErrors);

        return new ResponseEntity<>(error, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(InstanceNotFoundException.class)
    public ResponseEntity<?> handleInstanceNotFoundException(InstanceNotFoundException ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.NOT_FOUND.value(),
                HttpStatus.NOT_FOUND.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.NOT_FOUND);
    }

    @ExceptionHandler(InvalidContentTypeException.class)
    public ResponseEntity<?> handleInvalidContentTypeException(InvalidContentTypeException ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.BAD_REQUEST.value(),
                HttpStatus.BAD_REQUEST.getReasonPhrase(), "Invalid Request",  request.getRequestURI(), null);

        return new ResponseEntity<>(error, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(MoreThanOneFoundException.class)
    public ResponseEntity<?> handleMoreThanOneFoundException(MoreThanOneFoundException ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.CONFLICT.value(),
                HttpStatus.CONFLICT.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new  ResponseEntity<>(error, HttpStatus.CONFLICT);
    }

    @ExceptionHandler(DataAlreadyAdded.class)
    public ResponseEntity<?> handleDataAlreadyAdded(DataAlreadyAdded ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.CONFLICT.value(),
                HttpStatus.CONFLICT.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.CONFLICT);
    }

    @ExceptionHandler(FormatError.class)
    public ResponseEntity<?> handleFormatError(FormatError ex, HttpServletRequest request) {
        ErrorDto error = new ErrorDto(LocalDateTime.now(), HttpStatus.BAD_REQUEST.value(),
                HttpStatus.BAD_REQUEST.getReasonPhrase(), ex.getMessage(), request.getRequestURI(),null);
        return new ResponseEntity<>(error, HttpStatus.BAD_REQUEST);
    }

}
