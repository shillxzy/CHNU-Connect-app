import "./Chats.css";

const chats = [
  { id: 1, name: "ПІБ Викладача", last: "Дякую!", time: "2 хв" },
  { id: 2, name: "ПІБ Викладача", last: "Дякую!", time: "1 хв" },
  { id: 3, name: "Група №### | Назва дисц.", last: "Зрозуміло.", time: "10 хв" },
  { id: 4, name: "Група №### | Назва дисц.", last: "Добрий день!", time: "1 год" },
  { id: 5, name: "Група №### | Назва дисц.", last: "lecture1.txt", time: "1 день" }
];

const messages = [
  { id: 1, text: "Добрий день!", time: "9:54", fromMe: false },
  { id: 2, text: "Дякую!", time: "9:55", fromMe: true }
];

export default function Chats() {
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
              <div key={chat.id} className="chat-item">
                <div className="chat-avatar" />

                <div className="chat-info">
                  <div className="chat-name">{chat.name}</div>
                  <div className="chat-last">
                    Останнє: {chat.last} ({chat.time})
                  </div>
                </div>
              </div>
            ))}
          </div>

        </div>

        {/* Chat Window */}
        <div className="chat-window">

          <div className="chat-header">
            <div className="chat-title">ПІБ Викладача</div>
            <div className="chat-status">Онлайн</div>
          </div>

          <div className="messages">

            {messages.map(msg => (
              <div
                key={msg.id}
                className={`message ${msg.fromMe ? "mine" : ""}`}
              >
                {msg.text}
                <span className="time">{msg.time}</span>
              </div>
            ))}

          </div>

          <div className="chat-input">

            <input
              type="text"
              placeholder="Напишіть повідомлення..."
            />

            <button>Відправити</button>

          </div>

        </div>

      </div>

    
    </div>
  );
}