# DocMind — Internal Documentation Assistant

A .NET 8 Web API that ingests internal documents, extracts their text, splits it into overlapping chunks, generates embeddings with a local Ollama model, and stores vectors and chunk metadata in Qdrant. MongoDB stores document metadata, while the original uploaded files are stored locally. DocMind also provides a retrieval-augmented generation (RAG) chat workflow that retrieves relevant document chunks and uses a local LLM to generate answers grounded in the available documentation.

## Badges

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![C#](https://img.shields.io/badge/C%23-Backend-239120)
![Qdrant](https://img.shields.io/badge/Vector%20DB-Qdrant-DC382D)
![Ollama](https://img.shields.io/badge/Embeddings-Ollama-black)

## Features

- Upload supported text and Markdown documents.
- Store uploaded files in local storage.
- Store document metadata in MongoDB.
- Extract text from uploaded files.
- Split extracted text into chunks with overlap.
- Generate embeddings locally using Ollama and `nomic-embed-text`.
- Store chunk vectors and metadata in Qdrant.
- Process ingestion asynchronously using `BackgroundService` and `Channel<Guid>`.
- Track document processing status.
- Retrieve a document's stored chunks for debugging.
- Delete document vectors when a document is deleted.
- Retrieve semantically relevant document chunks from Qdrant.
- Answer questions using retrieved document context and a local chat model.
- Return source citations and retrieved-chunk information with chat responses.
- Support conversation history in chat requests.

## Tech Stack

- **.NET 8 / ASP.NET Core Web API** — API and application host
- **C#** — application implementation
- **MongoDB** — document metadata
- **Local file storage** — original uploaded files
- **Ollama** — local embedding generation
- **nomic-embed-text** — embedding model
- **Qdrant** — vector database
- **Docker Compose** — local services
- **Swagger** — API exploration
- **xUnit** — automated tests (where implemented in the test project)

## LLM Model Selection and Performance Optimization

DocMind uses locally hosted Large Language Models (LLMs) through Ollama to generate answers using the document chunks retrieved from Qdrant. Several chat-generation models were explored to find a practical balance between answer quality, response time, and local hardware requirements.

### Models Evaluated

The following models were tested during the model-selection process:

| Model | Evaluation Purpose |
|---|---|
| `llama3.2:3b` | Lightweight model evaluated for faster local inference |
| `llama3.2:9b` | Alternative model evaluated for response quality |
| `llama3.1:8b` | Alternative model evaluated for response quality and performance |
| `qwen2.5:7b` | Alternative model for answer generation |
| `phi3:mini` | Lightweight alternative for local inference |

After experimenting with these options, `llama3.2:3b` was selected as the current chat-generation model. It provides a practical balance between response quality, inference speed, and local system resource requirements.

### Retrieval Optimization: Top-K Reduction

Before generating an answer, DocMind performs semantic similarity search to retrieve relevant chunks from Qdrant. The Top-K setting controls how many of the highest-ranked chunks are returned for a question and provided as context to the chat model.

The retrieval count was reduced from 5 chunks to 3 chunks. Returning fewer chunks can reduce the size of the context included in the prompt, which can lower input-token usage and reduce the amount of information the model needs to process. This change is intended to help improve response time while keeping the context focused on the most relevant passages.

| Retrieval Setting | Previous Value | Current Value |
|---|---|---|
| Top-K | 5 | 3 |

A lower Top-K value does not automatically guarantee better answer quality. Useful information may sometimes appear in the chunks ranked below the top three. The evaluation dataset is therefore used to assess the effect of this change on answer relevance, retrieval quality, and latency.

### Current Model and Retrieval Configuration

| Setting | Current Value |
|---|---|
| LLM provider | Ollama |
| Chat-generation model | `llama3.2:3b` |
| Embedding model | `nomic-embed-text` |
| Vector database | Qdrant |
| Retrieval method | Semantic similarity search |
| Top-K retrieved chunks | 3 |
| Minimum similarity score | 0.50 |
| Chunk size | 500 |
| Chunk overlap | 50 |

> **Note:** The chat-generation model and embedding model have different responsibilities. `llama3.2:3b` generates natural-language answers, while `nomic-embed-text` generates embeddings used for semantic retrieval. Changing the chat model does not by itself change the embedding model or the vectors already stored in Qdrant.

### Evaluation Goals

The model and retrieval configuration can be evaluated using the predefined question dataset. The main measurements are:

- **Answer relevance:** whether the generated answer addresses the question and is supported by the supplied document context.
- **Retrieval quality:** whether the retrieved chunks contain the information needed to answer the question.
- **Response latency:** the time taken to retrieve context and generate an answer.
- **Token usage:** the amount of prompt and generated text processed by the model.

These measurements help determine whether the selected model and lower Top-K value provide a suitable balance between response quality and performance. Specific percentage improvements should only be reported after they have been measured.

## Architecture

- **Api** — controllers, middleware, configuration, and dependency injection
- **Business** — document and ingestion workflows, chunking, and background processing
- **Domain** — entities and interfaces
- **Infrastructure** — MongoDB repository, local file storage, text extraction, and Qdrant integration
- **Tests** — automated tests

The API depends on Business, Business depends on Domain, and Infrastructure implements interfaces defined by Domain. Dependency injection is configured in the API project.

## API Reference

The API is available locally at `http://localhost:5144` when running with the provided Docker Compose port mapping. Swagger UI is available at `/swagger` in Development mode.

| Method | Endpoint | Purpose |
|---|---|---|
| `POST` | `/documents` | Upload a document |
| `GET` | `/documents` | List documents |
| `GET` | `/documents/{id}` | Get document details |
| `DELETE` | `/documents/{id}` | Delete a document and its associated vectors |
| `GET` | `/documents/{id}/chunks` | Retrieve stored chunks for a document |

Confirm the exact routes and request schemas in Swagger, as they can change with the controller implementation.

## Environment Variables

The following settings are used by the application. Docker Compose can supply these values to the API container.

| Setting | Example | Purpose |
|---|---|---|
| `ASPNETCORE_URLS` | `http://+:8080` | API listen address inside the container |
| `Ollama__BaseUrl` | `http://ollama:11434` | Ollama service URL from the API container |
| `Ollama__EmbeddingModel` | `nomic-embed-text` | Embedding model |
| `Qdrant__Host` | `qdrant` | Qdrant service hostname from the API container |
| `Qdrant__Port` | `6334` | Qdrant gRPC port |
| `MongoDbSettings__ConnectionString` | `mongodb://mongodb:27017` | MongoDB connection string |

Other settings, such as the MongoDB database/collection names, file storage path, and chunking options, are configured through application configuration. Use your local configuration files for their exact values.

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

- .NET 8 SDK
- Docker Desktop with Docker Compose
- Git

### Start the services

From the directory containing `compose.yaml`, run:

```bash
docker compose up -d --build
```

Check service status:

```bash
docker compose ps
```

Open:

- API: `http://localhost:5144`
- Swagger UI: `http://localhost:5144/swagger`
- Qdrant dashboard: `http://localhost:6333/dashboard`

The API must connect to the Compose service names (`mongodb`, `qdrant`, and `ollama`) when it runs inside Docker. If running the API directly on the host, use the corresponding host addresses and ports instead.

### Run the API without Docker

Restore and build the solution:

```bash
dotnet restore
dotnet build
```

Run the API project:

```bash
dotnet run --project src/Kastle.DocMind.Api
```

When running outside Docker, ensure MongoDB, Qdrant, and Ollama are running and that the configured hostnames and ports are reachable.

## Usage / Examples

1. Start the services.
2. Open Swagger UI.
3. Use `POST /documents` to upload a supported document.
4. The API stores the file and document metadata, then queues the document for background ingestion.
5. Wait for the document status to become `Indexed`.
6. Use `GET /documents/{id}/chunks` to inspect the chunks stored in Qdrant.

Example request to retrieve chunks:

```http
GET /documents/1c129994-50ab-40b0-8d23-22318d93a2d9/chunks
```

The response contains chunk fields such as `id`, `documentId`, `sequenceNumber`, `text`, `fileName`, `section`, and `tokenCount`.

## Demo

The demo flow is document upload → background ingestion → vector storage in Qdrant → semantic retrieval → answer generation using the local chat model → response with source citations. The chat endpoint can also receive conversation history to support follow-up questions.

## Documentation

- Use Swagger UI at `/swagger` to inspect available endpoints and schemas.
- See the source projects under `src/` for the API, Business, Domain, and Infrastructure implementation.
- See the `tests/` directory for automated tests.

## Running Tests

Run the test suite from the solution root:

```bash
dotnet test
```

To build the solution:

```bash
dotnet build
```

## Deployment

The project is currently set up for local development using Docker Compose. Cloud deployment (for example, Azure) is not included in the current scope.

For a local container run:

```bash
docker compose up -d --build
```

## Roadmap

- [x] Document upload and local file storage
- [x] Document metadata persistence
- [x] Text and Markdown extraction
- [x] Chunking with overlap
- [x] Local embedding generation with Ollama
- [x] Qdrant vector storage
- [x] Background ingestion pipeline
- [x] Chunk inspection endpoint
- [x] Implement document retrieval for question answering
- [x] Add a chat endpoint with answers grounded in retrieved chunks
- [x] Return source citations with answers
- [ ] Add a user-facing chat interface
- [ ] Continue evaluating model quality, retrieval settings, latency, and token usage

## Contributing

This is currently an internship/learning project. For proposed changes:

1. Create a branch for your change.
2. Keep responsibilities within the appropriate solution project.
3. Build and run tests before submitting a pull request.
4. Describe the change and how it was verified.

## Authors

- Ayush Raj

## Acknowledgements

- The .NET and ASP.NET Core teams
- The MongoDB, Qdrant, and Ollama projects
- The maintainers of the libraries used in this project
- Project guide and internship mentors

## FAQ

**Why use Qdrant?**
It stores chunk embeddings and metadata so semantically relevant chunks can be retrieved later.

**Why store the original files locally?**
The original document remains available for extraction, reprocessing, and document management, while MongoDB stores its metadata.

**Why process ingestion in the background?**
Text extraction, chunking, and embedding can take time. A background worker lets the upload request finish without waiting for the entire pipeline.

**Why use Ollama?**
It allows both embedding generation and chat-model inference to run locally, rather than requiring a hosted model API. DocMind uses `nomic-embed-text` for embeddings and `llama3.2:3b` as its current chat-generation model.

**Does the project currently answer questions about documents?**
Yes. DocMind retrieves relevant chunks from Qdrant and passes them as context to its local chat-generation model. The response includes answer and source information from the retrieved document chunks.

## Support

For project questions or issues, contact the author or raise an issue in the project's repository.

## Appendix

### Local service ports

| Service | Host port |
|---|---|
| API | `5144` |
| MongoDB | `27017` |
| Qdrant HTTP/dashboard | `6333` |
| Qdrant gRPC | `6334` |
| Ollama | `11434` |

### Chunking and vectors

- Chunking is configured through `ChunkingOptions`.
- The current target is approximately 500 tokens per chunk with approximately 50 tokens of overlap.
- The `nomic-embed-text` setup uses 768-dimensional vectors.
- Qdrant collection: `docmind_chunks`
- Distance metric: Cosine

These values should stay aligned with the actual application configuration and embedding output.
