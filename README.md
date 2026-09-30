

# DocMind --- Internal Documentation Assistant

A .NET 8 Web API that ingests internal documents, extracts their text,
splits it into overlapping chunks, generates embeddings with a local
Ollama model, and stores vectors and chunk metadata in Qdrant. MongoDB
stores document metadata, while the original uploaded files are stored
locally. The current implementation focuses on document ingestion and
verification; a user-facing chat/RAG experience is planned for a later
phase.
## Badges

Add badges from somewhere like: [shields.io](https://shields.io/)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![C%23](https://img.shields.io/badge/C%23-Backend-239120)
![Qdrant](https://img.shields.io/badge/Vector%20DB-Qdrant-DC382D)
![Ollama](https://img.shields.io/badge/Embeddings-Ollama-black)


## Features

-   Upload supported text and Markdown documents.
-   Store uploaded files in local storage.
-   Store document metadata in MongoDB.
-   Extract text from uploaded files.
-   Split extracted text into chunks with overlap.
-   Generate embeddings locally using Ollama and `nomic-embed-text`.
-   Store chunk vectors and metadata in Qdrant.
-   Process ingestion asynchronously using `BackgroundService` and
    `Channel<Guid>`.
-   Track document processing status.
-   Retrieve a document's stored chunks for debugging.
-   Delete document vectors when a document is deleted.


## Tech Stack

-   **.NET 8 / ASP.NET Core Web API** --- API and application host
-   **C#** --- application implementation
-   **MongoDB** --- document metadata
-   **Local file storage** --- original uploaded files
-   **Ollama** --- local embedding generation
-   **nomic-embed-text** --- embedding model
-   **Qdrant** --- vector database
-   **Docker Compose** --- local services
-   **Swagger** --- API exploration
-   **xUnit** --- automated tests (where implemented in the test
    project)


## Architecture 
-   **Api** --- controllers, middleware, configuration, and dependency
    injection
-   **Business** --- document and ingestion workflows, chunking, and
    background processing
-   **Domain** --- entities and interfaces
-   **Infrastructure** --- MongoDB repository, local file storage, text
    extraction, and Qdrant integration
-   **Tests** --- automated tests

The API depends on Business, Business depends on Domain, and
Infrastructure implements interfaces defined by Domain. Dependency
injection is configured in the API project.


## API Reference

The API is available locally at `http://localhost:5144` when running
with the provided Docker Compose port mapping. Swagger UI is available
at `/swagger` in Development mode.

  --------------------------------------------------------------------------
  Method                  Endpoint                   Purpose
  ----------------------- -------------------------- -----------------------
  `POST`                  `/documents`               Upload a document

  `GET`                   `/documents`               List documents

  `GET`                   `/documents/{id}`          Get document details

  `DELETE`                `/documents/{id}`          Delete a document and
                                                     its associated vectors

  `GET`                   `/documents/{id}/chunks`   Retrieve stored chunks
                                                     for a document
  --------------------------------------------------------------------------

Confirm the exact routes and request schemas in Swagger, as they can
change with the controller implementation.
## Environment Variables

The following settings are used by the application. Docker Compose can
supply these values to the API container.

  -----------------------------------------------------------------------------------------
  Setting                               Example                     Purpose
  ------------------------------------- --------------------------- -----------------------
  `ASPNETCORE_URLS`                     `http://+:8080`             API listen address
                                                                    inside the container

  `Ollama__BaseUrl`                     `http://ollama:11434`       Ollama service URL from
                                                                    the API container

  `Ollama__EmbeddingModel`              `nomic-embed-text`          Embedding model

  `Qdrant__Host`                        `qdrant`                    Qdrant service hostname
                                                                    from the API container

  `Qdrant__Port`                        `6334`                      Qdrant gRPC port

  `MongoDbSettings__ConnectionString`   `mongodb://mongodb:27017`   MongoDB connection
                                                                    string
  -----------------------------------------------------------------------------------------

Other settings, such as the MongoDB database/collection names, file
storage path, and chunking options, are configured through application
configuration. Use your local configuration files for their exact
values.


## Run Locally

Clone the project

```bash
  git clone https://github.com/ayushraj2004/Kastle-DocMind.git
```

Go to the project directory

```bash
  cd my-project
```

Install dependencies

```bash
  npm install
```

Start the server

```bash
  npm run start
```

### Prerequisites

-   .NET 8 SDK
-   Docker Desktop with Docker Compose
-   Git

### Start the services

From the directory containing `compose.yaml`, run:

``` bash
docker compose up -d --build
```

Check service status:

``` bash
docker compose ps
```

Open:

-   API: `http://localhost:5144`
-   Swagger UI: `http://localhost:5144/swagger`
-   Qdrant dashboard: `http://localhost:6333/dashboard`

The API must connect to the Compose service names (`mongodb`, `qdrant`,
and `ollama`) when it runs inside Docker. If running the API directly on
the host, use the corresponding host addresses and ports instead.

### Run the API without Docker

Restore and build the solution:

``` bash
dotnet restore
dotnet build
```

Run the API project:

``` bash
dotnet run --project src/Kastle.DocMind.Api
```

When running outside Docker, ensure MongoDB, Qdrant, and Ollama are
running and that the configured hostnames and ports are reachable.

## Usage / Examples

1.  Start the services.
2.  Open Swagger UI.
3.  Use `POST /documents` to upload a supported document.
4.  The API stores the file and document metadata, then queues the
    document for background ingestion.
5.  Wait for the document status to become `Indexed`.
6.  Use `GET /documents/{id}/chunks` to inspect the chunks stored in
    Qdrant.

Example request to retrieve chunks:

``` http
GET /documents/1c129994-50ab-40b0-8d23-22318d93a2d9/chunks
```

The response contains chunk fields such as `id`, `documentId`,
`sequenceNumber`, `text`, `fileName`, `section`, and `tokenCount`.

## Demo

The current demo flow is document upload → background ingestion → chunk
retrieval through the debug endpoint. A full conversational "chat with
your documents" demo is planned for a later phase.

## Documentation

-   Use Swagger UI at `/swagger` to inspect available endpoints and
    schemas.
-   See the source projects under `src/` for the API, Business, Domain,
    and Infrastructure implementation.
-   See the `tests/` directory for automated tests.

## Running Tests

Run the test suite from the solution root:

``` bash
dotnet test
```

To build the solution:

``` bash
dotnet build
```

## Deployment

The project is currently set up for local development using Docker
Compose. Cloud deployment (for example, Azure) is not included in the
current scope.

For a local container run:

``` bash
docker compose up -d --build
```

## Roadmap

-   [x] Document upload and local file storage
-   [x] Document metadata persistence
-   [x] Text and Markdown extraction
-   [x] Chunking with overlap
-   [x] Local embedding generation with Ollama
-   [x] Qdrant vector storage
-   [x] Background ingestion pipeline
-   [x] Chunk inspection endpoint
-   [ ] Implement document retrieval for question answering
-   [ ] Add a chat endpoint with answers grounded in retrieved chunks
-   [ ] Return source citations with answers
-   [ ] Add a user-facing chat interface

## Contributing

This is currently an internship/learning project. For proposed changes:

1.  Create a branch for your change.
2.  Keep responsibilities within the appropriate solution project.
3.  Build and run tests before submitting a pull request.
4.  Describe the change and how it was verified.

## Authors

-   Ayush Raj

## Acknowledgements

-   The .NET and ASP.NET Core teams
-   The MongoDB, Qdrant, and Ollama projects
-   The maintainers of the libraries used in this project
-   Project guide and internship mentors

## FAQ

**Why use Qdrant?**\
It stores chunk embeddings and metadata so semantically relevant chunks
can be retrieved later.

**Why store the original files locally?**\
The original document remains available for extraction, reprocessing,
and document management, while MongoDB stores its metadata.

**Why process ingestion in the background?**\
Text extraction, chunking, and embedding can take time. A background
worker lets the upload request finish without waiting for the entire
pipeline.

**Why use Ollama?**\
It allows the embedding model to run locally rather than requiring a
hosted embedding API.

**Does the project currently answer questions about documents?**\
The current implemented scope covers ingestion and chunk
storage/inspection. The conversational RAG flow is planned for a later
phase.

## Support

For project questions or issues, contact the author or raise an issue in
the project's repository.

## Appendix

### Local service ports

  Service                   Host port
  ----------------------- -----------
  API                          `5144`
  MongoDB                     `27017`
  Qdrant HTTP/dashboard        `6333`
  Qdrant gRPC                  `6334`
  Ollama                      `11434`

### Chunking and vectors

-   Chunking is configured through `ChunkingOptions`.
-   The current target is approximately 500 tokens per chunk with
    approximately 50 tokens of overlap.
-   The `nomic-embed-text` setup uses 768-dimensional vectors.
-   Qdrant collection: `docmind_chunks`
-   Distance metric: Cosine

These values should stay aligned with the actual application
configuration and embedding output.
