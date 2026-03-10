import React, { useEffect, useState, useRef, useContext } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getChatsByUser, getMessages, sendMessage, markMessageRead } 
  from "../../api/chatAPI";
import * as signalR from "@microsoft/signalr";
import "./Chats.css";
import AuthContext from "../../context/AuthContext";

export default function Chats() {
  const { user } = useContext(AuthContext);
  const userId = user?.id;

  const { chatId } = useParams();
  const navigate = useNavigate();

  const [chats, setChats] = useState([]);
  const [selectedChat, setSelectedChat] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState("");
  const [loading, setLoading] = useState(true);

  const connectionRef = useRef(null);
  const selectedChatRef = useRef(null);
  const messagesEndRef = useRef(null);

  // тримаємо актуальний чат
  useEffect(() => {
    selectedChatRef.current = selectedChat;
  }, [selectedChat]);

  // SignalR connection
  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/chat", {
        accessTokenFactory: () => localStorage.getItem("accessToken")
      })
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveMessage", (message) => {
      const chat = selectedChatRef.current;
      if (!chat || message.chatId !== chat.id) return;

      setMessages(prev => {
        const newMessages = [...prev, message];
        newMessages.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
        return newMessages;
      });
    });

    const startConnection = async () => {
      try {
        await connection.start();
        console.log("SignalR connected");
      } catch (err) {
        console.error("SignalR connection error:", err);
      }
    };

    startConnection();
    connectionRef.current = connection;

    return () => connection.stop().catch(() => {});
  }, []);

  // Завантаження чатів
  useEffect(() => {
    if (!userId) return;

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

  // Вибір чату з URL
  useEffect(() => {
    if (!chatId || chats.length === 0) return;

    const chat = chats.find(c => c.id === parseInt(chatId));
    if (chat) setSelectedChat(chat);
  }, [chatId, chats]);

  // Завантаження повідомлень
  useEffect(() => {
    if (!selectedChat) return;

    const fetchMessages = async () => {
      try {
        const response = await getMessages(selectedChat.id);
        const sorted = response.data.sort(
          (a, b) => new Date(a.createdAt) - new Date(b.createdAt)
        );
        setMessages(sorted);
      } catch (err) {
        console.error("Помилка завантаження повідомлень:", err);
      }
    };

    fetchMessages();

    const joinGroup = async () => {
      try {
        if (connectionRef.current?.state === "Connected") {
          await connectionRef.current.invoke("JoinChat", selectedChat.id);
        }
      } catch (err) {
        console.error("JoinChat error:", err);
      }
    };

    joinGroup();

    return () => {
      connectionRef.current?.invoke("LeaveChat", selectedChat.id).catch(() => {});
    };
  }, [selectedChat]);

  // Відправка повідомлення
  const handleSendMessage = async () => {
    if (!newMessage.trim() || !selectedChat) return;

    try {
      await sendMessage(selectedChat.id, {
        chatId: selectedChat.id,
        senderId: userId,
        content: newMessage.trim()
      });
      setNewMessage("");
      // локально не додаємо — SignalR сам додасть
    } catch (err) {
      console.error("Помилка відправки повідомлення:", err);
    }
  };

  // Відзначення повідомлень як прочитаних
  useEffect(() => {
    if (!selectedChat) return;

    const markMessagesAsRead = async () => {
      for (const msg of messages) {
        if (msg.senderId !== userId) {
          try {
            await markMessageRead(selectedChat.id, msg.id, userId);
          } catch (err) {
            console.error("Не вдалося відзначити повідомлення:", err);
          }
        }
      }
    };

    markMessagesAsRead();
  }, [messages, selectedChat, userId]);

  // Автоскрол вниз
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

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
                onClick={() => navigate(`/chats/${chat.id}`)}
              >
                <div className="chat-avatar" />
                <div className="chat-info">
                  <div className="chat-name">{chat.name || chat.title || "Без назви"}</div>
                  <div className="chat-last">Останнє: {chat.lastMessage || "-"}</div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Chat window */}
        <div className="chat-window">
          {selectedChat ? (
            <>
              <div className="chat-header">
                <div className="chat-title">{selectedChat.name || selectedChat.title || "Без назви"}</div>
                <div className="chat-status">Онлайн</div>
              </div>

              <div className="messages">
                {messages.map(msg => (
                  <div
                    key={msg.id}
                    className={`message ${msg.senderId === userId ? "mine" : ""}`}
                  >
                    {msg.content}
                    <span className="time">
                      {new Date(msg.createdAt).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
                    </span>
                  </div>
                ))}
                <div ref={messagesEndRef} />
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
