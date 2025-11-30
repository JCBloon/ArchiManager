CREATE TABLE client (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    dni VARCHAR(9) NOT NULL UNIQUE,
    name VARCHAR(40) NOT NULL,
    surname1 VARCHAR(40),
    surname2 VARCHAR(40),
    phone VARCHAR(15),
    address VARCHAR(100)
);

CREATE TABLE project (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(100),
    expedient_number VARCHAR(5) NOT NULL UNIQUE,
    year SMALLINT,
    cadastral_reference VARCHAR(20) NOT NULL,
    archive_number NUMERIC(3),
    comment TEXT
);

CREATE TABLE associated (
    client_id BIGINT NOT NULL,
    project_id BIGINT NOT NULL,
    PRIMARY KEY (client_id, project_id),
    CONSTRAINT fk_assoc_client_id FOREIGN KEY (client_id) REFERENCES client(id),
    CONSTRAINT fk_assoc_project_id FOREIGN KEY (project_id) REFERENCES project(id)
);