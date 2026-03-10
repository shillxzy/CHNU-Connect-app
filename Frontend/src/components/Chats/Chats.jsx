import React, { useEffect, useState, useRef, useContext } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getChatsByUser, getMessages, sendMessage, markMessageRead } from "../../api/chatAPI";
import * as signalR from "@microsoft/signalr";
import "./Chats.css";
import AuthContext from "../../context/AuthContext";
import Avatar from "../Avatar/Avatar";

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

  useEffect(() => {
    selectedChatRef.current = selectedChat;
  }, [selectedChat]);

  /* ========================
      CHAT DISPLAY INFO
  ======================== */

  const getChatDisplayInfo = (chat) => {
  console.log("=== getChatDisplayInfo ===");
  console.log(chat.members); // дивимось масив member
  if (chat.type === "private" && chat.members) {
    const otherUser = chat.members.find(m => m.userId !== userId);
    console.log("Other user:", otherUser); // дивимось AuthorName, AuthorAvatar
    if (!otherUser) {
      return { name: "Unknown", avatar: null };
    }
    return {
      name: otherUser.AuthorName || "Unknown",
      avatar: otherUser.AuthorAvatar || null
    };
  }

  return {
    name: chat.title || "Group chat",
    avatar: null
  };
};


  /* ========================
      SIGNALR
  ======================== */

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
        console.error("SignalR error:", err);
      }
    };

    startConnection();
    connectionRef.current = connection;

    return () => connection.stop().catch(() => {});
  }, []);

  /* ========================
      LOAD CHATS
  ======================== */

  useEffect(() => {

    if (!userId) return;

    const fetchChats = async () => {

      try {
        const response = await getChatsByUser(userId);
        setChats(response.data);
      } catch (err) {
        console.error("Chat load error:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchChats();
  }, [userId]);

  /* ========================
      SELECT CHAT
  ======================== */

  useEffect(() => {
  if (!chatId || chats.length === 0) return;
  const chat = chats.find(c => c.id === parseInt(chatId));
  if (chat) {
    console.log("=== Selected chat from list ===");
    console.log(chat); // ← дивимось members, lastMessage, title
    setSelectedChat(chat);
  }
}, [chatId, chats]);


  /* ========================
      LOAD MESSAGES
  ======================== */

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
        console.error("Messages load error:", err);
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

  /* ========================
      SEND MESSAGE
  ======================== */

  const handleSendMessage = async () => {

    if (!newMessage.trim() || !selectedChat) return;

    try {
      await sendMessage(selectedChat.id, {
        chatId: selectedChat.id,
        senderId: userId,
        content: newMessage.trim()
      });
      setNewMessage("");
    } catch (err) {
      console.error("Send error:", err);
    }

  };

  /* ========================
      READ MESSAGES
  ======================== */

  useEffect(() => {

    if (!selectedChat) return;

    const markMessagesAsRead = async () => {
      for (const msg of messages) {
        if (msg.senderId !== userId) {
          try {
            await markMessageRead(
              selectedChat.id,
              msg.id,
              userId
            );
          } catch (err) {
            console.error("Read error:", err);
          }
        }
      }
    };
    markMessagesAsRead();
  }, [messages, selectedChat, userId]);

  /* ========================
      AUTO SCROLL
  ======================== */

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({
      behavior: "smooth"
    });
  }, [messages]);
  if (loading) return <p>Загрузка чату...</p>;
  const selectedDisplay = selectedChat
    ? getChatDisplayInfo(selectedChat)
    : null;

  return (
    <div className="chat-page">
      <div className="chat-layout">
        {/* SIDEBAR */}
        <div className="chat-sidebar">
          <div className="chat-sidebar-header">
            <h2>Чати</h2>
            <button className="add-chat">+</button>
          </div>
          <div className="chat-list">
            {chats.map(chat => {
              const display = getChatDisplayInfo(chat);
              return (
                <div
                  key={chat.id}
                  className={`chat-item ${selectedChat?.id === chat.id ? "selected" : ""}`}
                  onClick={() => navigate(`/chats/${chat.id}`)}
                >
                  <div className="chat-avatar">
                    <Avatar
                      photoUrl={display.avatar}
                      size={42}
                    />
                  </div>
                  <div className="chat-info">
                    <div className="chat-name">
                      {display.name}
                    </div>
                    <div className="chat-last">
                      {chat.lastMessage || "Немає повідомлень"}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
        {/* CHAT WINDOW */}
        <div className="chat-window">
          {selectedChat ? (
            <>
              <div className="chat-header">
                <Avatar
                  photoUrl={selectedDisplay?.avatar}
                  size={40}
                />
                <div>
                  <div className="chat-title">
                    {selectedDisplay?.name}
                  </div>
                  <div className="chat-status">
                    Онлайн
                  </div>
                </div>
              </div>
              <div className="messages">
                {messages.map(msg => (
                  <div
                    key={msg.id}
                    className={`message ${msg.senderId === userId ? "mine" : ""}`}
                  >
                    {msg.content}
                    <span className="time">
                      {new Date(msg.createdAt)
                        .toLocaleTimeString([], {
                          hour: "2-digit",
                          minute: "2-digit"
                        })}
                    </span>
                  </div>
                ))}
                <div ref={messagesEndRef} />
              </div>
              <div className="chat-input">
                <input
                  type="text"
                  placeholder="Написати повідомлення..."
                  value={newMessage}
                  onChange={e => setNewMessage(e.target.value)}
                  onKeyDown={e => e.key === "Enter" && handleSendMessage()}
                />
                <button onClick={handleSendMessage}>
                  Відправити
                </button>
              </div>
            </>
          ) : (
            <p>Обрати чат</p>
          )}
        </div>
      </div>
    </div>
  );
}
