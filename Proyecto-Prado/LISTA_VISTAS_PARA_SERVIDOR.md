# Lista completa de vistas para subir al servidor

Sube **toda la carpeta Views** manteniendo la misma estructura. En el servidor debe quedar: **/site1/Views/** con estas carpetas y archivos:

---

## Views (raíz)
- `_ViewStart.cshtml`
- `Web.config`

---

## Views/Account
- `ConfirmEmail.cshtml`
- `ExternalLoginConfirmation.cshtml`
- `ExternalLoginFailure.cshtml`
- `ForgotPassword.cshtml`
- `ForgotPasswordConfirmation.cshtml`
- `Login.cshtml`
- `Register.cshtml`
- `ResetPassword.cshtml`
- `ResetPasswordConfirmation.cshtml`
- `SendCode.cshtml`
- `VerifyCode.cshtml`
- `_ExternalLoginsListPartial.cshtml`

---

## Views/Admin
- `Configuraciones.cshtml`
- `ConfigurarHeroBanners.cshtml`
- `ConfigurarPreciosDepartamentos.cshtml`

---

## Views/Bitacora
- `IndexBitacoraEventos.cshtml`

---

## Views/Cargos
- `Index.cshtml`
- `Create.cshtml`
- `Edit.cshtml`
- `Details.cshtml`
- `Delete.cshtml`

---

## Views/CheckIn
- `Index.cshtml`
- `Create.cshtml`
- `ImprimirTarjeta.cshtml`
- `VerTarjeta.cshtml`

---

## Views/Citas
- `IndexCitas.cshtml`
- `Create.cshtml`
- `Edit.cshtml`
- `Details.cshtml`

---

## Views/Clientes
- `Index.cshtml`

---

## Views/Colaborador
- `IndexColaborador.cshtml`
- `Create.cshtml`
- `Edit.cshtml`

---

## Views/Departamento
- `Create.cshtml`
- `Edit.cshtml`
- `IndexDepartamentos.cshtml`
- `IndexDepartamentosClientes.cshtml`
- `HomeDepartamentos.cshtml`
- `Details.cshtml`
- `DetallesAdmin.cshtml`
- `AsignarCliente.cshtml`
- `EditarImagenes.cshtml`

---

## Views/Habitacion
- `IndexHabitaciones.cshtml`
- `IndexHabitacionesUsuario.cshtml`
- `Create.cshtml`
- `Edit.cshtml`
- `Details.cshtml`
- `habitacionesInfo.cshtml`
- `EditarImagenes.cshtml`
- `EstadoHabitaciones.cshtml`
- `CalendarioHabitaciones.cshtml`

---

## Views/Home
- `Index.cshtml`
- `About.cshtml`
- `Contact.cshtml`
- `Services.cshtml`
- `Entrar.cshtml`
- `Registro.cshtml`

---

## Views/Manage
- `Index.cshtml`
- `EditProfile.cshtml`
- `AddPhoneNumber.cshtml`
- `ChangePassword.cshtml`
- `ManageLogins.cshtml`
- `SetPassword.cshtml`
- `VerifyPhoneNumber.cshtml`

---

## Views/Mantenimiento
- `IndexMantenimiento.cshtml`
- `Create.cshtml`
- `Edit.cshtml`
- `Details.cshtml`

---

## Views/Reservas
- `ConfirmarReserva.cshtml`
- `Confirmacion.cshtml`
- `ReservasUsuario.cshtml`

---

## Views/ReservasA
- `IndexReservasA.cshtml`
- `Edit.cshtml`
- `ListaEspera.cshtml`
- `SolicitudesPendientes.cshtml`

---

## Views/Shared
- `_Layout.cshtml`
- `_LayoutSimple.cshtml`
- `_LoginPartial.cshtml`
- `Error.cshtml`
- `Lockout.cshtml`

---

## Views/SolicitudLimpieza
- `IndexSolicitudLimpieza.cshtml`
- `Create.cshtml`
- `Edit.cshtml`

---

## Views/Usuario
- `Index.cshtml`
- `Create.cshtml`
- `Edit.cshtml`
- `Delete.cshtml`

---

## Cómo subir

**Opción 1 (recomendada):** Después de publicar, sube **toda** la carpeta **Views** de PradoPubli a **/site1/Views/** en FileZilla (sobrescribir). Así te aseguras de que no falte ninguna.

**Opción 2:** Sube carpeta por carpeta (Account, Admin, Bitacora, Cargos, etc.) manteniendo la misma estructura dentro de /site1/Views/.

**Importante:** Incluye también **Views/Web.config** y **Views/_ViewStart.cshtml** en la raíz de Views.
