
# DocMind — Internal Documentation Assistant

A full-stack internal documentation assistant built with a **.NET 8 Web API** and a **React user interface**. DocMind ingests internal documents, extracts their text, splits it into overlapping chunks, generates embeddings using a local Ollama model, and stores vectors in Qdrant. MongoDB stores document metadata, while original uploaded files are stored locally.

DocMind uses **Retrieval-Augmented Generation (RAG)** to retrieve relevant document chunks and generate answers grounded in internal documentation. It also includes a dedicated Evaluation Console application to measure retrieval quality, answer accuracy, refusal behavior, and response performance.

## Badges

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![C#](https://img.shields.io/badge/C%23-Backend-239120)
![React](https://img.shields.io/badge/React-Frontend-61DAFB)
![MongoDB](https://img.shields.io/badge/Database-MongoDB-47A248)
![Qdrant](https://img.shields.io/badge/Vector%20DB-Qdrant-DC382D)
![Ollama](https://img.shields.io/badge/AI-Ollama-black)

## Features

- Upload supported text and Markdown documents.
- Store uploaded files in local storage.
- Store document metadata in MongoDB.
- Extract text and split it into overlapping chunks.
- Generate embeddings locally using Ollama and `nomic-embed-text`.
- Store chunk vectors and metadata in Qdrant.
- Process document ingestion asynchronously using `BackgroundService` and `Channel<Guid>`.
- Track document processing statuses.
- Retrieve semantically relevant document chunks.
- Answer questions using retrieved document context and a local LLM.
- Return source citations and retrieved-chunk information.
- Support conversation history in chat requests.
- Provide a React-based user interface for document management and AI chat.
- Evaluate retrieval and answer quality using a dedicated .NET console application.
- Generate timestamped evaluation reports with performance and accuracy metrics.

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / .NET 8 | Backend development |
| ASP.NET Core Web API | REST API |
| React | Frontend user interface |
| MongoDB | Document metadata storage |
| Qdrant | Vector database and similarity search |
| Ollama | Local AI inference |
| nomic-embed-text | Embedding generation |
| llama3.2:3b | Chat-generation model |
| Docker Compose | Local service management |
| Swagger / OpenAPI | API documentation |
| xUnit | Automated testing |

## LLM Model Selection and Performance Optimization

DocMind uses locally hosted Large Language Models through Ollama to generate answers using retrieved document chunks.

### Models Evaluated

| Model | Evaluation Purpose |
|---|---|
| `llama3.2:3b` | Lightweight model for faster inference |
| `llama3.2:9b` | Alternative model for response quality |
| `llama3.1:8b` | Alternative model for response quality and performance |
| `qwen2.5:7b` | Alternative chat-generation model |
| `phi3:mini` | Lightweight alternative |

After experimenting with these models, `llama3.2:3b` was selected as the current chat-generation model because it offers a practical balance between response quality, inference speed, and local hardware requirements.

### Retrieval Optimization: Top-K Reduction

The Top-K setting determines how many relevant document chunks are retrieved from Qdrant and provided to the chat model.

The retrieval count was reduced from 5 chunks to 3 chunks to reduce prompt context and improve response time.

| Setting | Previous Value | Current Value |
|---|---|---|
| Top-K | 5 | 3 |

### Current Configuration

| Setting | Value |
|---|---|
| LLM Provider | Ollama |
| Chat Model | `llama3.2:3b` |
| Embedding Model | `nomic-embed-text` |
| Vector Database | Qdrant |
| Retrieval Method | Semantic Similarity Search |
| Top-K | 3 |
| Minimum Similarity Score | 0.50 |
| Chunk Size | 500 |
| Chunk Overlap | 50 |

**Note:** `llama3.2:3b` generates natural-language answers, while `nomic-embed-text` generates embeddings for semantic retrieval.

## Project Architecture

The solution follows a layered architecture:

| Project | Responsibility |
|---|---|
| Api | Controllers, middleware, configuration, and dependency injection |
| Business | Application workflows, document processing, retrieval, and chat logic |
| Domain | Core entities, models, and interfaces |
| Infrastructure | MongoDB, local file storage, text extraction, Ollama, and Qdrant integration |
| Tests | Automated unit tests |
| Evaluation | Console application for evaluating retrieval and answer quality |
| React UI | User-facing document management and chat interface |

The API depends on Business, Business depends on Domain, and Infrastructure implements interfaces defined by Domain.

The Evaluation project reuses the same Business services as the API instead of maintaining a separate copy of the retrieval and chat pipeline.

## DocMind User Interface

DocMind includes a React-based user interface that allows users to interact with the documentation assistant without directly using Swagger.

### Main Capabilities

- Upload and view available documents.
- Interact with the document collection.
- Ask questions about internal documentation.
- View AI-generated responses.
- Review source citations and retrieved document chunks.
- Perform document-based question answering through the RAG pipeline.

The React frontend communicates with the .NET Web API, which handles document ingestion, retrieval, and answer generation.

## Evaluation Console Application

DocMind includes a dedicated .NET console application:

`Kastle.DocMind.Evaluation`

The application evaluates the actual retrieval and chat pipeline using a predefined set of questions.

### Evaluation Metrics

| Metric | Description |
|---|---|
| Retrieval Hit Rate @K | Checks whether the expected source document appears in retrieved chunks |
| False Refusals | Counts answerable questions incorrectly refused |
| False Answers | Counts out-of-scope questions incorrectly answered |
| Keyword Score | Measures matching between expected answer points and generated answers |
| Citation Correctness | Checks whether citations refer to the expected source |
| Latency | Measures response generation time |
| p50 / p95 | Measures median and 95th-percentile response latency |

### Baseline Evaluation Results

The initial evaluation run tested 10 predefined questions.

| Metric | Result |
|---|---:|
| Total Questions | 10 |
| Answerable Questions | 8 |
| Out-of-Scope Questions | 2 |
| Retrieval Hit Rate @K | 0% (0/8) |
| False Refusals | 0 |
| False Answers | 1 |
| Average Keyword Score | 29% |
| Latency p50 | 12.7 seconds |
| Latency p95 | 36.3 seconds |

**Evaluation Observations:**

- The system correctly refused one out-of-scope question.
- One out-of-scope question was incorrectly answered.
- Retrieval hit rate requires further investigation.
- Answer keyword matching indicates opportunities to improve response quality.
- Response latency provides a baseline for future performance optimization.

These results represent the initial baseline evaluation and are intended to guide further improvements.

### Run the Evaluation Console

From the repository root:

```bash
dotnet run --project src/Kastle.DocMind.Evaluation
```

The evaluation application runs the predefined questions through the actual retrieval and chat pipeline and generates a timestamped report.

Evaluation runs are maintained under:

```text
eval/
├── questions.json
├── baseline-results.md
└── runs/
```

## API Reference

The API runs locally at:

`http://localhost:5144`

Swagger documentation:

`http://localhost:5144/swagger`

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/documents` | Upload a document |
| GET | `/documents` | List available documents |
| GET | `/documents/{id}` | Get document details |
| DELETE | `/documents/{id}` | Delete a document and associated vectors |
| GET | `/documents/{id}/chunks` | Retrieve stored document chunks |
| POST | `/chat` | Ask questions using retrieved document context |

Refer to Swagger UI for the exact request and response schemas.

## Environment Configuration

The following settings are used by the application:

| Setting | Example | Purpose |
|---|---|---|
| `ASPNETCORE_URLS` | `http://+:8080` | API listen address |
| `Ollama__BaseUrl` | `http://ollama:11434` | Ollama service URL |
| `Ollama__EmbeddingModel` | `nomic-embed-text` | Embedding model |
| `Qdrant__Host` | `qdrant` | Qdrant hostname |
| `Qdrant__Port` | `6334` | Qdrant gRPC port |
| `MongoDbSettings__ConnectionString` | `mongodb://mongodb:27017` | MongoDB connection |

Other settings, including database names, storage paths, and chunking configuration, are maintained in application configuration files.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js and npm
- Docker Desktop with Docker Compose
- Git
- Ollama

### 1. Clone the Repository

```bash
git clone https://github.com/ayushraj2004/Kastle-DocMind.git
cd Kastle-DocMind
```

### 2. Pull Required Ollama Models

```bash
ollama pull llama3.2:3b
ollama pull nomic-embed-text
```

### 3. Start Backend Services

From the directory containing `compose.yaml`:

```bash
docker compose up -d --build
```

Check running containers:

```bash
docker compose ps
```

### 4. Run the React UI

Navigate to the React UI project directory (the directory containing `package.json`):

```bash
npm install
npm run dev
```

Open the local URL displayed by the React development server.

Ensure the frontend API configuration points to:

```text
http://localhost:5144
```

### 5. Access Services

| Service | URL |
|---|---|
| API | http://localhost:5144 |
| Swagger UI | http://localhost:5144/swagger |
| Qdrant Dashboard | http://localhost:6333/dashboard |

### Run API Without Docker

Restore and build the solution:

```bash
dotnet restore
dotnet build
```

Run the API:

```bash
dotnet run --project src/Kastle.DocMind.Api
```

Ensure MongoDB, Qdrant, and Ollama are running and reachable using the configured host addresses and ports.

## Usage Workflow

1. Start the backend services.
2. Launch the React UI.
3. Upload a supported document.
4. DocMind stores the original file and metadata.
5. The background ingestion worker extracts and chunks the document.
6. Ollama generates embeddings for the chunks.
7. Qdrant stores the vectors and associated metadata.
8. The document status changes to `Indexed`.
9. Ask a question through the chat interface.
10. DocMind retrieves relevant chunks and generates an answer using the local chat model.
11. The UI displays the answer and supporting source citations.

## Demo

The complete workflow:

**Document Upload → Text Extraction → Chunking → Embedding Generation → Qdrant Storage → Semantic Retrieval → LLM Answer Generation → Source Citations**

The Evaluation Console can independently test the retrieval and chat workflow and generate reports to measure system quality.

## 🖥️ Application Screenshots

### 1. DocMind Chat
![DocMind Chat](https://yt3.ggpht.com/lKsU8ziVfOLSb5G12dfX4DKPv_Q073jKlbcAS6t31RpwRLFaZPrlcEafuAs4YAfYqK8XGzISI_xCXA=s800-rw-nd-v1)


## Running Tests

Run automated tests:

```bash
dotnet test
```

Build the complete solution:

```bash
dotnet build
```

## Deployment

The project is currently configured for local development using Docker Compose. Cloud deployment, such as Azure hosting, is not included in the current scope.

Start the containers:

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
- [x] Document chunk inspection endpoint
- [x] Semantic retrieval and RAG chat
- [x] Source citations in chat responses
- [x] Conversation history support
- [x] React-based user interface
- [x] Evaluation console application
- [x] Evaluation reports with retrieval and answer metrics
- [x] Model experimentation and Top-K optimization
- [ ] Further improve retrieval accuracy and answer grounding
- [ ] Expand evaluation dataset and performance experiments

## Contributing

This is currently an internship and learning project. For proposed changes:

1. Create a branch for your changes.
2. Keep responsibilities within the appropriate solution project.
3. Build and run tests before submitting a pull request.
4. Describe the changes and how they were verified.

## Author

**Ayush Raj**

GitHub: [ayushraj2004](https://github.com/ayushraj2004)

## Acknowledgements

- .NET and ASP.NET Core teams
- MongoDB, Qdrant, and Ollama projects
- Open-source libraries used in this project
- GlobalLogic internship mentors and project guides

## FAQ

**Why use Qdrant?**

Qdrant stores document chunk embeddings and metadata, allowing DocMind to retrieve semantically relevant information.

**Why use MongoDB?**

MongoDB stores document metadata, including document IDs, filenames, and processing information.

**Why process ingestion in the background?**

Text extraction, chunking, and embedding generation can take time. Background processing allows upload requests to finish without waiting for the entire ingestion pipeline.

**Why use Ollama?**

Ollama allows embedding generation and chat-model inference to run locally instead of relying on hosted AI APIs.

**Does DocMind support document-based question answering?**

Yes. DocMind retrieves relevant chunks from Qdrant and passes them to the local chat model to generate answers with supporting source information.

**What is the Evaluation Console used for?**

It runs predefined questions through the actual application pipeline and measures retrieval quality, answer scoring, refusal behavior, citations, and response latency.

## Support

For project-related questions or issues, contact the author or raise an issue in the GitHub repository.

## Appendix

### Local Service Ports

| Service | Host Port |
|---|---|
| API | 5144 |
| MongoDB | 27017 |
| Qdrant HTTP / Dashboard | 6333 |
| Qdrant gRPC | 6334 |
| Ollama | 11434 |

### Chunking and Vector Configuration

- Chunking is configured through `ChunkingOptions`.
- Current target chunk size: approximately 500 tokens.
- Current chunk overlap: approximately 50 tokens.
- Embedding model: `nomic-embed-text`.
- Vector dimensions: 768.
- Qdrant collection: `docmind_chunks`.
- Distance metric: Cosine.

These values should remain aligned with the actual application configuration and embedding output.
