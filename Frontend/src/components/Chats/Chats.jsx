import React, { useEffect, useState, useRef, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  getChatsByUser,
  getMessages,
  sendMessage,
  markMessageRead,
  updateMessage,
  deleteMessage,
  createChat,
} from '../../api/chatAPI';
import { getAllUsers } from '../../api/userAPI';
import * as signalR from '@microsoft/signalr';
import './Chats.css';
import AuthContext from '../../context/AuthContext';
import Avatar from '../Avatar/Avatar';
import Loading from '../Loading/Loading';

export default function Chats() {
  const { user, isOnline } = useContext(AuthContext);
  const userId = user?.id;

  const { chatId } = useParams();
  const navigate = useNavigate();

  const [chats, setChats] = useState([]);
  const [selectedChat, setSelectedChat] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [editingMessageId, setEditingMessageId] = useState(null);
  const [editContent, setEditContent] = useState('');

  const [showGroupModal, setShowGroupModal] = useState(false);
  const [groupTitle, setGroupTitle] = useState('');
  const [selectedMembers, setSelectedMembers] = useState([]);
  const [allUsers, setAllUsers] = useState([]);
  const [modalLoading, setModalLoading] = useState(false);

  const canCreateGroup = ['teacher', 'admin', 'superAdmin'].includes(
    user?.role,
  );

  const connectionRef = useRef(null);
  const selectedChatRef = useRef(null);
  const messagesEndRef = useRef(null);

  useEffect(() => {
    selectedChatRef.current = selectedChat;
  }, [selectedChat]);

  /* ======================== DISPLAY HELPERS ======================== */

  const getChatDisplayInfo = (chat) => {
    if (chat.type === 'private' && chat.members?.length) {
      const other =
        chat.members.find((m) => m.userId !== userId) || chat.members[0];
      return {
        name: other.authorName || 'Користувач',
        avatar: other.authorAvatar,
        userId: other.userId,
      };
    }
    return { name: chat.title || 'Груповий чат', avatar: null, userId: null };
  };

  /* ======================== SIGNALR ======================== */

  useEffect(() => {
    const API_URL = import.meta.env.VITE_API_BASE_URL;
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/hubs/chat`, {
        accessTokenFactory: () => localStorage.getItem('accessToken'),
      })
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (message) => {
      const chat = selectedChatRef.current;
      if (!chat || message.chatId !== chat.id) {
        // Оновити lastMessage в списку чатів
        setChats((prev) =>
          prev.map((c) =>
            c.id === message.chatId
              ? { ...c, lastMessage: message.content }
              : c,
          ),
        );
        return;
      }
      setMessages((prev) => {
        const updated = [...prev, message];
        return updated.sort(
          (a, b) => new Date(a.createdAt) - new Date(b.createdAt),
        );
      });
    });

    // Реалтайм редагування
    connection.on('MessageUpdated', (updated) => {
      setMessages((prev) =>
        prev.map((m) => (m.id === updated.id ? updated : m)),
      );
    });

    // Реалтайм видалення
    connection.on('MessageDeleted', (deletedId) => {
      setMessages((prev) => prev.filter((m) => m.id !== deletedId));
    });

    connection
      .start()
      .then(async () => {
        await connection.invoke('SubscribeToNotifications').catch(() => {});
      })
      .catch((err) => console.error('SignalR error:', err));

    connectionRef.current = connection;
    return () => connection.stop().catch(() => {});
  }, []);

  /* ======================== LOAD CHATS ======================== */

  useEffect(() => {
    if (!userId) {return;}
    const fetch = async () => {
      try {
        const res = await getChatsByUser(userId);
        setChats(res.data);
      } catch (err) {
        console.error('Chat load error:', err);
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, [userId]);

  /* ======================== SELECT CHAT ======================== */

  useEffect(() => {
    if (!chatId || chats.length === 0) {return;}
    const chat = chats.find((c) => c.id === parseInt(chatId));
    if (chat) {setSelectedChat(chat);}
  }, [chatId, chats]);

  /* ======================== LOAD MESSAGES ======================== */

  useEffect(() => {
    if (!selectedChat) {return;}
    const fetch = async () => {
      try {
        const res = await getMessages(selectedChat.id);
        setMessages(
          res.data.sort(
            (a, b) => new Date(a.createdAt) - new Date(b.createdAt),
          ),
        );
      } catch (err) {
        console.error('Messages load error:', err);
      }
    };
    fetch();

    const join = async () => {
      if (connectionRef.current?.state === 'Connected') {
        await connectionRef.current
          .invoke('JoinChat', selectedChat.id)
          .catch(() => {});
      }
    };
    join();

    return () => {
      connectionRef.current
        ?.invoke('LeaveChat', selectedChat.id)
        .catch(() => {});
    };
  }, [selectedChat]);

  /* ======================== SEND ======================== */

  const handleSend = async () => {
    if (!newMessage.trim() || !selectedChat) {return;}
    try {
      await sendMessage(selectedChat.id, {
        chatId: selectedChat.id,
        senderId: userId,
        content: newMessage.trim(),
      });
      setNewMessage('');
    } catch (err) {
      console.error('Send error:', err);
    }
  };

  /* ======================== READ ======================== */

  useEffect(() => {
    if (!selectedChat || messages.length === 0) {return;}
    const mark = async () => {
      for (const msg of messages) {
        if (msg.senderId !== userId) {
          await markMessageRead(selectedChat.id, msg.id, userId).catch(
            () => {},
          );
        }
      }
    };
    mark();
  }, [messages, selectedChat, userId]);

  /* ======================== SCROLL ======================== */

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  /* ======================== EDIT / DELETE ======================== */

  const handleUpdate = async (messageId) => {
    if (!editContent.trim()) {return;}
    try {
      await updateMessage(selectedChat.id, messageId, editContent.trim());
      setMessages((prev) =>
        prev.map((m) =>
          m.id === messageId ? { ...m, content: editContent } : m,
        ),
      );
      setEditingMessageId(null);
      setEditContent('');
    } catch (err) {
      console.error('Update error:', err);
    }
  };

  const handleDelete = async (messageId) => {
    try {
      await deleteMessage(selectedChat.id, messageId);
      setMessages((prev) => prev.filter((m) => m.id !== messageId));
    } catch (err) {
      console.error('Delete error:', err);
    }
  };

  /* ======================== GROUP CHAT ======================== */

  const openGroupModal = async () => {
    setGroupTitle('');
    setSelectedMembers([]);
    setShowGroupModal(true);
    try {
      const res = await getAllUsers();
      setAllUsers(res.data.filter((u) => u.id !== userId));
    } catch (err) {
      console.error('Users load error:', err);
    }
  };

  const toggleMember = (id) => {
    setSelectedMembers((prev) =>
      prev.includes(id) ? prev.filter((m) => m !== id) : [...prev, id],
    );
  };

  const handleCreateGroup = async () => {
    if (!groupTitle.trim() || selectedMembers.length === 0) {return;}
    setModalLoading(true);
    try {
      const res = await createChat({
        type: 'group',
        title: groupTitle.trim(),
        createdBy: userId,
        memberIds: [userId, ...selectedMembers],
      });
      setChats((prev) => [res.data, ...prev]);
      setShowGroupModal(false);
      navigate(`/chats/${res.data.id}`);
    } catch (err) {
      console.error('Create group error:', err);
    } finally {
      setModalLoading(false);
    }
  };

  if (loading) {return <Loading />;}

  const selectedDisplay = selectedChat
    ? getChatDisplayInfo(selectedChat)
    : null;
  const isOtherOnline = selectedDisplay?.userId
    ? isOnline(selectedDisplay.userId)
    : false;

  return (
    <div className="chat-page">
      <div className="chat-layout">
        {/* ========== SIDEBAR ========== */}
        <div className="chat-sidebar">
          <div className="chat-sidebar-header">
            <h2>Чати</h2>
            {canCreateGroup && (
              <button className="btn-create-group" onClick={openGroupModal}>
                + Груповий чат
              </button>
            )}
          </div>

          <div className="chat-list">
            {chats.length === 0 && (
              <p
                style={{
                  color: '#888',
                  fontSize: '14px',
                  textAlign: 'center',
                  marginTop: '20px',
                }}
              >
                Немає чатів
              </p>
            )}
            {chats.map((chat) => {
              const display = getChatDisplayInfo(chat);
              const online = display.userId ? isOnline(display.userId) : false;
              const isActive = selectedChat?.id === chat.id;

              return (
                <div
                  key={chat.id}
                  className={`chat-item ${isActive ? 'selected' : ''}`}
                  onClick={() => navigate(`/chats/${chat.id}`)}
                >
                  <div className="chat-avatar-wrap">
                    <Avatar photoUrl={display.avatar} size={42} />
                    {online && <span className="online-dot" />}
                  </div>
                  <div className="chat-info">
                    <div className="chat-name">{display.name}</div>
                    <div className="chat-last">
                      {chat.lastMessage || 'Немає повідомлень'}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* ========== CHAT WINDOW ========== */}
        <div className="chat-window">
          {selectedChat ? (
            <>
              {/* Header */}
              <div className="chat-header">
                <div className="chat-avatar-wrap">
                  <Avatar photoUrl={selectedDisplay?.avatar} size={40} />
                  {isOtherOnline && <span className="online-dot" />}
                </div>
                <div>
                  <div className="chat-title">{selectedDisplay?.name}</div>
                  <div
                    className={`chat-status ${isOtherOnline ? 'online' : 'offline'}`}
                  >
                    {isOtherOnline ? '● Онлайн' : '○ Офлайн'}
                  </div>
                </div>
              </div>

              {/* Messages */}
              <div className="messages">
                {messages.map((msg, index) => {
                  const msgDate = new Date(msg.createdAt);
                  const prevDate =
                    index > 0 ? new Date(messages[index - 1].createdAt) : null;
                  const showDate =
                    !prevDate ||
                    msgDate.toDateString() !== prevDate.toDateString();

                  return (
                    <React.Fragment key={msg.id}>
                      {showDate && (
                        <div className="date-divider">
                          {msgDate.toLocaleDateString('uk-UA', {
                            weekday: 'long',
                            day: 'numeric',
                            month: 'long',
                          })}
                        </div>
                      )}

                      <div
                        className={`message ${msg.senderId === userId ? 'mine' : ''}`}
                      >
                        {editingMessageId === msg.id ? (
                          <div className="edit-wrap">
                            <input
                              value={editContent}
                              onChange={(e) => setEditContent(e.target.value)}
                              onKeyDown={(e) =>
                                e.key === 'Enter' && handleUpdate(msg.id)
                              }
                              autoFocus
                            />
                            <div className="message-actions">
                              <button onClick={() => handleUpdate(msg.id)}>
                                ✓ Зберегти
                              </button>
                              <button onClick={() => setEditingMessageId(null)}>
                                ✗ Скасувати
                              </button>
                            </div>
                          </div>
                        ) : (
                          <>
                            <div className="message-content">{msg.content}</div>
                            {msg.editedAt && (
                              <span className="edited-label">редаговано</span>
                            )}

                            <div className="message-footer">
                              <span className="time">
                                {msgDate.toLocaleTimeString([], {
                                  hour: '2-digit',
                                  minute: '2-digit',
                                })}
                              </span>
                            </div>

                            {msg.senderId === userId && (
                              <div className="message-actions">
                                <button
                                  onClick={() => {
                                    setEditingMessageId(msg.id);
                                    setEditContent(msg.content);
                                  }}
                                >
                                  ✏️
                                </button>
                                <button onClick={() => handleDelete(msg.id)}>
                                  🗑️
                                </button>
                              </div>
                            )}
                          </>
                        )}
                      </div>
                    </React.Fragment>
                  );
                })}
                <div ref={messagesEndRef} />
              </div>

              {/* Input */}
              <div className="chat-input">
                <input
                  type="text"
                  placeholder="Написати повідомлення..."
                  value={newMessage}
                  onChange={(e) => setNewMessage(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleSend()}
                />
                <button onClick={handleSend}>Відправити</button>
              </div>
            </>
          ) : (
            <div className="chat-empty">
              <div>💬</div>
              <p>Оберіть чат зі списку</p>
            </div>
          )}
        </div>
      </div>

      {showGroupModal && (
        <div className="modal-overlay" onClick={() => setShowGroupModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h3>Новий груповий чат</h3>
            <input
              type="text"
              placeholder="Назва групи"
              value={groupTitle}
              onChange={(e) => setGroupTitle(e.target.value)}
            />
            <div className="modal-members-list">
              {allUsers.map((u) => (
                <label key={u.id} className="modal-member-item">
                  <input
                    type="checkbox"
                    checked={selectedMembers.includes(u.id)}
                    onChange={() => toggleMember(u.id)}
                  />
                  <Avatar photoUrl={u.photoUrl} size={30} />
                  <span className="modal-member-name">
                    {u.fullName || u.email}
                  </span>
                </label>
              ))}
            </div>
            <div className="modal-actions">
              <button
                className="btn-cancel"
                onClick={() => setShowGroupModal(false)}
              >
                Скасувати
              </button>
              <button
                className={`btn-submit${!modalLoading && groupTitle.trim() && selectedMembers.length > 0 ? ' ready' : ''}`}
                onClick={handleCreateGroup}
                disabled={
                  modalLoading ||
                  !groupTitle.trim() ||
                  selectedMembers.length === 0
                }
              >
                {modalLoading ? 'Створення...' : 'Створити'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
