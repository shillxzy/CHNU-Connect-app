import { useEffect, useState, useContext } from "react";
import { getGroups } from "../../api/groupAPI";
import AuthContext from "../../context/AuthContext";
import "./GroupsPage.css";

export default function GroupsPage() {
  const { user } = useContext(AuthContext);

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    const res = await getGroups();
    setGroups(res.data);
  };

  const openGroup = (group) => {
    setSelectedGroup(group);
  };

  const backToList = () => {
    setSelectedGroup(null);
  };

  return (
    <div className="groups-page">

      {/* ===== LIST ===== */}
      {!selectedGroup && (
        <div className="groups-container">

          <div className="page-header">
            <h1>Групи</h1>

            {user?.role === "admin" && (
              <button className="btn-primary">
                + Створити групу
              </button>
            )}
          </div>

          <div className="groups-list">
            {groups.map((g) => (
              <div
                key={g.id}
                className="group-card"
                onClick={() => openGroup(g)}
              >
                <strong>{g.name}</strong>
                <p>{g.description}</p>
                <span>Куратор: {g.curatorName || "Немає"}</span>
              </div>
            ))}
          </div>

        </div>
      )}

      {/* ===== DETAILS ===== */}
      {selectedGroup && (
        <div className="group-details">

          <button className="btn-back" onClick={backToList}>
            ← Назад
          </button>

          <h2>{selectedGroup.name}</h2>

          <p><b>Опис:</b> {selectedGroup.description}</p>
          <p><b>Куратор:</b> {selectedGroup.curatorName || "Немає"}</p>

          <hr />

          {/* 👥 STUDENTS */}
          <div className="section">
            <h3>Студенти</h3>

            <div className="placeholder">
              {/* тут має бути список студентів */}
              <p>Тут буде список студентів групи</p>
            </div>
          </div>

          <hr />

          {/* 📚 SUBJECTS */}
          <div className="section">
            <h3>Дисципліни</h3>

            <div className="placeholder">
              {/* тут має бути список дисциплін */}
              <p>Тут буде список дисциплін</p>
            </div>
          </div>

          <hr />

          {/* 📅 SCHEDULE */}
          <div className="section">
            <h3>Розклад</h3>

            <div className="placeholder">
              {/* тут має бути таблиця розкладу */}
              <p>Тут буде розклад (drag & drop)</p>
            </div>
          </div>

          <hr />

          {/* 💬 CHAT */}
          <div className="section">
            <h3>Чат групи</h3>

            <div className="placeholder">
              {/* тут має бути чат */}
              <p>Тут буде чат групи</p>
            </div>
          </div>

        </div>
      )}

    </div>
  );
}
