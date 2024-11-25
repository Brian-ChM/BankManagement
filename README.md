# Instalación

###### 1. Clonar el repositorio

```bash
git clone https://github.com/Brian-ChM/BankManagement.git
cd BankManagement
```

###### 2. Crear el contenedor de Docker o usar una base de datos PostgreSQL

```bash
docker run --name postgres -e POSTGRES_PASSWORD=123456 -d -p 5432:5432 postgres
```

###### 3. Actualizar la base de datos

```bash
dotnet ef database update -p .\Infrastructure\ -s .\BankManagement\
```

###### 4. Ejecutar la API

```bash
// Presiona el botón para ejecutar y depurar, o ingresa el siguiente comando
dotnet run --project .\BankManagement\
```

---

# Uso

###### `[POST] bank/{ id }/get-token`

**Recibe:** `Id: int`

**Descripción:**  
El 'Id' recibido debe corresponder a un cliente. Con este identificador se genera un token, y dependiendo del rol asociado al cliente, se pueden acceder a otros endpoints, como aprobar o rechazar un préstamo.

**Devuelve:** `string` (El token generado)

---

###### `[POST] bank/loan/simulate`

**Recibe:**

```json
{
  "amount": int,  // Monto solicitado
  "month": int    // Cantidad de cuotas solicitadas 
}
```

**Descripción:**  
Simula un préstamo basado en los datos proporcionados. El interés puede variar dependiendo de la cantidad de cuotas seleccionadas.

**Devuelve:**

```json
{
  "monthlyPaid": int,     // Monto mensual a pagar.
  "totalPaid": int,       // Monto total a pagar al finalizar el préstamo.
  "interestRate": decimal // Tasa de interés anual aplicada al préstamo.
}
```

---

###### `[POST] bank/loan/request`

**Recibe:**

```json
{
  "customerId": int,      // ID del cliente solicitante.
  "loanType": "string",   // Tipo de préstamo: "personal", "vivienda", "automotriz".
  "amountRequest": int,   // Monto solicitado.
  "monthRequest": int     // Número de meses solicitados para el préstamo.
}
```

**Descripción:**  
Genera una solicitud de préstamo para un cliente específico. El estado inicial de la solicitud es "pending" (pendiente) y debe ser aprobada o rechazada por un usuario con rol de 'admin'.

**Devuelve:**

```json
{
  "message": "Solicitud lista, estado Pending"
}
```

---

###### `[GET] bank/loan/{ id }`

**Recibe:** `Id: int`

**Descripción:**  
Obtiene todos los detalles de un préstamo aprobado, identificado por el 'Id'. La información incluye el monto solicitado, las cuotas pagadas y pendientes, y el estado general del préstamo.

**Devuelve:**

```json
{
  "customer": {
    "id": int,            // ID del cliente
    "name": "string"      // Nombre del cliente
  },
  "approveDate": "01/01/2000",    // Fecha de aprobación del préstamo
  "amountRequest": int,           // Monto solicitado
  "totalPaid": decimal,           // Total pagado hasta la fecha
  "earnedProfit": decimal,        // Ganancia generada por el préstamo
  "months": int,                  // Número de meses del préstamo
  "loanType": "string",           // Tipo de préstamo: "personal", "vivienda", "automotriz"
  "interest": decimal,            // Tasa de interés aplicada
  "duesPaid": int,                // Cuotas pagadas
  "pendingInstallments": int,     // Cuotas pendientes
  "nextExpirationDate": "01/02/2000" // Fecha de vencimiento de la próxima cuota
}
```

---

###### `[GET] bank/loan/{ id }/installments`

**Recibe:** `Id: int, Status: string (opcional)`

**Descripción:**  
Obtiene una lista de todas las cuotas asociadas a un préstamo identificado por su 'Id'. Además, se puede aplicar un filtro por estado de cuota de forma opcional.

**Devuelve:**

```json
[
  {
    "customer": {
      "id": int,          // ID del cliente
      "name": "string"    // Nombre del cliente
    },
    "totalAmount": decimal,  // Monto total de la cuota
    "dueDate": "01/01/2000", // Fecha de vencimiento de la cuota
    "status": "string"       // Estado de la cuota (por ejemplo: "paid", "pending")
  },
  ...
]
```

---

###### `[POST] bank/installments/payment`

**Recibe:**

```json
{
  "id": int,      // ID del préstamo
  "amount": int   // Monto a pagar
}
```

**Descripción:**  
Realiza el pago de una o varias cuotas de un préstamo, identificadas por su 'id'. El estado de la primera cuota vencida o próxima a vencer se actualiza, y se agrega un nuevo registro en la tabla de pagos con la fecha en que se realizó el pago.

**Devuelve:**

```json
{
  "message": "Pago de {cantidad: int} cuotas, quedan {cantidad: int} cuotas pendientes"
}
```

---

###### `[GET] bank/installments/overdue`

**Descripción:**  
Obtiene todas las cuotas que están pendientes de pago y cuya fecha de vencimiento ya ha pasado.

**Devuelve:**

```json
[
  {
    "customer": {
      "id": int,          // ID del cliente
      "name": "string"    // Nombre del cliente
    },
    "dueDate": "01/01/2000",  // Fecha de vencimiento de la cuota
    "daysLate": "Cuenta con {cantidad: int} días de atraso.",  // Días de atraso
    "amountPending": decimal // Monto pendiente de pago
  },
  ...
]
```

---

###### `[GET] bank/loan/request/{ id }/approve`

**Recibe:** `Id: int`

**Descripción:**  
Este endpoint requiere un token que se puede obtener a través de `bank/{ id }/get-token`. Una vez autorizado, la solicitud de préstamo pasa del estado "pending" a "approved". Se agrega un nuevo registro en la tabla de préstamos con los datos correspondientes y se generan las cuotas para este préstamo, cada una con estado "pending".

**Devuelve:**

```json
{
  "customerId": int,         // ID del cliente
  "approvedDate": "01/01/2000",  // Fecha de aprobación del préstamo
  "amount": int,             // Monto aprobado
  "months": int,             // Duración del préstamo en meses
  "interest": decimal,      // Tasa de interés aplicada
  "loanType": "string"      // Tipo de préstamo (por ejemplo: "personal", "vivienda", "automotriz")
}
```

---

###### `[POST] bank/loan/request/reject`

**Recibe:**

```json
{
  "id": int,         // ID del préstamo
  "reason": "string" // Razón del rechazo
}
```

**Descripción:**  
Este endpoint cambia el estado de una solicitud de préstamo de "pending" a "rejected", si no ha sido previamente aprobada o rechazada. No se agrega un nuevo registro en la tabla de préstamos, solo se actualiza el estado.

**Devuelve:**

```json
{
  "id": int,           // ID del préstamo
  "status": "string",   // Estado actualizado (por ejemplo: "rejected")
  "rejectionReason": "string" // Razón del rechazo
}
```

---
