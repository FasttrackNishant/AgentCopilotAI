import './App.css'
import { Routes,Route,Navigate } from 'react-router-dom'
import CustomerChat from './pages/CustomerChat';
import AgentChat from './pages/AgentChat';

function App() {
  return (
		<Routes>
			<Route path="/customer" element={<CustomerChat />}></Route>
			<Route path="/agent" element={<AgentChat />}></Route>
		</Routes>
  );
}

export default App
