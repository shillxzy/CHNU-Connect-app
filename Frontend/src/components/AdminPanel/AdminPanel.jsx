import React, { useState, useEffect } from "react";
import "./AdminPanel.css";
import { getAllUsers } from "../../api/userAPI";

export default function AdminPanel() {
  const [search, setSearch] = useState("");
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        const data = await getAllUsers(token);
        setUsers(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, [token]);

  const filteredUsers = users.filter(user =>
    (user.username || user.fullName || user.email)
      .toLowerCase()
      .includes(search.toLowerCase())
  );

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
          {!loading && !error && filteredUsers.map(user => (
            <div key={user.id} className="user-item">
              <img
                src={user.photoUrl}
                alt="Avatar"
                className="avatar"
              />
              <div className="user-info">
                <span className="user-name">{user.fullName || user.username || user.email}</span>
                <span className="user-email">{user.email}</span>
                <span className="user-faculty">{user.faculty}, курс {user.course}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
