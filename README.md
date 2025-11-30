# ArchiManager

**ArchiManager** es una aplicación  para windows de gestión de **clientes y proyectos**, diseñada con una arquitectura de **3 capas**:  
- **Frontend:** WinUI3 con .NET 8  
- **Backend:** Spring Boot  
- **Base de datos:** MySQL  

Permite almacenar, visualizar y modificar clientes y proyectos, asegurando que cada proyecto tenga siempre un cliente asignado. También incluye funcionalidades extra como la generación de PDF de clientes.

---

## 🔹 Tecnologías usadas

| Capa | Tecnología | Notas |
|------|------------|-------|
| Frontend | WinUI3, .NET 8 | Patrones MVVM: View pasiva, ViewModel con lógica de presentación, Model con datos y lógica de negocio. La guía realizada incluye aprendizaje de C#, XAML y sus componentes. |
| Backend | Spring Boot (Maven) | Patrón Controller-Service-Repository: Controller recibe peticiones, Service gestiona la lógica de negocio, Repository maneja la persistencia con JPA/Hibernate. Internacionalización Español/Inglés con LocaleConfiguration. No hay autenticación. Dependencias principales: Spring Boot Starter Web, Data JPA, Validation, MySQL Connector y Jackson XML. |
| Base de datos | MySQL Community Server 8.4.6 LTS x64 | Base de datos relacional, con tablas para clientes, proyectos y asociaciones. |

---

## 🏗 Arquitectura y patrones de diseño

### Frontend WinUI3
- **MVVM (Model-View-ViewModel)**  
  La **Vista** se enlaza al **ViewModel** mediante bindings y contiene mínima lógica.  
  El **ViewModel** maneja la presentación y actualiza el **Model**, que contiene datos y lógica de negocio.

### Backend Spring Boot
- **Controller-Service-Repository**  
  - **Controller:** gestiona solicitudes y respuestas HTTP  
  - **Service:** lógica de negocio  
  - **Repository:** interacción con la base de datos usando JPA/Hibernate

---

## ⚙️ Funcionalidades

- Crear, visualizar y modificar **clientes**  
- Crear, visualizar y modificar **proyectos**  
- Cada proyecto debe tener un cliente asignado  
- Generación de PDF con información de clientes  

> Todo esto está documentado en la documentación interna para comprender la implementación y flujo de datos.

---

## 🗄 Modelo de datos (resumen conceptual)

- **Clientes:** identificados por DNI, con nombre, apellidos, teléfono y dirección.  
- **Proyectos:** cada proyecto tiene título, número de expediente, año, referencia catastral, número de archivo y comentarios.  
- **Relación:** un cliente puede tener varios proyectos y viceversa. Un proyecto debe estar asociado a mínimo un cliente.

---

## ⚙️ Requisitos e instalación

- **Base de datos:** 
  - MySQL Community Server 8.4.6 LTS x64
- **Backend:**
  - Java 17
  - Maven
- **Frontend:**
  - .NET 8  
  - Visual Studio 2022
  - WinUI3

---

## 🚀 Ejecución

1. Levantar MySQL y crear la base de datos con el script.
2. Ejecutar backend Spring Boot con Maven.
3. Ejecutar frontend WinUI3 desde Visual Studio.

**NOTA:** Para la BD y el frontend, se incluyen guías más detalladas para su preparación. Se recomienda integrar el programa para facilitar el proceso. Leer documentación para más detalles.

---

## 📃 Guías

**Base de Datos**: Versión y consultas necesarias para la configuración de la BD

**Frontend**: A modo de curso-explicación, se incluye un conjunto de ficheros que explican:
    - Tecnologías consideradas y presentación a .NET y WinUI3
    - Introducción a XAML
    - Introducción a C#
    - Implmentación
    - Notas sobre varias posibles formas de integrar

---

## 📂 Estructura del proyecto

```
ArchiManager/
├─ amproject/           # Backend Java-Maven
├─ ArchiManagerWinUI/   # Frontend .NET-WinUI3
├─ BD/                  # BD
│ ├── Info.md               # Instrucciones para la BD
│ └── scriptSQL.sql         # Script SQL
├─ GuiaFrontend/        # Documentación para front e integración
└─ README.md
```

---

## ❗ Errores conocidos

Estos se irán arreglando con el mantenimiento:

- Imagen en la barra de tareas del programa incorrecta al ejecutarse

## 📄 Licencia

Este proyecto está bajo la licencia **MIT**.
