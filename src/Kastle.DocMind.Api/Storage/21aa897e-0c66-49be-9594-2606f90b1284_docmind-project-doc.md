# DocMind — Internal Documentation Assistant

**A local, AI-powered "chat with our docs" application built on .NET 8**

---

## 1. Project Description

DocMind is a documentation assistant that lets users ask natural-language questions about internal documentation and receive accurate, cited answers. Users upload documents (Markdown, text, PDF), the system processes and indexes them, and a chat interface answers questions grounded strictly in that content — pointing back to the exact source of every answer.

The system implements the **RAG (Retrieval-Augmented Generation)** pattern, the industry-standard architecture for building AI assistants over private data:

1. **Ingest** — documents are parsed, split into chunks, and converted into vector embeddings.
2. **Retrieve** — when a question comes in, the most relevant chunks are found via vector similarity search.
3. **Generate** — a language model composes an answer using only those retrieved chunks, with citations.

Everything runs **entirely locally** — no cloud AI services, no API costs, and no data leaving the machine.

### Technology Stack

| Layer | Technology |
|---|---|
| Backend API | .NET 8, ASP.NET Core |
| AI runtime | Ollama (local) — llama3.1:8b for chat, nomic-embed-text for embeddings |
| AI abstraction | Microsoft.Extensions.AI (`IChatClient`, `IEmbeddingGenerator`) |
| Vector database | Qdrant (Docker) |
| Metadata database | MongoDB or SQL Server |
| Frontend | Blazor |
| Packaging | Docker Compose |

### End Goal

At the end of the project, anyone should be able to clone the repository, run `docker compose up`, open a browser, upload documentation, and ask questions like *"How does X work in our system?"* — receiving an accurate answer with citations, generated completely on the local machine.

---

## 2. Expectations from the Project

### What the project should deliver

- A working end-to-end application: document upload → indexing → question answering with citations.
- Clean, layered, testable code following standard .NET practices (dependency injection, separation of concerns, meaningful unit and integration tests).
- All AI interactions behind provider-agnostic interfaces, so the local model could later be swapped for a cloud provider by changing configuration only.
- A measurable approach to AI quality: an evaluation set of questions and a harness that scores retrieval and answer quality — improvements should be demonstrated with numbers, not impressions.
- One-command startup via Docker Compose, with a README that a new developer can follow cold.
- A final design document (architecture, data model, key decisions and their reasoning) and a demo presentation.

### Ways of working

- **Version control discipline:** small, focused commits; feature branches; every change goes through a pull request.
- **Regular communication:** brief status updates, and raising blockers early — spending a reasonable time investigating independently first, then asking with context ("here's what I tried").
- **Design before code for anything non-trivial:** a short written note or diagram before building each major component; key decisions are discussed before implementation.
- **Quality is part of the task:** code isn't "done" without error handling, logging, and tests. Demos should run from a clean state.
- **Curiosity is encouraged:** experimenting with models, chunk sizes, and prompts is part of the work — measured experiments are the point of running everything locally at zero cost.

### How success is evaluated

| Area | Weight | What "great" looks like |
|---|---|---|
| Working software | 30% | End-to-end flow works, starts with one command, handles errors gracefully |
| Code quality | 20% | Clean layering, provider-agnostic AI interfaces, meaningful tests, focused PRs |
| AI engineering | 20% | Data-driven choices on models/chunking/retrieval; grounded, cited answers |
| Ownership & process | 15% | Consistent progress, independent investigation, clear updates |
| Communication & docs | 15% | Design doc quality, README, final demo clarity |

---

## 3. Phase 1 — Document Management API (no AI yet)

**Duration:** ~2 weeks (after initial environment setup)
**Goal:** Build the foundation — a clean, working document management API with real storage and tests. No AI is involved in this phase; this guarantees a solid, shippable base before any model integration begins.

### Prerequisites (complete before starting)

- .NET 8 SDK, Docker Desktop, Git, and an IDE (Visual Studio or VS Code) installed and working.
- Ollama installed with `llama3.1:8b` and `nomic-embed-text` pulled, verified with a test request to `localhost:11434` (used from Phase 2 onward, but confirm early that the machine can run it).
- Solution skeleton created with this structure:

```
Kastle.DocMind.sln
├── src/
│   ├── Kastle.DocMind.Api             → ASP.NET Core Web API (endpoints, middleware, Swagger)
│   ├── Kastle.DocMind.Domain          → entities, domain models, service/repository interfaces
│   ├── Kastle.DocMind.Business        → business logic and service implementations (validation, orchestration)
│   └── Kastle.DocMind.Infrastructure  → database access, file storage, external integrations
└── tests/
    └── Kastle.DocMind.Tests           → unit and integration tests
```

Dependency direction: `Api → Business → Domain`, with `Infrastructure` implementing interfaces defined in `Domain`. Interfaces (e.g., `ITextExtractor`, repositories) live in `Domain`; their implementations live in `Business` (logic) or `Infrastructure` (I/O), wired together via dependency injection in `Api`. This mirrors the structure of the team's existing services, so patterns learned here carry over directly.

### Scope of work

**1. Document upload and storage**
- `POST /documents` — accepts a file upload (start with `.txt` and `.md`).
- Store the raw file on a local volume; store metadata (id, filename, size, content type, upload timestamp, status) in the database.
- Validate inputs: file presence, allowed extensions, maximum file size. Reject invalid requests with clear error responses.

**2. Document retrieval and deletion**
- `GET /documents` — list all documents with metadata (add simple paging).
- `GET /documents/{id}` — fetch a single document's metadata.
- `DELETE /documents/{id}` — remove the document's file and metadata.
- Correct status codes throughout: 201 on create, 404 for unknown ids, 400 for bad input.

**3. Text extraction**
- A text-extraction service that reads the stored file and returns its plain-text content, behind an interface (e.g., `ITextExtractor`) with implementations per file type.
- Plain text and Markdown in scope for this phase; PDF extraction (via the PdfPig library) is a stretch item.

**4. Cross-cutting foundations**
- Structured logging with Serilog (request logging + meaningful log statements in services).
- Global error-handling middleware — no raw stack traces in API responses.
- Input validation with FluentValidation.
- Configuration via `appsettings.json` and the options pattern — no hard-coded paths or connection strings.
- Swagger/OpenAPI enabled with the endpoints documented.

**5. Testing**
- Unit tests for the service layer (validation logic, text extraction) using mocked dependencies.
- At least one integration test covering the upload endpoint end to end (using `WebApplicationFactory`).

### Definition of done

- All endpoints work and are demonstrated via Swagger from a clean start.
- Uploading, listing, fetching, and deleting a document works against real storage (file + database), not in-memory shortcuts.
- Invalid inputs return clear, consistent error responses; nothing returns an unhandled 500.
- Tests pass in one command (`dotnet test`); the build has no warnings treated as acceptable noise.
- Code is merged to the main branch through reviewed pull requests.
- The README explains how to run the project and its tests locally.

### Checkpoint at the end of Phase 1

A short review session covering: a live demo of the API, a walkthrough of the solution structure and one or two key design choices, and feedback on code quality. The outcome of this checkpoint feeds directly into Phase 2, where documents get chunked and embedded into the vector database.

---

*Questions at any point — ask early rather than late. A 10-minute conversation often saves a day of rework.*
