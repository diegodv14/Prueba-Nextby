# Prueba-Nextby

## Proyecto Inventario - Microservicios en .NET 9

Este proyecto está desarrollado en .NET 9 y se compone de dos microservicios principales:

- **Productos**: manejo de productos disponibles.
- **Transacciones**: gestión de compras y ventas relacionadas a los productos.

Como base de datos se utiliza **PostgreSQL**.

## Requisitos

- .NET 9 SDK
- Docker
- Docker Compose

## Levantar el proyecto en ambiente local

1. Clona el repositorio y abre la solución en tu editor (por ejemplo, Visual Studio o VS Code).

2. Navega a la carpeta `configuration/`, donde se encuentra el `docker-compose.yml` para levantar la base de datos.

3. Ejecuta el siguiente comando dentro de esa carpeta:

```bash
docker-compose up -d
```

Este comando inicia el contenedor de PostgreSQL y automáticamente ejecuta un archivo `init.sql` que se encuentra en la misma carpeta. Asegúrate de no mover ese archivo para que la inicialización funcione correctamente.

4. Una vez levantada la base de datos, simplemente corre ambos microservicios desde la solución.

## Probar los endpoints

Cada microservicio expone su propia interfaz Swagger para facilitar el consumo de sus APIs. Solo navega al navegador con la ruta correspondiente:

- **Productos**: `http://localhost:puerto_productos/swagger/index.html`
- **Transacciones**: `http://localhost:puerto_transacciones/swagger/index.html`

Ahí puedes probar las operaciones CRUD de cada uno directamente desde Swagger UI.
