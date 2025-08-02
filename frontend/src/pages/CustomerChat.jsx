import React, { useEffect, useState } from "react";
import { sendMessage, startConnection } from "../services/signalr";

export default function CustomerChat()
{
    const [messages,setMessages] = useState([]);
    const [input, setInput] = useState('');

    useEffect(()=>
    {
        startConnection("chat-123",(sender,message)=>
        {
            setMessages((prev)=> [...prev,{sender,message}]);
        });
    },[]);

    const handleSend = () =>
    {
     let timestamp = Date.now().toString();
        sendMessage('chat-123', 'Customer', input, timestamp);
        setInput('');
        messages.forEach(element => {
          console.log(element);
        });
    }

    return (
	 <div>
      <h2>Customer Chat</h2>
      <div>
        {messages.map((m, i) => (
          <p key={i}><strong>{m.sender}:</strong> {m.message}</p>
        ))}
      </div>
      <input value={input} onChange={(e) => setInput(e.target.value)} />
      <button onClick={handleSend}>Send</button>
    </div>
	);

}

