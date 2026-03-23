import { useEffect, useState, useContext } from "react";
import { getGroups, getGroupById } from "../../api/groupAPI";
import AuthContext from "../../context/AuthContext";
import "./GroupsPage.css";
import { useNavigate } from "react-router-dom";
import Avatar from "../Avatar/Avatar";

export default function GroupsPage() {
  const { role } = useContext(AuthContext);
  const navigate = useNavigate();

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    const res = await getGroups();
    setGroups(res.data);
  };

  const openGroup = async (group) => {
    const res = await getGroupById(group.id);
    setSelectedGroup(res.data);
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

            {role === "admin" && (
              <button
                className="btn-primary"
                onClick={() => navigate("/groups/create")}
              >
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

                <span>
                  Куратор: {g.curatorName || "Немає"}
                </span>
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

          {/* ===== CURATOR ===== */}
          <div className="section">
            <h3>Куратор</h3>

            {selectedGroup.curator ? (
              <div className="user-row">
                <Avatar
                  photoUrl={selectedGroup.curator.photoUrl}
                  size={40}
                />
                <span>{selectedGroup.curator.fullName}</span>
              </div>
            ) : (
              <p>Немає</p>
            )}
          </div>

          <hr />

          {/* ===== STUDENTS ===== */}
          <div className="section">
            <h3>Студенти</h3>

            {selectedGroup.users && selectedGroup.users.length > 0 ? (
              selectedGroup.users.map(u => (
                <div key={u.id} className="user-row">
                  <Avatar
                    photoUrl={u.photoUrl}
                    size={40}
                  />
                  <span>{u.fullName}</span>
                </div>
              ))
            ) : (
              <p>Немає студентів</p>
            )}
          </div>

          <hr />

          {/* ===== PLACEHOLDERS ===== */}
          <div className="section">
            <h3>Дисципліни</h3>
            <p>Тут буде список дисциплін</p>
          </div>

          <hr />

          <div className="section">
            <h3>Розклад</h3>
            <p>Тут буде розклад (drag & drop)</p>
          </div>

          <hr />

          <div className="section">
            <h3>Чат групи</h3>
            <p>Тут буде чат групи</p>
          </div>

        </div>
      )}

    </div>
  );
}
