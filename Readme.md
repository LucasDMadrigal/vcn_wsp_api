# 💬 Chat API

Proyecto backend desarrollado en **.NET 8** que expone una API REST y un WebSocket con **SignalR** para gestionar conversaciones y mensajes en tiempo real.  
Utiliza **MongoDB** como base de datos y está preparado para integrarse con **Meta Webhooks**.

---

## 🏗️ Arquitectura General

La solución está organizada siguiendo una arquitectura en capas con separación de responsabilidades:

- **Chat.Api**  
  - Proyecto de entrada (ASP.NET Core Web API).  
  - Expone los **endpoints REST** y los **webhooks**.  
  - Maneja la comunicación en tiempo real con clientes vía **SignalR**.  
  - Configura los servicios, dependencias y middlewares.  

- **Chat.Domain**  
  - Contiene las **entidades del negocio** (`Message`, `Conversation`, etc).  
  - Define **enums** y lógica de dominio simple (`MessageStatus`).  
  - No tiene dependencias externas.  

- **Chat.Shared**  
  - Define los **DTOs (Data Transfer Objects)** usados entre las capas y para comunicación con los clientes.  
  - Estandariza la información que viaja en requests/responses.  

- **Chat.Data**  
  - Contiene la capa de acceso a datos (Repositories).  
  - Define las interfaces (`IMessageRepository`) y sus implementaciones (`MessageRepository`).  
  - Gestiona la conexión con **MongoDB**.  

---

## 📂 Estructura de Directorios

ChatSolution/
│
├── Chat.Api/ # Proyecto principal (entry point)
│ ├── Controllers/ # Controladores REST (MessagesController, WebhookController)
│ ├── Hubs/ # Hubs de SignalR (ChatHub)
│ ├── Program.cs # Configuración principal
│ └── appsettings.json # Configuración de entorno
│
├── Chat.Domain/ # Entidades y lógica de dominio
│ ├── Entities/ # Message, Conversation
│ └── Enums/ # MessageStatus
│
├── Chat.Shared/ # DTOs y objetos compartidos
│ └── DTOs/ # MessageDto, MessageStatusDto
│
└── Chat.Data/ # Acceso a datos
├── Repositories/ # Interfaces de Repositorios
└── ImplRepositories/ # Implementaciones con MongoDB

---


---

## 📦 Dependencias

- **ASP.NET Core Web API** – Framework principal.  
- **SignalR** – Comunicación en tiempo real.  
  - `Microsoft.AspNetCore.SignalR`  
- **MongoDB** – Persistencia de mensajes.  
  - `MongoDB.Driver`  
- **Swagger / Swashbuckle.AspNetCore** – Documentación y pruebas de endpoints.  
- **Microsoft.Extensions.Options** – Inyección de configuraciones (`MongoDbSettings`).  
- **Microsoft.Extensions.Logging** – Logging centralizado.  

---

## 📨 Endpoints Principales

### 🔹 MessagesController
- `GET /api/messages/{conversationId}` → Obtiene mensajes de una conversación.  
- `POST /api/messages/send` → Envía un mensaje, lo guarda en Mongo y lo emite por SignalR.  

### 🔹 WebhookController
- `GET /api/webhook` → Verificación de webhook (modo `subscribe`).  
- `POST /api/webhook` → Recepción de eventos entrantes desde Meta (mensajes, estados).  

---

## 🛠️ Flujo de Datos

1. **Webhook Entrante**  
   Meta → `WebhookController.Receive` → Parseo → Guardado en Mongo (`MessageRepository`) → Broadcast a clientes vía SignalR (`ChatHub`).  

2. **Envió desde API**  
   Cliente → `MessagesController.Send` → Guardado en Mongo → Broadcast por SignalR.  

3. **Lectura de mensajes**  
   Cliente → `MessagesController.GetMessagesByConversationId` → Respuesta desde Mongo.  

---

## ⚙️ Configuración

Archivo `appsettings.Development.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ChatAppDb"
  },
  "WebhookVerificationToken": "mi_token_por_defecto",
  "Meta": {
    "AppSecret": "tu_app_secret"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}


```

---

🚀 Cómo Ejecutar el Proyecto

Clonar el repositorio.

Levantar MongoDB (local o en cluster Atlas).

Configurar appsettings.Development.json con la cadena de conexión.

Compilar y ejecutar el proyecto Chat.Api.

Probar en:

Swagger UI → https://localhost:{puerto}/swagger

Postman para pruebas manuales.

Cliente SignalR conectado a /chathub.

📌 Alcance Actual

✅ Persistencia de mensajes en MongoDB.
✅ Recepción de webhooks de Meta (mensajes y estados).
✅ Emisión de eventos en tiempo real a clientes vía SignalR.
✅ Endpoints básicos para enviar y listar mensajes.
✅ Documentación interactiva con Swagger.

🔜 Próximos pasos

Manejo avanzado de usuarios/conversaciones.

Autenticación y autorización.

Métricas y monitorización.

Tests unitarios e integración.