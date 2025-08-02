import React, { useEffect, useState,useRef } from "react";
import { sendMessage, startConnection } from "../services/signalr";

export default function CustomerChat()
{
  const [messages, setMessages] = useState([]);
    const [input, setInput] = useState("");
    const chatId = "chat-123";
    const messageEndRef = useRef(null);
  
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
      sendMessage(chatId, "Customer", input);
      setInput("");
    };
  
    return (
      <div style={{ maxWidth: 600, margin: "auto", padding: 20 }}>
        <h2>Customer Chat</h2>
        <div style={{ border: "1px solid #ccc", padding: 10, height: 400, overflowY: "auto", borderRadius: 8 }}>
          {messages.map((m, i) => (
            <div key={i} style={{ marginBottom: 10 }}>
              <strong>{m.sender}:</strong> {m.message}
             <div style={{ fontSize: 10, color: "#999" }}>
  {m.timestamp ? new Date(m.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : "Invalid time"}
</div>
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
      </div>
    );
}

