import * as signalR from '@microsoft/signalr';

let connection = null;
let currentChatId = null;

export const startConnection = async (chatId, onMessageReceived) => {
	if (
		connection &&
		connection.state === signalR.HubConnectionState.Connected &&
		currentChatId === chatId
	) {
		console.log('Already connected to chat:', chatId);
		return;
	}

	if (!connection) {
		connection = new signalR.HubConnectionBuilder()
			.withUrl('http://localhost:5000/hub/chat')
			.withAutomaticReconnect()
			.build();

		connection.on('ReceiveMessage', (sender, message) => {
			console.log('Received:', sender, message);
			onMessageReceived(sender, message);
		});

		connection.onclose((err) => {
			console.log('SignalR disconnected:', err);
			connection = null;
			currentChatId = null;
		});
	}

	try {
		if (connection.state !== signalR.HubConnectionState.Connected) {
			await connection.start();
			console.log('SignalR connected.');
		}
		await connection.invoke('JoinChat', chatId);
		currentChatId = chatId;
		console.log('Joined group:', chatId);
	} catch (err) {
		console.error('SignalR start/join failed:', err);
	}

	return connection;
};

export const sendMessage = async (chatId, sender, message,timestamp) => {
	if (!connection || connection.state !== 'Connected') {
		console.error('SignalR not connected.');
		return;
	}

	await connection.invoke('SendMessage', chatId, sender, message,timestamp);
};
