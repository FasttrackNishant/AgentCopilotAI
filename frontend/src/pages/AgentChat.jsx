import React, { useEffect, useState } from 'react';
import { sendMessage, startConnection } from '../services/signalr';

export default function AgentChat() {
	const [messages, setMessages] = useState([]);
	const [input, setInput] = useState('');
	const [summary, setSummary] = useState('');

	useEffect(() => {
		startConnection('chat-123', (sender, message) => {
			setMessages((prev) => [...prev, { sender, message }]);
		});
	}, []);

	const handleSend = () => {
    let timestamp = Date.now().toString();
		sendMessage('chat-123', 'Agent', input,timestamp);
		setInput('');

		messages.forEach((element) => {
			console.log(element);
		});
	};

	const fetchSummary = async () => {
		const res = await fetch(
			'http://localhost:5000/summary/chat-123'
		);
		const data = await res.json();
		setSummary(data.summary);
	};

	return (
		<div>
			<h2>Agent Chat</h2>
			<div>
				{messages.map((m, i) => (
					<p key={i}>
						<strong>{m.sender}:</strong> {m.message}
					</p>
				))}
			</div>
			<input value={input} onChange={(e) => setInput(e.target.value)} />
			<button onClick={handleSend}>Send</button>

			<button onClick={fetchSummary}>Summarize Chat</button>

			{summary && (
				<div className="p-3 mt-2 bg-yellow-100 text-gray-900 rounded shadow text-sm">
					<strong>Summary:</strong> {summary}
				</div>
			)}
		</div>
	);
}
