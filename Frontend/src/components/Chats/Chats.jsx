import React, { useEffect, useState, useRef } from "react";
import { getChatsByUser, getMessages, sendMessage, markMessageRead } from "../../api/chatAPI";
import * as signalR from "@microsoft/signalr";
import "./Chats.css";

export default function Chats({ userId }) {
  const [chats, setChats] = useState([]);
  const [selectedChat, setSelectedChat] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [loading, setLoading] = useState(true);
  const connectionRef = useRef(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/chathub", { accessTokenFactory: () => localStorage.getItem("accessToken") })
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => console.log("SignalR connected"))
      .catch(err => console.error("SignalR connection error:", err));

    connection.on("ReceiveMessage", (message) => {
      if (selectedChat && message.chatId === selectedChat.id) {
        setMessages(prev => [...prev, message]);
      }
    });

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [selectedChat]);

  // 2️⃣ Завантаження списку чатів
  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await getChatsByUser(userId);
        setChats(response.data);
      } catch (err) {
        console.error("Помилка завантаження чатів:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchChats();
  }, [userId]);

  useEffect(() => {
  const markMessagesAsRead = async () => {
    if (!selectedChat) return;

    for (const msg of messages) {
      if (msg.senderId !== userId) {
        try {
          await markMessageRead(selectedChat.id, msg.id, userId);
        } catch (err) {
          console.error("Не вдалося відзначити повідомлення як прочитане:", err);
        }
      }
    }
  };

  markMessagesAsRead();
}, [selectedChat, messages]);


  useEffect(() => {
    if (!selectedChat) return;

    const fetchMessages = async () => {
      try {
        const response = await getMessages(selectedChat.id);
        setMessages(response.data);
      } catch (err) {
        console.error("Помилка завантаження повідомлень:", err);
      }
    };

    fetchMessages();

    // Додаємо користувача до групи SignalR
    connectionRef.current?.invoke("JoinChat", selectedChat.id);

    return () => {
      connectionRef.current?.invoke("LeaveChat", selectedChat.id);
    };
  }, [selectedChat]);

  // 4️⃣ Відправка повідомлення
  const handleSendMessage = async () => {
    if (!newMessage.trim() || !selectedChat) return;

    try {
      const response = await sendMessage(selectedChat.id, {
        senderId: userId,
        content: newMessage.trim()
      });
      setMessages(prev => [...prev, response.data]);
      setNewMessage("");
    } catch (err) {
      console.error("Помилка відправки повідомлення:", err);
    }
  };

  if (loading) return <p>Завантаження чатів...</p>;

  return (
    <div className="chat-page">
      <div className="chat-layout">

        {/* Sidebar */}
        <div className="chat-sidebar">
          <div className="chat-sidebar-header">
            <h2>Чати</h2>
            <button className="add-chat">+</button>
          </div>
          <div className="chat-list">
            {chats.map(chat => (
              <div
                key={chat.id}
                className={`chat-item ${selectedChat?.id === chat.id ? "selected" : ""}`}
                onClick={() => setSelectedChat(chat)}
              >
                <div className="chat-avatar" />
                <div className="chat-info">
                  <div className="chat-name">{chat.name}</div>
                  <div className="chat-last">
                    Останнє: {chat.lastMessage || "-"}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Chat Window */}
        <div className="chat-window">
          {selectedChat ? (
            <>
              <div className="chat-header">
                <div className="chat-title">{selectedChat.name}</div>
                <div className="chat-status">Онлайн</div>
              </div>

              <div className="messages">
                {messages.map(msg => (
                  <div key={msg.id} className={`message ${msg.senderId === userId ? "mine" : ""}`}>
                    {msg.content}
                    <span className="time">{new Date(msg.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                  </div>
                ))}
              </div>

              <div className="chat-input">
                <input
                  type="text"
                  placeholder="Напишіть повідомлення..."
                  value={newMessage}
                  onChange={e => setNewMessage(e.target.value)}
                  onKeyDown={e => e.key === "Enter" && handleSendMessage()}
                />
                <button onClick={handleSendMessage}>Відправити</button>
              </div>
            </>
          ) : (
            <p>Оберіть чат для перегляду повідомлень</p>
          )}
        </div>

      </div>
    </div>
  );
}
