# API de Gestión de Clubes, Dirigentes y Socios

Esta API permite realizar operaciones CRUD sobre clubes, dirigentes y socios. Está desarrollada en C# con SQL Server y expuesta mediante Swagger.

## Tecnologías utilizadas
- ASP.NET Core
- SQL Server
- Swagger
- Visual Studio

## Endpoints disponibles

### Club
- GET /club
- GET /club/{id}
- POST /club
- PUT /club/{id}
- DELETE /club/{id}

### Dirigente
- GET /dirigente
- GET /dirigente/{id}
- POST /dirigente
- PUT /dirigente/{id}
- DELETE /dirigente/{id}

### Socio
- GET /socio
- GET /socio/{id}
- POST /socio
- PUT /socio/{id}
- DELETE /socio/{id}

## Validaciones implementadas
- Fecha de nacimiento no puede ser futura
- DNI debe ser mayor a cero
- Campos obligatorios como nombre, apellido y rol

## Cómo ejecutar el proyecto
1. Clonar el repositorio
2. Abrir en Visual Studio
3. Configurar la cadena de conexión en `appsettings.json`
4. Ejecutar con F5 y abrir Swagger

## Autor
Acosta Karina,
Pedrozo Marcos,
Sanchez Belen — Trabajo Practico de API REST