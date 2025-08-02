import React, { useEffect, useRef, useState } from "react";
import { sendMessage, startConnection } from "../services/signalr";
import axios from "axios"; 

const AgentChat = () => {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState("");
  const chatId = "chat-123";
  const messageEndRef = useRef(null);
  const [loading, setLoading] = useState(false);
  const [summary, setSummary] = useState("");

  useEffect(() => {
    const init = async () => {
      const connection = await startConnection(chatId, (sender, message, timestamp) => {
        setMessages(prev => [...prev, { sender, message, timestamp }]);
      });

      // Load history from Redis
      const history = await connection.invoke("LoadHistory", chatId);
      const parsed = history.map(msg => JSON.parse(msg));
      setMessages(parsed);
    };

    init();
  }, []);

  useEffect(() => {
    messageEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = () => {
    if (!input.trim()) return;
    sendMessage(chatId, "Agent", input);
    setInput("");
  };

  const handleGenerateSummary = async () => {
    try {
      setLoading(true);
      const res = await axios.get(`http://localhost:5000/api/chat/summary/${chatId}`);
      setSummary(res.data.summary);
    } catch (err) {
      console.error("Failed to fetch summary", err);
      setSummary("Failed to generate summary.");
    } finally {
      setLoading(false);
    }
  };


  return (
    <div style={{ maxWidth: 600, margin: "auto", padding: 20 }}>
      <h2>Agent Chat</h2>
      <div style={{ border: "1px solid #ccc", padding: 10, height: 400, overflowY: "auto", borderRadius: 8 }}>
        {messages.map((m, i) => (
          <div key={i} style={{ marginBottom: 10 }}>
            <strong>{m.sender}:</strong> {m.message}
            <div style={{ fontSize: 10, color: "#999" }}>{new Date(m.timestamp).toLocaleTimeString()}</div>
          </div>
        ))}
        <div ref={messageEndRef}></div>
      </div>
      <div style={{ marginTop: 10, display: "flex", gap: 8 }}>
        <input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type message..."
          style={{ flex: 1, padding: 10, borderRadius: 6, border: "1px solid #ccc" }}
        />
        <button onClick={handleSend} style={{ padding: "10px 16px", borderRadius: 6 }}>Send</button>
      </div>

	   <div style={{ marginTop: 20 }}>
        <button onClick={handleGenerateSummary} disabled={loading} style={{ padding: "10px 16px", borderRadius: 6 }}>
          {loading ? "Generating..." : "Generate Summary"}
        </button>
        {summary && (
          <div style={{ marginTop: 10, padding: 10, background: "#f9f9f9", borderRadius: 6 }}>
            <strong>Summary:</strong>
            <p>{summary}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default AgentChat;