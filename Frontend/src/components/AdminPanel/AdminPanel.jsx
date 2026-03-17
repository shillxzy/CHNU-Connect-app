import React, { useState, useEffect } from "react";
import "./AdminPanel.css";
import { getAllUsers } from "../../api/userAPI";

export default function AdminPanel() {
  const [search, setSearch] = useState("");
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Використовуємо правильний ключ токена
  const token = localStorage.getItem("accessToken");
  console.log("AdminPanel: accessToken =", token);

  useEffect(() => {
    const fetchUsers = async () => {
      if (!token) {
        console.error("AdminPanel: token відсутній!");
        setError("Не авторизовано: відсутній токен.");
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        console.log("AdminPanel: Отримую користувачів...");
        const data = await getAllUsers(token);
        console.log("AdminPanel: Відповідь API getAllUsers:", data);

        // Перевірка структури
        if (!Array.isArray(data)) {
          console.warn("AdminPanel: data не є масивом, перевірте API!");
          setUsers([]);
        } else {
          setUsers(data);
        }
      } catch (err) {
        console.error("AdminPanel: Помилка при fetchUsers:", err);
        setError(err.response?.data?.message || err.message || "Помилка при завантаженні користувачів");
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, [token]);

  // Фільтрація користувачів
  const filteredUsers = users.filter(user =>
    [user.username, user.fullName, user.email]
      .filter(Boolean)
      .some(field => field.toLowerCase().includes(search.toLowerCase()))
  );

  console.log("AdminPanel: відфільтровані користувачі:", filteredUsers);

  return (
    <div className="admin-panel">
      <div className="sidebar">
        <h2>Панель адміністратора</h2>
        <ul>
          <li>Користувачі</li>
          <li>Пости</li>
          <li>Події</li>
          <li>Коментарі</li>
          <li>Статистика</li>
        </ul>
      </div>

      <div className="content">
        <div className="search-box">
          <input
            type="text"
            placeholder="Ім’я користувача або email"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          <button>🔍</button>
        </div>

        <div className="user-list">
          {loading && <p>Завантаження...</p>}
          {error && <p style={{ color: "red" }}>Помилка: {error}</p>}

          {!loading && !error && filteredUsers.length === 0 && (
            <p>Користувачі не знайдені</p>
          )}

          {!loading && !error && filteredUsers.map(user => (
            <div key={user.id} className="user-item">
              <img
                src={user.photoUrl || "/default-avatar.png"}
                alt="Avatar"
                className="avatar"
              />
              <div className="user-info">
                <span className="user-name">{user.fullName || user.username || user.email}</span>
                <span className="user-email">{user.email}</span>
                <span className="user-faculty">{user.faculty || "-"}, курс {user.course || "-"}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
