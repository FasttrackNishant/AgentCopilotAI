
# AgentCopilot.AI – AI-Powered Contact Center Assistant

AgentCopilot.AI is an intelligent customer support assistant that enables real-time chat between customers and agents with AI-powered summarization and smart suggestions.

---

## 🚀 Features

- Real-time chat using SignalR (agent ↔ customer)
- Context-aware AI chat summarization using Azure OpenAI
- Multi-turn conversation memory powered by Redis
- Frontend built with React, backend in .NET 9
- Containerized with Docker for full-stack deployment

---

## 🛠️ Tech Stack

| Layer         | Technology                |
|---------------|----------------------------|
| Frontend      | React, JavaScript, Nginx   |
| Backend       | ASP.NET Core (.NET 9)      |
| Realtime Comm | SignalR                    |
| AI Engine     | Azure OpenAI (Chat GPT)    |
| Memory Store  | Redis                      |
| Container     | Docker                     |

---

## 📁 Project Structure

```
AgentCopilot/
├── frontend/           # React app
├── backend/
│   ├── api/            # .NET 9 backend
│   └── apisolution.sln
├── docker-compose.yml
└── README.md
```

---

## ⚙️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/AgentCopilot.AI.git
cd AgentCopilot.AI
```

### 2. Configure Environment

Update the following in `appsettings.json` or as environment variables:

```json
"AzureOpenAI": {
  "ApiKey": "your-api-key",
  "Endpoint": "https://your-resource.openai.azure.com",
  "Deployment": "your-model-deployment",
  "ApiVersion": "2024-05-01-preview"
}
```

### 3. Run with Docker

```bash
docker-compose up --build
```

- Frontend: http://localhost:5173  
- Backend API: http://localhost:5000  
- Redis: runs internally

---

## 🧠 How It Works

- Customers and agents communicate over SignalR-based chat.
- All messages are stored in Redis under a `chatId`.
- Backend retrieves recent messages and sends them as prompt to Azure OpenAI to generate a summary.
- Agent can view chat history summary in real-time.

---

## 📝 Future Scope

- AI-suggested replies to speed up agent response
- Authentication & role-based dashboards
- Chat rating and analytics
- Multi-channel integration (email, WhatsApp)

---

## 🤝 Contributing

Pull requests and feedback are welcome. Start a discussion or fork the repo to explore!

---

## 📄 License

This project is licensed under the MIT License.
