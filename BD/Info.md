# MySQL Community Server 8.4.6 LTS x64
## Usuarios
	root - ####
	archiuser - archi

## Configuración
Pasos seguidos para configurar la BD

// Conectarse
mysql -u root -p 
	// En caso de estar usando un puerto distinto al default
		mysql -u root -p --port=10200

// Crear usuario
CREATE USER 'archiuser'@'localhost' IDENTIFIED BY 'archi';
CREATE DATABASE archidb;
CREATE DATABASE archidbtest;
GRANT ALL PRIVILEGES ON archidb.* to 'archiuser'@'localhost' WITH GRANT OPTION;
GRANT ALL PRIVILEGES ON archidbtest.* to 'archiuser'@'localhost' WITH GRANT OPTION;

// Cargar script con las tablas en la BD
mysql -u archiuser --password=archi archidb < scriptSQL.sql
mysql -u archiuser --password=archi archidbtest < scriptSQL.sql