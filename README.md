# Guía Rápida para el Frontend: Consumo de API y Autenticación

Esta sección contiene las prácticas recomendadas para interactuar con el backend desde el proyecto Blazor.

### Consumo de la API con Refit (`IApiClient`)

Para todas las llamadas al backend, **se debe utilizar la interfaz `IApiClient`**. Esta interfaz, definida en `DEFIANTS.Shared/Clients/IApiClient.cs`, es la única fuente de verdad para la comunicación con la API.

**¿Qué es y por qué usarlo?**

`IApiClient` utiliza una librería llamada **Refit** para convertir la API REST en una interfaz de C# fuertemente tipada. En lugar de escribir URLs como cadenas de texto, llamas a métodos de C# como si estuvieras usando una librería local.

**Ventajas Clave:**
*   **Cero Errores de Tipeo:** El compilador te avisará si escribes mal el nombre de un método o si los parámetros no coinciden. Se acabaron los errores 404 por una URL mal escrita.
*   **Fuertemente Tipado:** Trabajas directamente con tus DTOs (`TorneoResumenDto`, `LoginDto`, etc.). El compilador asegura que los datos que envías y recibes son del tipo correcto.
*   **Autocompletado y Claridad:** Tu IDE te ofrecerá autocompletado para todos los endpoints disponibles. El código se vuelve mucho más legible.
*   **Autenticación Integrada:** Ya está configurado para adjuntar automáticamente el token JWT a las peticiones que lo necesiten. No tienes que preocuparte por añadir la cabecera `Authorization` manualmente.

**Cómo Usarlo en un Componente Blazor:**

1.  **Inyectar el cliente** en la parte superior de tu archivo `.razor`:
    ```csharp
    @using DEFIANTS.Shared.Clients
    @inject IApiClient ApiClient
    ```

2.  **Llamar a un método** dentro de tu bloque `@code`:
    ```csharp
    private List<TorneoResumenDto> torneos;

    protected override async Task OnInitializedAsync()
    {
        // SIN REFIT (Propenso a errores):
        // torneos = await Http.GetFromJsonAsync<List<TorneoResumenDto>>("api/torneoss"); // ¡Error de tipeo en la URL!

        // CON REFIT (Limpio y seguro):
        torneos = await ApiClient.GetTorneos(); 
    }
    ```

### Autenticación en la Interfaz de Usuario

El sistema de autenticación del frontend ya está configurado. Para usarlo, tienes dos herramientas principales:

#### a) Proteger Páginas Completas con `[Authorize]`

Para restringir el acceso a una página entera (como un panel de control) a usuarios autenticados, añade el atributo `@attribute [Authorize]` en la parte superior del archivo `.razor`.

**Ejemplo (`Dashboard.razor`):**
```csharp
@page "/cuenta/dashboard"
@attribute [Authorize] // <-- Si un anónimo intenta entrar, es redirigido al login.

<PageTitle>Mi Panel</PageTitle>
<h1>¡Bienvenido!</h1>
```

**Para proteger por roles:**
```csharp
@attribute [Authorize(Roles = "Admin, Organizador")]
```

#### b) Mostrar/Ocultar Elementos con `<AuthorizeView>` (Ejemplo en `Home.razor`)

Para mostrar diferentes botones, menús o cualquier elemento en la misma página dependiendo de si el usuario está logueado, usa el componente `<AuthorizeView>`.

**Caso Práctico: Adaptar los botones de la página de inicio.**

En tu `Home.razor`, tenemos una sección "Hero" y una sección "CTA Final". Ambas deben mostrar diferentes botones si el usuario ha iniciado sesión.

**Código de Ejemplo para `Home.razor`:**

```html
<!-- En la sección "Hero" -->
<div class="hero-actions">
    <AuthorizeView>
        <Authorized>
            <!-- Contenido para usuarios AUTENTICADOS -->
            <!-- Le damos la bienvenida y un enlace a su panel. -->
            <a href="/cuenta/dashboard" class="btn btn-primary">IR A MI PANEL</a>
            <a href="/torneos" class="btn btn-secondary btn-ghost">
                <i class="fas fa-trophy"></i> Explorar Torneos
            </a>
        </Authorized>
        <NotAuthorized>
            <!-- Contenido para usuarios ANÓNIMOS -->
            <!-- Lo invitamos a unirse o iniciar sesión. -->
            <a href="/login" class="btn btn-primary">UNIRSE A LA ARENA</a>
            <a href="/noticias" class="btn btn-secondary btn-ghost">
                <i class="fas fa-bullhorn"></i> Ver noticias
            </a>
        </NotAuthorized>
    </AuthorizeView>
</div>
```

*   **`<Authorized>`**: El HTML dentro de esta etiqueta solo se renderiza si el `CustomAuthenticationStateProvider` confirma que hay un token válido.
*   **`<NotAuthorized>`**: El HTML aquí solo se renderiza si no hay un token o este es inválido.
*   **Acceder a Datos del Usuario:** Dentro de la sección `<Authorized>`, puedes usar la variable `context` para personalizar aún más la experiencia: `<span>Hola, @context.User.Identity?.Name!</span>`.

---
---

# DEFIANTS Backend API Documentation (Referencia Completa)

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
    *   **DTO de Entrada:** `RegisterDto`.
    *   **Respuesta:** `200 OK`. `400 Bad Request` con `ErrorResponseDto` si la validación falla. `409 Conflict` si el usuario ya existe.

*   **`POST /login`** (Público)
    *   **DTO de Entrada:** `LoginDto`.
    *   **Respuesta:** `200 OK` con `LoginResultDto`. `401 Unauthorized` si las credenciales son incorrectas.

### 6.2. `JuegosController` (`/api/juegos`)

*   **`GET /`** (Público)
    *   **Respuesta:** `200 OK` con `List<JuegoDto>`.

*   **`GET /{id}`** (Público)
    *   **Respuesta:** `200 OK` con `JuegoDto`.

### 6.3. `PerfilesJuegoController` (`/api/perfilesjuego`)

*   **`GET /misperfiles`** (Auth)
    *   **Respuesta:** `200 OK` con `List<PerfilJuegoDto>`.

*   **`POST /`** (Auth)
    *   **DTO de Entrada:** `CrearPerfilJuegoDto`.
    *   **Respuesta:** `200 OK` con la entidad `PerfilJuego` creada.

*   **`PUT /{id}`** (Auth)
    *   **DTO de Entrada:** `ActualizarPerfilJuegoDto`.
    *   **Respuesta:** `204 No Content`.

### 6.4. `EquiposController` (`/api/equipos`)

*   **`GET /`** (Público)
    *   **Respuesta:** `200 OK` con `List<EquipoResumenDto>`.

*   **`GET /misequipos`** (Auth)
    *   **Respuesta:** `200 OK` con `List<MiEquipoDto>`.

*   **`GET /{id}`** (Público)
    *   **Respuesta:** `200 OK` con `EquipoDetalleDto`.

*   **`POST /`** (Auth)
    *   **DTO de Entrada:** `CrearEquipoDto`.
    *   **Respuesta:** `201 Created` con `EquipoDetalleDto`.

*   **`PUT /{id}`** (Auth, Capitán)
    *   **DTO de Entrada:** `CrearEquipoDto` (solo se usa `Nombre`).
    *   **Respuesta:** `204 No Content`.

*   **`POST /{equipoId}/miembros`** (Auth, Capitán)
    *   **DTO de Entrada:** `InvitarMiembroDto`.
    *   **Respuesta:** `200 OK`.

*   **`PUT /{equipoId}/miembros/{miembroId}/rol`** (Auth, Capitán)
    *   **DTO de Entrada:** `ActualizarRolMiembroDto`.
    *   **Respuesta:** `204 No Content`.

*   **`DELETE /{equipoId}/miembros/{miembroId}`** (Auth, Capitán)
    *   **Respuesta:** `204 No Content`.

*   **`DELETE /{id}`** (Auth, Capitán)
    *   **Respuesta:** `204 No Content`.

### 6.5. `TorneosController` (`/api/torneos`)

*   **`GET /`** (Público)
    *   **Respuesta:** `200 OK` con `List<TorneoResumenDto>`.

*   **`GET /misinscripciones`** (Auth)
    *   **Respuesta:** `200 OK` con `List<MiInscripcionDto>`.

*   **`GET /{id}`** (Público)
    *   **Respuesta:** `200 OK` con `TorneoDetalleDto`.

*   **`GET /{torneoId}/inscripciones`** (Auth, Admin/Creador)
    *   **Respuesta:** `200 OK` con `List<InscripcionDetalleDto>`.

*   **`POST /`** (Auth)
    *   **DTO de Entrada:** `CrearTorneoDto`.
    *   **Respuesta:** `201 Created` con la entidad `Torneo` creada.

*   **`PUT /{id}`** (Auth, Admin/Creador)
    *   **DTO de Entrada:** `CrearTorneoDto`.
    *   **Respuesta:** `204 No Content`.

*   **`DELETE /{id}`** (Auth, Admin/Creador)
    *   **Respuesta:** `204 No Content`.

*   **`POST /{torneoId}/inscripciones`** (Auth, Capitán)
    *   **DTO de Entrada:** `InscribirEquipoDto`.
    *   **Respuesta:** `200 OK`.

*   **`DELETE /{torneoId}/inscripciones/{inscripcionId}`** (Auth, Admin/Creador)
    *   **Respuesta:** `204 No Content`.

*   **`POST /{id}/iniciar`** (Auth, Admin/Creador)
    *   **Respuesta:** `200 OK`. `400 Bad Request` si no hay suficientes equipos.

*   **`POST /partidos/{partidoId}/victoria`** (Auth, Admin/Creador)
    *   **DTO de Entrada:** `int` (ID del equipo ganador).
    *   **Respuesta:** `200 OK`.

### 6.6. `PartidosController` (`/api/partidos`)

*   **`GET /mispartidos`** (Auth)
    *   **Respuesta:** `200 OK` con `List<PartidoDto>`.

*   **`GET /{id}`** (Público)
    *   **Respuesta:** `200 OK` con un objeto anónimo (debería ser un `PartidoDetalleDto`).

*   **`PUT /{id}`** (Auth, Admin)
    *   **DTO de Entrada:** `CorregirPartidoDto`.
    *   **Respuesta:** `204 No Content`.

### 6.7. `AdminController` (`/api/admin`)

*   **`GET /users`** (Auth, Admin)
    *   **Respuesta:** `200 OK` con `List<{ Id, UserName, Email }>`.

*   **`GET /users/{id}`** (Auth, Admin)
    *   **Respuesta:** `200 OK` con `{ id, userName, email, roles: List<string> }`.

*   **`POST /assign-role`** (Auth, Admin)
    *   **DTO de Entrada:** `UpdateRoleDto`.
    *   **Respuesta:** `200 OK`.

*   **`DELETE /users/{id}/roles/{roleName}`** (Auth, Admin)
    *   **Respuesta:** `204 No Content`.

*   **`POST /juegos`** (Auth, Admin)
    *   **DTO de Entrada:** `CrearJuegoDto`.
    *   **Respuesta:** `201 Created` con la entidad `Juego` creada.

*   **`PUT /juegos/{id}`** (Auth, Admin)
    *   **DTO de Entrada:** `CrearJuegoDto`.
    *   **Respuesta:** `204 No Content`.

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
