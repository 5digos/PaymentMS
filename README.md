
# Descripción del flujo completo del pago de la reserva:

## Contexto general

Este microservicio (MS) permite realizar el pago a través de Mercado Pago de reservas consumidas del MS Reservation, incluyendo el cálculo de recargos por devolución de auto tardía. Se integra un microservicio de reservas con un microservicio de pagos, utilizando llamadas HTTP entre ellos y la API de Mercado Pago CheckoutPro para la gestión 'real' del pago.

1. Creación de la preferencia de pago (CheckoutPro de Mercado Pago):

Cuando se obtiene una reserva, el MS de pagos la consume desde el MS de reservas, y debe generar un link de pago en Mercado Pago.
Por lo que el servicio MercadoPagoService crea una preferencia con los datos del pago:
	
	- Título y monto base.
	- Una propiedad ExternalReference serializada en JSON que contiene:
	- PaymentId (identificador local del pago).
	- LateFee (posible recargo por devolución tardía).
	- URLs de retorno (BackUrls) que Mercado Pago usará para notificar el estado del pago (éxito, fallo o pendiente).

Y el Mercado Pago devuelve una URL para redirigir al usuario al checkout, pudiendo ingresar al link con una cuenta de MP de prueba para poder simular el pago.

-------------

2. Cálculo del monto total y recargos:

El PaymentCalculationService recibe un resumen de la reserva (ReservationSummaryResponse) y en base a ese Response, se calcula el monto total a pagar con base en:

	La tarifa por hora (HourlyRateSnapshot).
	El tiempo real de uso, considerando si la devolución fue tardía (ActualReturnTime).

Devuelviendo dos valores:

	TotalAmount: total calculado.
	LateFee: recargo si hubo devolución fuera de horario (devolución tardía).

-------------

3. Confirmación del pago:
Cuando Mercado Pago notifica un cambio en el estado del pago (por ejemplo, que el pago fue aprobado), ejecuta una llamada HTTP (BackUrl) a la API interna en el endpoint (/api/payment/pago-exitoso), llamando internamente al método VerifyPayment que:

	-Consulta a Mercado Pago el estado actual del pago usando el paymentId.
	-Deserializa el campo ExternalReference para obtener el PaymentId local y el LateFee.
	-Obtiene la información del pago almacenado en la base de datos local.
	-Si el pago no está previamente procesado (no está aprobado ni rechazado), actualiza su estado según el estado actual de Mercado Pago (approved, rejected, cancelled).

	En caso de aprobación:

		Actualiza el estado del pago a "Aprobado".
		Notifica al microservicio de reservas que el pago fue confirmado, enviando detalles como el monto total, recargo por demora, pasarela de pago e ID de transacción (PaymentConfirmationRequest).
		El microservicio de reservas actualiza internamente el estado de la reserva (siendo fuera de este MS de pagos)

-------------

4. Comunicación entre microservicios:

El microservicio de pagos utiliza el 'ReservationServiceClient' mencionado para comunicarse con el microservicio de reservas.
Para confirmar un pago, el servicio de pagos hace una llamada HTTP POST a:

	POST /api/reservations/{reservationId}/payment		

→ enviando un objeto de tipo 'PaymentConfirmationRequest' con:
	
	TotalAmount
	LateFee
	PaymentGateway (en este caso, "MercadoPago")
	TransactionId (de Mercado Pago)

-------------

5. Flujo resumido:

	Se consume una reserva desde el MS pagos y se calcula monto + recargos.
	Se crea preferencia de pago en Mercado Pago con la referencia serializada.
	Usuario completa el pago en Mercado Pago.
	Mercado Pago llama al BackUrl /api/payment/pago-exitoso.
	El endpoint interno verifica el pago consultando la API de Mercado Pago.
	Actualiza estado local y notifica al microservicio de reservas.
	El microservicio de reservas recibe la notificación y continua con el flujo.

-------------

# CONFIGURACION: 
# Paso 1: Configuración del Entorno

## 1.1 Configurar Access Token de Mercado Pago

a. **Obtener credenciales de Mercado Pago**:
	- Regístrate o inicia sesión en [Mercado Pago Developers](https://www.mercadopago.com.ar/developers)
	- Con la sesión iniciada, accedé a 'Tus integraciones'. Allí, haz clic en Crear aplicación y creala siguiendo los pasos.
	- Ahora creá 2 cuentas de prueba: una de COMPRADOR para pagar y otra de VENDEDOR para recibir el pago.
  - Ingresa a la cuenta de Vendedor y creá nuevamente una aplicacion con esa cuenta.
	- Anda a la sección "Credenciales de producción" para obtener el ACCESS TOKEN.
  *(a la cuenta de Comprador vas a ingresar cuando tengas que efectur simulaciones de pago)*
	
Con el `Access Token` de la cuenta de VENDEDOR:

b. **Configuralo en appsettings.json**:
   ```json
   {
     "MercadoPago": {
       "AccessToken": "TEST-0000000000000000-000000-00000000000000000000000000000000-000000000"
     }
   }
   ```

## 1.2 Configurar URLs de Callback y Notificaciones

a. Usar la herramienta - ngrok - para exponer tu localhost, y obtener la url provista por - ngrok - y proceder a setearla en appsettings.json

b. **Setear la url en appsettings.json**:
   ```json
   {
     "MercadoPago": {
       "BackUrlBase": "https://url.ngrok-free.app"
     }
   }
  ```
Así las **URLs de Callback** se van a configurar automáticamente (serán utilizadas por Mercado Pago para redireccionar al usuario):
   - **Success**: URL a la que el usuario será redirigido después de un pago exitoso
   - **Failure**: URL a la que el usuario será redirigido después de un pago fallido
   - **Pending**: URL a la que el usuario será redirigido si el pago está pendiente


## 1.3: Revisar y configurar los BaseUrl de HttpClient:

a. Entrar en appsettings.json, y revisar que el port coincida con la dirección de Reservation.

```json
 "ReservationService": {
     "BaseUrl": "https://localhost:port"
}
```

# Paso 2: Ejecutar el Microservicio MS 

1. **En Visual Studio**:

		Establece el proyecto PaymentMS como proyecto de inicio
		Presiona F5 o haz clic en "Iniciar depuración"


-------------

# Prueba completa de flujo:

1° Debe consumir una Reserva del MSReservas pasando el ID de la reserva como parametro en ( GET /api/Payment/reservation/{id}), obteniendo un Response del siguiente estilo :

```json
{
  "reservationId": "a1e2b3c4-d5f6-7890-ab12-34567890cd01",
  "userId": 101,
  "vehicleId": "b2c3d4e5-f6a7-8901-bc23-45678901de01",
  "startTime": "2025-05-31T14:00:00Z",
  "endTime": "2025-05-31T16:00:00Z",
  "actualPickupTime": "2025-05-31T14:05:00Z",
  "actualReturnTime": "2025-05-31T15:55:00Z",
  "hourlyRateSnapshot": 501
}
```

2°: Usar este response para crear un pago a partir de esa reserva en ( POST /api/Payment/from-reservation) enviando como requestBody al response obtenido en el paso anterior.
Y se va a obtener de este POST una URL del CheckoutPro de MP, y el PaymentID del pago persistido en la Base de Datos:

```json
{
  "checkoutUrl": "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=2286099976-220d6295-1111-1111-1111-44441f079f76",
  "paymentId": "9943b049-6180-4b19-bc43-2aa96c514364"
}
```

3°: Se debe ingresar a la CUENTA COMPRADOR DE PRUEBA de MP, para que, con la URL obtenida, se pueda proceder a realizar el pago.

4°: Con la sesión de la cuenta de comprador, copiar el 'checkoutUrl' y pegarlo en el navegador web, y realizar el pago (simulado).

5°: Cuando se realice el pago, pasado 4 segundos se va a redirigir automáticamente hacia:

	POST /api/Payment/verify/{mercadoPagoPaymentId}
 
gracias al endpoint: 

	GET: /api/payment/pago-exitoso
 
 que redirecciona el URL de Callback de MP. 

6°: Y se va a mostrar en el navegador la información relevante sobre el pago que se realizó.

7°: Paralelamente, cuando se ejecuta:

	POST /api/Payment/verify/{mercadoPagoPaymentId} 
 
 , al efectuarse correctamente el pago, se realiza la notificacion hacia el MS Reservas, enviándole un PaymentConfirmationRequest con la info relevante del pago.

8°: Y asi se cierra el flujo sobre el MS Pagos.

9°: Adicionalmente se tiene un endpoint con el cual se puede obtener toda la info de un Pago persistido en DB

--------------
--------------
