# PreClear — Backend

This document describes the PreClear backend (ASP.NET Core 8 Web API). It explains architecture, modules, running instructions, testing guidance (Swagger + curl examples), DB setup, JWT usage, and workflows.

---
## 1️⃣ Project Overview
What is PreClear backend
- PreClear is a shipment pre-clearance & compliance backend providing APIs to manage users, shipments, documents, AI compliance, approvals, invoices, payments (simulated), tags, audits, and a mock sync integration to ERP/WMS.

Purpose of each module (short)
- Auth: user registration, login, JWT issuance, role management.
- Users: CRUD for user accounts, roles, activation.
- Shipments: Create/edit shipments, lifecycle (draft → submitted → approved/rejected/hold/reopen).
- Documents: Upload, list, delete docs; file storage and validation.
- AI Compliance: Run product/document analysis (HS code, findings) and persist results.
- Document Validation (AI Extract): Extract fields from invoices and store results.
- Exceptions: Record and resolve shipment exceptions.
- Chat: Real-time chat per shipment (SignalR) and message history.
- Notifications: User notifications and read/unread state.
- Audit Logs: Record actions (actor, shipment, action, description, timestamp).
- Broker Assignment: Assign brokers to shipments and query assigned shipments.
- Invoice: Generate invoices (PDF), attach to shipments.
- Payment Simulation: Create/confirm payment intents (no real gateway).
- Dashboard Summary: Aggregated metrics for admin/broker/shipper.
- Sync: Fake ERP/WMS sync that inserts/updates shipments and logs syncs.

High-level architecture (text):

Controllers → Services → Interfaces → Repositories → EF Core (MySQL)

Example flow: `ShipmentController` → `ShipmentService` → `IShipmentRepository` → `PreclearDbContext` → MySQL

---

## 2️⃣ Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core (EF Core 8) with Pomelo MySQL provider
- MySQL (or compatible MariaDB)
- Swagger UI for API exploration
- SignalR for real-time chat
- JWT Authentication for protected endpoints
- Local file storage for uploads (configured in `appsettings.json`)

---

## 3️⃣ Folder Structure (key folders)

- `Controllers/` — Web API endpoints (one controller per module)
- `Services/` — Business logic implementations
- `Interfaces/` — Service & repository interfaces
- `Repositories/` — Data access (EF Core usage)
- `Models/` — EF entities and DTOs used by services
- `Data/` — `PreclearDbContext` and EF mappings
- `Migrations/` — EF Core migrations
- `Swagger/` — custom Swagger helpers
- `Helpers/`, `Utils/` — utility and helper classes
- `Program.cs` — DI registration, CORS, DB setup, startup wiring

Example:

```
backend/
├─ Controllers/
├─ Services/
├─ Interfaces/
├─ Repositories/
├─ Models/
├─ Data/PreclearDbContext.cs
├─ Migrations/
├─ Program.cs
└─ appsettings.json
```

---

## 4️⃣ How to Run the Backend

Prerequisites:
- .NET 8 SDK
- MySQL server
- Node/npm if you will build the frontend locally

Set up `appsettings.json` (copy from `appsettings.Development.json` if provided). At minimum configure `DefaultConnection` and `Jwt` secret section:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=preclear;User=root;Password=YourPassword;"
  },
  "Jwt": {
    "Secret": "a-very-long-secret-key-here"
  }
}
```

Commands (run from `backend/`):

```bash
dotnet restore
dotnet ef migrations add Initial    # scaffold if database empty
dotnet ef database update           # apply migrations
dotnet run --urls "http://localhost:5232"
```

Enable Swagger: Swagger is enabled by default in Development. Navigate to `http://localhost:5232/swagger` after the app starts.

If frontend dev server runs on a different origin, set `VITE_API_BASE` in the frontend `.env` to `http://localhost:5232`.

---

## 5️⃣ API Testing Guide (MOST IMPORTANT)

General tips:
- Use Swagger UI (`/swagger`) to explore endpoints and model schemas.
- Use Postman or curl for scripted testing.
- For protected endpoints include `Authorization: Bearer <token>` header.

Common curl examples (replace host/port as needed):

```bash
# Register
curl -X POST http://localhost:5232/api/auth/register -H "Content-Type: application/json" -d '{"email":"test@x.com","password":"Pass123!","firstName":"Test","lastName":"User"}'

# Login
curl -X POST http://localhost:5232/api/auth/login -H "Content-Type: application/json" -d '{"email":"test@x.com","password":"Pass123!"}'

# Use token for protected request
curl -H "Authorization: Bearer <TOKEN>" http://localhost:5232/api/users
```
Module-by-module testing steps (Swagger → example input → expected output):

### 5.1 Auth Module

- Sign up (POST `/api/auth/register`)
  - Input: { email, password, firstName, lastName }
  - Expected: 201 Created or 200 with newly created user id (no password returned).

- Login (POST `/api/auth/login`)
  - Input: { email, password }
  - Expected: 200 OK { token: "<jwt>", user: { id, email, role } }

- Test JWT-protected routes
  - Use returned token as `Authorization: Bearer <token>` to call `/api/users` or `/api/shipments`.
  - Expected: 200 OK for authorized users; 401 for missing/invalid token.

Error cases: invalid credentials → 400/401 with message.

### 5.2 Users Module

- Get all users (GET `/api/users`) — Expected: 200 list of users.
- Get user by ID (GET `/api/users/{id}`) — Expected: 200 user object or 404.
- Update role (PUT `/api/users/{id}/role`) — Body: { role: "admin" } — Expected: 200 updated user.
- Delete user (DELETE `/api/users/{id}`) — Expected: 204 No Content or 200.

### 5.3 Shipments Module

- Create shipment (POST `/api/shipments`) — Body sample:

```json
{
  "referenceId": "REF-1001",
  "shipmentName": "Test Shipment",
  "shipperId": 1,
  "consigneeId": 2,
  "totalValue": 1200.00,
  "currency": "USD"
}
```
- Expected: 201 Created with shipment object.
- Submit shipment (POST `/api/shipments/{id}/submit`) — Expected: 200 with status `submitted`.
- Approve/reject/hold/reopen (POST endpoints like `/api/shipments/{id}/approve`) — Expected: 200 with updated status.
- Get shipments of user (GET `/api/shipments?userId=1`) — Expected: 200 list filtered.

### 5.4 Documents Module

- Upload document (POST `/api/documents/upload`) — multipart/form-data with `file` and `shipmentId`.
  - Expected: 200 with stored document metadata and URL field.
  - Validate allowed extensions (e.g., .pdf, .jpg, .png). Uploading other extensions yields 400.

- List documents (GET `/api/documents?shipmentId={id}`) — Expected: 200 array.
- Delete document (DELETE `/api/documents/{id}`) — Expected: 204 or 200.

Curl example (file upload):

```bash
curl -F "file=@/path/to/invoice.pdf" -F "shipmentId=1" http://localhost:5232/api/documents/upload
```

### 5.5 AI Compliance Module

- Test product analysis (POST `/api/ai/analyze`) — sample input: { shipmentId, items }.
- Expected: 200 with predicted HS codes, risk findings stored in DB.
- Fetch compliance history (GET `/api/ai/shipments/{id}/history`) — Expected: array of analysis results.

### 5.6 Document Validation (AI Extract)

- Upload invoice (same document upload endpoint)
- Request extract-text (POST `/api/documents/extract`) with document id.
- Expected: 200 with extracted fields (invoice_number, total_amount, date). Store extraction in DB.

### 5.7 Exceptions Module

- Create exception (POST `/api/exceptions`) — body: { shipmentId, description, createdBy }
- Resolve exception (POST `/api/exceptions/{id}/resolve`) — Expected: 200 and exception marked resolved
- Fetch open exceptions (GET `/api/exceptions?status=open`) — Expected: array

### 5.8 Chat Module

- Send message (POST `/api/chat/{shipmentId}/messages`) — body: { userId, text }
- Fetch chat history (GET `/api/chat/{shipmentId}/messages`) — Expected: array ordered by createdAt
- Real-time: connect via SignalR hub `/hubs/chat` and receive messages.

### 5.9 Notifications Module

- Get notifications (GET `/api/notifications`) — Expected: 200 with notifications list
- Mark as read (POST `/api/notifications/{id}/read`) — Expected: 200

### 5.10 Invoice Module

- Generate invoice for shipment (POST `/api/invoices/generate` { shipmentId }) — Expected: 200 with `pdfUrl` and `invoiceId`.
- Download invoice (GET `/api/invoices/{id}/download`) — returns PDF stream.

### 5.11 Payment Simulation

- Create payment intent (POST `/api/payment/create-intent`)
  - Body: { userId, amount }
  - Expected: 200 { id, status: "requires_confirmation", amount }

- Confirm payment (POST `/api/payment/confirm`)
  - Body: { paymentId }
  - Expected: 200 { id, status: "succeeded" }

Curl example:

```bash
curl -X POST http://localhost:5232/api/payment/create-intent -H "Content-Type: application/json" -d '{"userId":1,"amount":25.50}'
curl -X POST http://localhost:5232/api/payment/confirm -H "Content-Type: application/json" -d '{"paymentId":123}'
```

### 5.12 Audit Log

- See activity for a shipment (GET `/api/audit/{shipmentId}`) — Expected: 200 list of audit logs
- See activity per user (GET `/api/audit/user/{userId}`) — Expected: 200 list

### 5.13 Broker Assignment

- Assign broker (POST `/api/broker/assign` { shipmentId, brokerId }) — Expected: 200
- Broker get assigned shipments (GET `/api/broker/{brokerId}/shipments`) — Expected: list

### 5.14 Tags Module

- Add tag (POST `/api/shipments/{id}/tags` { name }) — Expected: 200 created tag
- Remove tag (DELETE `/api/shipments/{id}/tags/{tagId}`) — Expected: 204
- List tags (GET `/api/shipments/{id}/tags`) — Expected: array

### 5.15 Dashboard Summary

- GET `/api/dashboard/summary` — Expected: { totalShipments, approvedShipments, pendingShipments, exceptionsCount }

### 5.16 Sync Module

- Run fake sync (POST `/api/sync/run`) — Expected: 200 { imported, updated, details }

---

## 6️⃣ Error Handling

- Validation errors: 400 Bad Request with error details JSON.
- Authentication: 401 Unauthorized when JWT missing/invalid.
- Authorization: 403 Forbidden when role insufficient.
- Not found: 404 Not Found when resource missing.
- Server errors: 500 Internal Server Error with message/log ID (avoid leaking stack traces in Prod).

Exceptions are typically returned as JSON: `{ "error": "message", "details": { ... } }`.

---

## 7️⃣ Database Schema (overview)

Main tables:
- `users`
- `shipments`
- `shipment_items`, `shipment_packages`, `shipment_parties`
- `shipment_documents`
- `ai_findings` / `shipment_compliance`
- `invoices`
- `payments`
- `audit_logs`
- `tags`
- `notifications`
- `sync_logs`

Small relationship diagram (text):

```
users 1---* shipments
shipments 1---* shipment_items
shipments 1---* shipment_documents
shipments 1---* invoices
shipments 1---* audit_logs
shipments 1---* tags
```

---

## 8️⃣ JWT Setup Explanation

- Secret key: configured in `appsettings.json` under `Jwt:Secret`.
- Tokens generated in `JwtTokenGenerator` helper (see `Helpers/JwtTokenGenerator.cs`).
- Test protected endpoints: login, copy token, then add HTTP header `Authorization: Bearer <token>`.

---

## 9️⃣ Sample API Workflow (E2E)

1. Register user: `POST /api/auth/register` → receives user id.
2. Login: `POST /api/auth/login` → receives JWT.
3. Create shipment: `POST /api/shipments` with Authorization header.
4. Upload documents: `POST /api/documents/upload` (multipart) with shipmentId.
5. Run AI compliance: `POST /api/ai/analyze` with shipmentId.
6. Broker approval: `POST /api/shipments/{id}/approve`.
7. Generate invoice: `POST /api/invoices/generate`.
8. Payment (simulation): `POST /api/payment/create-intent` -> `POST /api/payment/confirm`.

Each step returns JSON with IDs and status to use in the next step.

---

## 1️⃣0️⃣ FAQs

- Q: Migrations failing? A: Ensure `DefaultConnection` is set, DB exists, and credentials have schema creation rights. Run `dotnet ef migrations remove` to rollback a bad migration.
- Q: Login fails? A: Check `Jwt:Secret` is set and same as when tokens were issued. Verify the password hashing flow.
- Q: Swagger file upload not working? A: Use `multipart/form-data` in Swagger UI and ensure OperationFilter for file uploads is present (the project includes `FileUploadOperationFilter`).

---

## 1️⃣1️⃣ Contribution Guide (short)

- Branching: `main` protected; create feature branches `feature/<short-desc>`.
- Commit messages: `type(scope): message` e.g., `feat(payment): add payment simulation`.
- PRs: include testing steps and verify migrations.

## 1️⃣2️⃣ License

This repository is provided for educational purposes. Add a license file if needed (e.g., MIT).

---

If you want, I can also:
- Generate a Postman collection for these endpoints,
- Add a minimal `backend/README_quickstart.md` with copy-paste commands,
- Or update the repo root `README.md` to link to this backend README.

---

File location: `backend/README.md`
