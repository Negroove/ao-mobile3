# Actividad Obligatoria Integradora N 1

Web API desarrollada en .NET 8 para la gestion de contactos.

## Caracteristicas

- Modelo de dominio `Contacto`
- Servicio en memoria `ContactoService`
- Endpoint minimal `GET /minimal/contactos`
- Endpoints con controller para consultar, crear, editar y eliminar contactos
- Autenticacion con JWT
- Autorizacion por rol `Admin`
- Swagger configurado para probar endpoints protegidos

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Swagger / OpenAPI
- JWT Bearer Authentication
- PasswordHasher

## Ejecucion

Desde la carpeta del proyecto:

```powershell
cd .\ContactosApi
dotnet run
```

Swagger queda disponible en:

- `http://localhost:5234/swagger`
- `https://localhost:7247/swagger`

## Credenciales de prueba

- Usuario: `admin`
- Password: `1234`

## Flujo para probar la API

1. Ejecutar `POST /api/auth/login`
2. Enviar este body:

```json
{
  "usuario": "admin",
  "password": "1234"
}
```

3. Copiar el token devuelto
4. Hacer click en `Authorize` en Swagger
5. Pegar el token con el formato:

```text
Bearer TU_TOKEN
```

6. Probar endpoints protegidos

## Endpoints principales

### Minimal API

- `GET /minimal/contactos`

### Auth

- `POST /api/auth/login`

### Contactos

- `GET /api/contacto/{id}`
- `POST /api/contacto/add`
- `PUT /api/contacto/edit/{id}`
- `DELETE /api/contacto/delete/{id}`

## Ejemplo de alta de contacto

```json
{
  "nombre": "Grace",
  "apellido": "Hopper",
  "telefono": "11-5555-0003",
  "email": "grace@contactos.com"
}
```

## Persistencia

Los contactos se almacenan en memoria dentro de `ContactoService`.

- si agregas o editas contactos mientras la aplicacion esta corriendo, los cambios se mantienen


## Estructura principal

- `ContactosApi/Program.cs`: configuracion general, JWT, Swagger y minimal APIs
- `ContactosApi/Controllers/ContactosController.cs`: endpoints con controller
- `ContactosApi/Services/ContactoService.cs`: logica de contactos en memoria
- `ContactosApi/Services/AuthService.cs`: validacion de credenciales
- `ContactosApi/Services/TokenService.cs`: generacion de JWT

## Nota

Para entregar en `.zip`, eliminar las carpetas `bin` y `obj`.
