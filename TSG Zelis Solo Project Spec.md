# Training Cohort Capstone – **Communication Experience Lifecycle App**

> **Mission:** Build a Communication Lifecycle application that lets payers (and internal CS reps) track the status of documents such as EOBs, EOPs, and ID cards—from creation all the way to delivery.

---

## 1 🎯 Learning goals

| Area | Why it matters |
| --- | --- |
| **Domain modeling** | Express a shared status taxonomy that can flex per communication type. |
| **Event-driven architecture** | Consume RabbitMQ events to update status in near-real time. |
| **Blazor Server** | Build responsive, component-based UIs with real-time SignalR updates (optional). |
| **Secure Web APIs** | Protect every endpoint with Okta, enforce role-based access. |
| **Testing** | All business logic must be unit tested and those tests passing. |
| **Cloud fundamentals** | Optionally deploy the whole stack to Azure (App Service + Azure SQL). |

---

## 2 🧱 Tech stack (baseline)

| Tier | Technology |
| --- | --- |
| Front end | **Blazor Server** (.NET 8) |
| API layer | ASP.NET Core Web API |
| Messaging | **RabbitMQ** (local container) |
| AuthN / AuthZ | **Okta** OIDC; Admin role for privileged pages |
| Data | **SQL Server** (EF Core code-first) |
| DevOps | Docker Compose |

---

## 3 📑 Core requirements & screens

| # | Screen | Must-have behaviours |
| --- | --- | --- |
| 1 | **Communications List** |  • Paginated grid of EOBs, EOPs, ID Cards  • Columns: Title, Type, Current Status, _Last Updated_, etc.  • “View Details” button navigates to Details page |
| 2 | **Communication Details** |  • Header info + full audit trail of status changes (timeline/stepper)  • Links to source file (mock URL) if applicable |
| 3 | **Event Simulator** |  • Dropdown to pick an existing Communication  • Dropdown to pick an **event** (e.g., `IdCardPrinted`)  • “Publish” posts the event to RabbitMQ and immediately reflects new status in the UI |
| 4 | **Status Taxonomy Admin** (admins only) |  • CRUD a **communication type** (e.g., `ID_CARD`)  • Check-boxes to mark which global statuses are valid for that type  • Per-type description for each status |
| 5 | **Add / Edit Communication Type** |  • Form lives under Admin section; only visible with `Admin` role claim |
| 6 | **Add Communications** | A re-usable UI that allows you to create new communications that will show up in the communications list. Even though different communication types may have slightly different fields, we want to re-use the same data structure for all commnunications. |

---

## 4 🔖 Reference status taxonomy

Below is a **starter set** of global statuses gathered from Zelis’ Lifecycle discovery work:

| Phase | Status code | Notes |
| --- | --- | --- |
| Creation | `ReadyForRelease`, `Released` | |
| Production | `QueuedForPrinting`, `Printed`, `Inserted`, `WarehouseReady` | |
| Logistics | `Shipped`, `InTransit`, `Delivered`, `Returned` | |


Add others as you see fit: `Failed`, `Cancelled`, `Expired`, `Archived`, etc.

*Each communication type (EOB, EOP, ID Card, …) should map to a _subset_ of these global statuses; enforce this in the admin UI and API validation layer.*

---

## 5 ⚙️ Domain & data hints

### SQL tables (suggested)

* `Communication` – `Id`, `Title`, `TypeCode`, `CurrentStatus`, `LastUpdatedUtc`, …
* `CommunicationStatusHistory` – `Id`, `CommunicationId`, `StatusCode`, `OccurredUtc`
* `CommunicationType` – `TypeCode`, `DisplayName`
* `CommunicationTypeStatus` – `(TypeCode, StatusCode, Description)`

### Event contract (JSON)

```jsonc
IdCardPrinted
{
  "communicationId": "74823" (or generated a GUID),
  "timestampUtc": "2025-07-28T14:07:22Z"
}
```

## Nice-to-haves
- host in azure
- localization - support multiple languages
- signalR (.net's implementation of websockets) - realtime updates on your list and details screens when status update or new communications are added.
- access data via GraphQL
- show status history in details
- filterable comms list (for example, able to filter to all EOBs)

## If-you-dares
- use mediatr to implement the mediator pattern for your backend logic.
- use azure event grid instead of rabbitMQ (requires azure)
