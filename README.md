# DEFIANTS Backend API Documentation

## 1. Introducción

Este documento proporciona la información esencial para que el desarrollador frontend (Blazor) pueda interactuar con la API RESTful del backend de DEFIANTS. Cubre la autenticación, la estructura de los datos, los endpoints disponibles y los flujos de uso principales.

**Tecnologías del Backend:**
*   **ASP.NET Core Web API:** Framework principal para la API.
*   **Entity Framework Core:** ORM para la interacción con la base de datos.
*   **PostgreSQL:** Base de datos relacional.
*   **JWT (JSON Web Tokens):** Para autenticación y autorización.

**Tecnologías del Frontend:**
*   **Blazor:** Framework para la interfaz de usuario.

## 2. URL Base y Entorno

La URL base para la API en un entorno de desarrollo local es:

`http://localhost:5031` (o `https://localhost:7123` si tienes HTTPS configurado)

**Nota:** Esta URL cambiará para entornos de staging o producción.

## 3. Autenticación y Autorización (JWT Bearer Token)

El backend utiliza JSON Web Tokens (JWT) para gestionar la autenticación y la autorización de los usuarios.

### 3.1. Flujo de Autenticación

1.  **Login:** El usuario envía sus credenciales (`username`, `password`) al endpoint de login (`/api/auth/login`).
2.  **Token:** Si las credenciales son correctas, el backend devuelve un JWT.
3.  **Almacenamiento:** El frontend debe almacenar este JWT de forma segura (por ejemplo, en `localStorage` o `sessionStorage` del navegador).
4.  **Peticiones Protegidas:** Para acceder a cualquier endpoint que requiera autenticación, el frontend debe incluir el JWT en la cabecera `Authorization` de cada petición, con el formato:
    `Authorization: Bearer <tu_token_jwt_aqui>`

### 3.2. Detalles del JWT

*   **Expiración del Token:** Los tokens generados expiran después de **3 horas**. El frontend debe manejar la expiración (ej. redirigir al usuario a la página de login o intentar refrescar el token si se implementa esa funcionalidad).
*   **Roles en el Token:** El JWT contiene los roles del usuario (ej. "Jugador", "Admin") como `ClaimTypes.Role`. El frontend puede decodificar el token (aunque no debe confiar ciegamente en él para decisiones de seguridad, solo para la lógica de la UI) para adaptar la interfaz de usuario.
*   **Política de Contraseñas:** Por defecto, ASP.NET Core Identity requiere contraseñas que:
    *   Tengan al menos 6 caracteres.
    *   Contengan al menos un carácter no alfanumérico (ej. `!`, `@`, `#`).
    *   Contengan al menos un dígito (`0`-`9`).
    *   Contengan al menos una letra mayúscula (`A`-`Z`).

### 3.3. Configuración del Primer Administrador

Para que el sistema de roles funcione, el primer usuario con rol "Admin" debe ser configurado manualmente:
1.  Registra un usuario normal (ej. `adminuser`) a través de `POST /api/auth/register`.
2.  Accede directamente a la base de datos PostgreSQL (ej. con DataGrip) y asigna el rol "Admin" a este usuario en la tabla `AspNetUserRoles`.
3.  Inicia sesión con `adminuser` para obtener un JWT que incluya el rol "Admin".

## 4. Manejo de Errores

El backend utiliza códigos de estado HTTP estándar para indicar el resultado de las operaciones.

*   **`200 OK`**: La petición fue exitosa.
*   **`201 Created`**: El recurso fue creado exitosamente (respuesta a `POST`).
*   **`204 No Content`**: La petición fue exitosa, pero no hay contenido que devolver (respuesta a `PUT`, `DELETE`).
*   **`400 Bad Request`**: La petición es inválida debido a datos incorrectos o una violación de reglas de negocio (ej. contraseña débil, no hay suficientes equipos para iniciar un torneo). La respuesta contendrá un JSON con detalles del error.
*   **`401 Unauthorized`**: La petición requiere autenticación (falta el token JWT o es inválido/expirado).
*   **`403 Forbidden`**: El token JWT es válido, pero el usuario no tiene los permisos (roles) necesarios para realizar la acción.
*   **`404 Not Found`**: El recurso solicitado no existe.
*   **`409 Conflict`**: La petición no pudo ser completada debido a un conflicto con el estado actual del recurso (ej. intentar registrar un `username` ya existente, inscribir un equipo ya inscrito).
*   **`500 Internal Server Error`**: Una excepción no controlada ocurrió en el servidor.

## 5. Data Transfer Objects (DTOs)

El backend utiliza DTOs (Data Transfer Objects) para definir el contrato de datos entre el frontend y el backend.

*   **Propósito:**
    *   Evitar ciclos de referencia al serializar objetos complejos de Entity Framework.
    *   Exponer solo los datos necesarios al cliente, mejorando la seguridad y eficiencia.
*   **Ubicación:** Todos los DTOs están definidos en el proyecto `DEFIANTS.Shared/DTOs`.
*   **Recomendación para Blazor:** El proyecto `DEFIANTS.Client` (Blazor) debería **referenciar directamente** el proyecto `DEFIANTS.Shared`. Esto permite reutilizar las clases DTOs y las validaciones (`[Required]`, `[StringLength]`) en el frontend, asegurando que ambos extremos de la aplicación utilicen la misma definición de datos.

## 6. Referencia de Endpoints de la API

Los endpoints están agrupados por controlador. Los que requieren autenticación se indican con **(Auth)**. Los que requieren roles específicos se indican con **(Auth, Rol)**.

### 6.1. `AuthController` (`/api/auth`)

*   **`POST /register`** (Público)
    *   **Descripción:** Registra un nuevo usuario en la plataforma. Por defecto, se le asigna el rol "Jugador".
    *   **DTO de Entrada:** `RegisterModel` (en `AuthController.cs` - se recomienda mover a `DEFIANTS.Shared/DTOs`).
        ```
        public class RegisterModel
        {
            public required string Username { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
        ```
    *   **Respuesta:** `200 OK` con `{ status: "Success", message: "User created successfully!" }`. `400 Bad Request` con `{ status: "Error", errors: [...] }` si la contraseña es débil o `409 Conflict` si el usuario ya existe.

*   **`POST /login`** (Público)
    *   **Descripción:** Autentica un usuario y devuelve un JWT.
    *   **DTO de Entrada:** `LoginModel` (en `AuthController.cs` - se recomienda mover a `DEFIANTS.Shared/DTOs`).
        ```
        public class LoginModel
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }
        ```
    *   **Respuesta:** `200 OK` con `{ token: "...", expiration: "..." }`. `401 Unauthorized` si las credenciales son incorrectas.

### 6.2. `JuegosController` (`/api/juegos`)

*   **`GET /`** (Público)
    *   **Descripción:** Obtiene una lista de todos los juegos disponibles en la plataforma.
    *   **Respuesta:** `200 OK` con `List<Juego>`.

*   **`GET /{id}`** (Público)
    *   **Descripción:** Obtiene los detalles de un juego específico por su ID.
    *   **Respuesta:** `200 OK` con `Juego`. `404 Not Found` si el juego no existe.

### 6.3. `PerfilesJuegoController` (`/api/perfilesjuego`)

*   **`GET /misperfiles`** (Auth)
    *   **Descripción:** Obtiene todos los perfiles de juego del usuario autenticado.
    *   **Respuesta:** `200 OK` con `List<PerfilJuego>`.

*   **`POST /`** (Auth)
    *   **Descripción:** Crea un nuevo perfil de juego para el usuario autenticado.
    *   **DTO de Entrada:** `CrearPerfilJuegoDto`.
    *   **Respuesta:** `200 OK` con `PerfilJuego` creado. `409 Conflict` si el usuario ya tiene un perfil para ese juego.

*   **`PUT /{id}`** (Auth)
    *   **Descripción:** Actualiza el nickname de un perfil de juego específico del usuario autenticado.
    *   **DTO de Entrada:** `ActualizarPerfilJuegoDto`.
    *   **Respuesta:** `204 No Content`. `404 Not Found` si el perfil no existe. `403 Forbidden` si el perfil no pertenece al usuario.

### 6.4. `EquiposController` (`/api/equipos`)

*   **`GET /`** (Público)
    *   **Descripción:** Obtiene una lista de todos los equipos registrados.
    *   **Respuesta:** `200 OK` con `List<{ Id, Nombre, JuegoId }>`.

*   **`GET /misequipos`** (Auth)
    *   **Descripción:** Obtiene una lista de los equipos a los que pertenece el usuario autenticado.
    *   **Respuesta:** `200 OK` con `List<{ Id, Nombre, JuegoId, Rol }>`.

*   **`GET /{id}`** (Público)
    *   **Descripción:** Obtiene los detalles completos de un equipo, incluyendo sus miembros.
    *   **Respuesta:** `200 OK` con `EquipoDetalleDto`. `404 Not Found` si el equipo no existe.

*   **`POST /`** (Auth)
    *   **Descripción:** Crea un nuevo equipo. El usuario autenticado se convierte en el capitán.
    *   **DTO de Entrada:** `CrearEquipoDto`.
    *   **Respuesta:** `201 Created` con `EquipoDetalleDto` del equipo creado. `400 Bad Request` si el usuario no tiene perfil para el juego.

*   **`PUT /{id}`** (Auth, Capitán)
    *   **Descripción:** Actualiza el nombre de un equipo. Solo el capitán puede hacerlo.
    *   **DTO de Entrada:** `CrearEquipoDto` (solo se usa `Nombre`).
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es el capitán.

*   **`POST /{equipoId}/miembros`** (Auth, Capitán)
    *   **Descripción:** Añade un miembro a un equipo. Solo el capitán puede hacerlo.
    *   **DTO de Entrada:** `InvitarMiembroDto`.
    *   **Respuesta:** `200 OK` con `{ message: string }`. `400 Bad Request` si el usuario a invitar no existe o no tiene perfil para el juego. `409 Conflict` si ya es miembro.

*   **`PUT /{equipoId}/miembros/{miembroId}/rol`** (Auth, Capitán)
    *   **Descripción:** Actualiza el rol de un miembro del equipo. Solo el capitán puede hacerlo.
    *   **DTO de Entrada:** `ActualizarRolMiembroDto`.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es el capitán. `400 Bad Request` si intenta cambiar el rol del capitán a algo que no sea "Lider".

*   **`DELETE /{equipoId}/miembros/{miembroId}`** (Auth, Capitán)
    *   **Descripción:** Expulsa a un miembro del equipo. Solo el capitán puede hacerlo.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es el capitán. `400 Bad Request` si intenta expulsar al capitán.

*   **`DELETE /{id}`** (Auth, Capitán)
    *   **Descripción:** Disuelve un equipo, eliminando todos sus miembros. Solo el capitán puede hacerlo.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es el capitán.

### 6.5. `TorneosController` (`/api/torneos`)

*   **`GET /`** (Público)
    *   **Descripción:** Obtiene una lista de todos los torneos disponibles.
    *   **Respuesta:** `200 OK` con `List<{ Id, Titulo, Status, MaxEquipos }>`.

*   **`GET /misinscripciones`** (Auth)
    *   **Descripción:** Obtiene una lista de los torneos en los que el usuario autenticado tiene un equipo inscrito.
    *   **Respuesta:** `200 OK` con `List<{ Id, Titulo, Status, NombreEquipo }>`.

*   **`GET /{id}`** (Público)
    *   **Descripción:** Obtiene los detalles completos de un torneo específico, incluyendo su bracket de partidos.
    *   **Respuesta:** `200 OK` con `TorneoDetalleDto`. `404 Not Found` si el torneo no existe.

*   **`GET /{torneoId}/inscripciones`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Lista todas las inscripciones para un torneo específico.
    *   **Respuesta:** `200 OK` con `List<{ Id, EquipoId, Nombre, EstadoPago }>`. `403 Forbidden` si no es Admin o creador.

*   **`POST /`** (Auth)
    *   **Descripción:** Crea un nuevo torneo. El usuario autenticado se convierte en el creador.
    *   **DTO de Entrada:** `CrearTorneoDto`.
    *   **Respuesta:** `201 Created` con `Torneo` creado.

*   **`PUT /{id}`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Actualiza los detalles de un torneo.
    *   **DTO de Entrada:** `CrearTorneoDto`.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es Admin o creador. `400 Bad Request` si el torneo ya ha comenzado.

*   **`DELETE /{id}`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Cancela un torneo (cambia su estado a `Cancelado`).
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es Admin o creador.

*   **`POST /{torneoId}/inscripciones`** (Auth)
    *   **Descripción:** Inscribe un equipo en un torneo. Solo el capitán del equipo puede hacerlo.
    *   **DTO de Entrada:** `InscribirEquipoDto`.
    *   **Respuesta:** `200 OK` con `{ message: string }`. `400 Bad Request` si el torneo no está abierto, el juego no coincide, etc. `409 Conflict` si ya está inscrito o no hay cupos.

*   **`DELETE /{torneoId}/inscripciones/{inscripcionId}`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Cancela una inscripción específica de un torneo.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es Admin o creador.

*   **`POST /{id}/iniciar`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Inicia el torneo y genera el bracket de partidos.
    *   **Respuesta:** `200 OK` con `{ message: string }`. `403 Forbidden` si no es Admin o creador. `400 Bad Request` si no hay suficientes equipos inscritos.

*   **`POST /partidos/{partidoId}/victoria`** (Auth, Admin/Creador del Torneo)
    *   **Descripción:** Reporta la victoria de un equipo en un partido, propagando el ganador al siguiente partido del bracket.
    *   **DTO de Entrada:** `int` (ID del equipo ganador).
    *   **Respuesta:** `200 OK` con `{ message: string }`. `403 Forbidden` si no es Admin o creador. `400 Bad Request` si el ganador no es un participante o el partido ya está finalizado.

### 6.6. `PartidosController` (`/api/partidos`)

*   **`GET /mispartidos`** (Auth)
    *   **Descripción:** Obtiene una lista de los partidos en los que el usuario autenticado está involucrado.
    *   **Respuesta:** `200 OK` con `List<{ TorneoTitulo, EquipoA, EquipoB, Ronda, Estado, EquipoGanadorId, ScoreA, ScoreB }>`.

*   **`GET /{id}`** (Público)
    *   **Descripción:** Obtiene los detalles de un partido específico.
    *   **Respuesta:** `200 OK` con `PartidoDto`. `404 Not Found` si el partido no existe.

*   **`PUT /{id}`** (Auth, Admin)
    *   **Descripción:** Permite a un administrador corregir manualmente el resultado de un partido.
    *   **DTO de Entrada:** `CorregirPartidoDto`.
    *   **Respuesta:** `204 No Content`. `403 Forbidden` si no es Admin.

### 6.7. `AdminController` (`/api/admin`)

*   **`GET /users`** (Auth, Admin)
    *   **Descripción:** Lista todos los usuarios registrados en la plataforma.
    *   **Respuesta:** `200 OK` con `List<{ Id, UserName, Email }>`.

*   **`GET /users/{id}`** (Auth, Admin)
    *   **Descripción:** Obtiene los detalles de un usuario específico, incluyendo sus roles.
    *   **Respuesta:** `200 OK` con `{ id, userName, email, roles: List<string> }`.

*   **`POST /assign-role`** (Auth, Admin)
    *   **Descripción:** Asigna un rol a un usuario.
    *   **DTO de Entrada:** `UpdateRoleDto`.
    *   **Respuesta:** `200 OK` con `{ message: string }`. `400 Bad Request` si el usuario o rol no existen.

*   **`DELETE /users/{id}/roles/{roleName}`** (Auth, Admin)
    *   **Descripción:** Elimina un rol específico de un usuario.
    *   **Respuesta:** `204 No Content`. `400 Bad Request` si el usuario o rol no existen.

*   **`POST /juegos`** (Auth, Admin)
    *   **Descripción:** Crea un nuevo juego en la plataforma.
    *   **DTO de Entrada:** `CrearJuegoDto`.
    *   **Respuesta:** `201 Created` con `Juego` creado.

*   **`PUT /juegos/{id}`** (Auth, Admin)
    *   **Descripción:** Actualiza los detalles de un juego existente.
    *   **DTO de Entrada:** `CrearJuegoDto`.
    *   **Respuesta:** `204 No Content`. `404 Not Found` si el juego no existe.

## 7. Secuencia General de Uso de la Página

### 7.1. Flujo de Usuario Estándar

1.  **Registro:**
    *   El usuario se registra usando `POST /api/auth/register`.
2.  **Login:**
    *   El usuario inicia sesión usando `POST /api/auth/login` para obtener su JWT.
    *   El frontend almacena este token.
3.  **Crear Perfil de Juego:**
    *   El usuario crea un perfil para un juego (ej. "League of Legends") usando `POST /api/perfilesjuego`.
4.  **Crear Equipo:**
    *   El usuario crea un equipo, convirtiéndose en el capitán, usando `POST /api/equipos`.
5.  **Gestionar Equipo (Opcional):**
    *   El capitán puede invitar a otros jugadores (`POST /api/equipos/{id}/miembros`) o cambiar roles (`PUT /api/equipos/{id}/miembros/{id}/rol`).
6.  **Explorar Torneos:**
    *   El usuario ve los torneos disponibles usando `GET /api/torneos`.
7.  **Inscribir Equipo:**
    *   El capitán inscribe su equipo en un torneo usando `POST /api/torneos/{id}/inscripciones`.
8.  **Ver Actividad:**
    *   El usuario puede ver sus equipos (`GET /api/equipos/misequipos`), sus inscripciones (`GET /api/torneos/misinscripciones`) y sus partidos (`GET /api/partidos/mispartidos`).

### 7.2. Flujo de Administrador

1.  **Configuración Inicial (Manual):**
    *   Un usuario se registra.
    *   **Manualmente** en la base de datos, se le asigna el rol "Admin" en la tabla `AspNetUserRoles`.
2.  **Login:**
    *   El administrador inicia sesión usando `POST /api/auth/login` para obtener un JWT que incluye el rol "Admin".
    *   El frontend almacena este token.
3.  **Gestionar Juegos:**
    *   Crea nuevos juegos (`POST /api/admin/juegos`).
    *   Actualiza juegos existentes (`PUT /api/admin/juegos/{id}`).
4.  **Gestionar Torneos:**
    *   Crea nuevos torneos (`POST /api/torneos`).
    *   Actualiza torneos (`PUT /api/torneos/{id}`).
    *   Cancela torneos (`DELETE /api/torneos/{id}`).
    *   Lista inscripciones de un torneo (`GET /api/torneos/{id}/inscripciones`).
    *   Cancela inscripciones (`DELETE /api/torneos/{id}/inscripciones/{id}`).
    *   Inicia torneos (`POST /api/torneos/{id}/iniciar`).
5.  **Gestionar Partidos:**
    *   Reporta resultados de partidos (`POST /api/torneos/partidos/{id}/victoria`).
    *   Corrige resultados de partidos (`PUT /api/partidos/{id}`).
6.  **Gestionar Usuarios y Roles:**
    *   Lista todos los usuarios (`GET /api/admin/users`).
    *   Ve detalles de un usuario (`GET /api/admin/users/{id}`).
    *   Asigna roles a usuarios (`POST /api/admin/assign-role`).
    *   Elimina roles de usuarios (`DELETE /api/admin/users/{id}/roles/{roleName}`).

## 8. Cómo Empezar (Desarrollador Frontend)

1.  **Clonar el Repositorio:** Asegúrate de tener el código fuente del backend.
2.  **Configurar la Base de Datos:**
    *   Asegúrate de tener PostgreSQL instalado y en ejecución.
    *   Ajusta la cadena de conexión en `DEFIANTS.Server/appsettings.json` para que apunte a tu instancia de PostgreSQL.
    *   Abre una terminal en la carpeta `DEFIANTS.Server` y ejecuta:
        ```sh
        dotnet ef database update
        ```
        Esto creará el esquema de la base de datos.
3.  **Ejecutar el Backend:**
    *   Inicia el proyecto `DEFIANTS.Server` desde tu IDE (Rider, Visual Studio).
    *   El backend estará disponible en la URL base mencionada anteriormente.
4.  **Acceder a Swagger UI:**
    *   Navega a `http://localhost:5031/swagger` (o la URL correspondiente) en tu navegador.
    *   Utiliza Swagger UI para probar los endpoints y entender sus respuestas.
    *   Recuerda usar el botón "Authorize" en Swagger para pegar tu token JWT después de iniciar sesión.
5.  **Referenciar el Proyecto `Shared`:**
    *   En tu proyecto Blazor (`DEFIANTS.Client`), añade una referencia al proyecto `DEFIANTS.Shared`. Esto te permitirá usar directamente los DTOs y Enums definidos en el backend, facilitando la comunicación y la validación.
6.  **Configurar CORS:**
    *   El backend ya está configurado para aceptar peticiones desde `http://localhost:5000` y `https://localhost:5001` (URLs de desarrollo comunes para Blazor). Si tu frontend se ejecuta en un puerto diferente, deberás añadir esa URL a la política de CORS en `DEFIANTS.Server/Program.cs`.
