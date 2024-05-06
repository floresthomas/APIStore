API Store

Esta API ASP.NET Core ayuda a manejar la gestión de órdenes, productos y usuarios, incluyendo autenticación y autorización. Además de las operaciones básicas de CRUD (Crear, Leer, Actualizar, Eliminar), la API ofrece las siguientes características clave:

Autenticación y Autorización: 
- Implementé un sistema de autenticación utilizando tokens de acceso JWT (JSON Web Token).
- Verificación por Correo Electrónico: Incluye un proceso de verificación por correo electrónico para validar las cuentas de usuario. Después del registro, se envía un correo electrónico de verificación con un enlace único que el usuario debe hacer clic para confirmar su dirección de correo electrónico antes de poder iniciar sesión.
- Gestión de Refresh Tokens: La API implementa un mecanismo de refresh token para permitir a los clientes solicitar un nuevo token de acceso sin tener que volver a ingresar las credenciales de inicio de sesión.
Integración con SQL Server:
- La API utiliza una base de datos SQL Server para almacenar la información de órdenes, productos y usuarios. 
- Utilicé Entity Framework Core para interactuar con la base de datos y realizar operaciones de lectura y escritura.

### Requisitos Previos

Antes de levantar el proyecto, asegúrate de tener instalado lo siguiente:

- [.NET Core SDK](https://dotnet.microsoft.com/download) (versión 8.0.202 o superior)
- [Visual Studio](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)

Configuración del Proyecto

Clona el repositorio en tu máquina local, usando el comando:
```json
git clone https://github.com/tu-usuario/tu-proyecto.git
```
Abre el proyecto en el IDE que hayas decidido instalar (por ejemplo, Visual Studio o Visual Studio Code).
Asegúrate de restaurar las dependencias del proyecto ejecutando el siguiente comando en la raíz del proyecto:
dotnet restore

### Ejecutar el Proyecto

Para ejecutar el proyecto, sigue estos pasos:

Abre una terminal en la raíz del proyecto.

Ejecuta el siguiente comando para compilar y ejecutar la aplicación:
```json
dotnet run
```
La aplicación estará disponible en el puerto http://localhost:5278

Para acceder a la documentación de la API a través de swagger, visita esta direccion http://localhost:5278/swagger

Uso

### Registro de Usuario

Para registrar un nuevo usuario, envía una solicitud POST al endpoint `/register` con el siguiente cuerpo:

```json
{
  "email": "tu@email.com",
  "userName": "nombre_de_usuario",
  "password": "tu_contraseña"
}
```
Este deberia ser el resultado
```json
{
  "result": true,
  "refreshToken": null,
  "token": null,
  "errors": null
}
```
Después de registrarse, se enviará un correo electrónico de verificación. El usuario debe hacer clic en el enlace de verificación para poder activar su cuenta.

### Inicio de Sesión

Para iniciar sesión, envía una solicitud POST al endpoint /login con el siguiente cuerpo:
```json
{
  "email": "tu@email.com",
  "password": "tu_contraseña"
}
```
La respuesta mostrará un token de acceso y un token de refresco:
```json
{
  "result": true,
  "refreshToken": "#.Fmi-rmfHjQQWiet-uynMN",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ..."
}
```

### Autorización en Swagger

Para autorizar tus solicitudes en Swagger, apreta clic en el botón "Authorize" y agrega el token de acceso obtenido después de iniciar sesión como un encabezado de autorización con el formato Bearer. Por ejemplo:

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ...

Una vez que hayas autorizado tu solicitud, podrás acceder a los otros endpoints protegidos de la API.
