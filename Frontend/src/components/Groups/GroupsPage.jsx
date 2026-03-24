import { useEffect, useState, useContext } from "react";
import { getGroups, getGroupById } from "../../api/groupAPI";
import { getSubjectsByGroup } from "../../api/subjectAPI";
import AuthContext from "../../context/AuthContext";
import "./GroupsPage.css";
import { useNavigate } from "react-router-dom";
import UserTooltip from "../ToolTip/UserTooltip";

export default function GroupsPage() {
  const { role, userId: currentUserId } = useContext(AuthContext);
  const navigate = useNavigate();

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [subjects, setSubjects] = useState([]);

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

    // 👇 підтягуємо дисципліни
    const subjectsRes = await getSubjectsByGroup(group.id);
    setSubjects(subjectsRes.data);
  };

  const backToList = () => {
    setSelectedGroup(null);
    setSubjects([]);
  };

  console.log("SUBJECTS:", subjects);


  return (
    <div className="groups-page">

      {/* ===== LIST ===== */}
      {!selectedGroup && (
        <div className="container">

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
                <div className="group-title">{g.name}</div>
                <div className="group-desc">{g.description}</div>

                <div className="group-meta">
                  Куратор: <b>{g.curatorName || "Нікого"}</b>
                </div>
              </div>
            ))}
          </div>

        </div>
      )}

      {/* ===== DETAILS ===== */}
      {selectedGroup && (
        <div className="container">

          <button className="btn-back" onClick={backToList}>
            ← Назад
          </button>

          {role === "admin" && (
            <button
              className="btn-primary"
              onClick={() => navigate(`/groups/edit/${selectedGroup.id}`)}
            >
              Редагувати
            </button>
          )}

          <div className="card">
            <h2>{selectedGroup.name}</h2>
            <p className="desc">{selectedGroup.description}</p>

            {/* ===== CURATOR ===== */}
            <div className="section">
              <h3>Куратор</h3>

              {selectedGroup.curator ? (
                <div className="user-row">
                  <UserTooltip
                    userId={selectedGroup.curator.id}
                    currentUserId={currentUserId}
                    size={40}
                  />
                  <span>{selectedGroup.curator.fullName}</span>
                </div>
              ) : (
                <p className="muted">Нікого</p>
              )}
            </div>

            {/* ===== STUDENTS ===== */}
            <div className="section">
              <h3>Студенти</h3>

              {selectedGroup.users && selectedGroup.users.length > 0 ? (
                <div className="users-list">
                  {selectedGroup.users.map(u => (
                    <div key={u.id} className="user-row">
                      <UserTooltip
                        userId={u.id}
                        currentUserId={currentUserId}
                        size={40}
                      />
                      <span>{u.fullName}</span>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="muted">Немає студентів</p>
              )}
            </div>

            {/* ===== SUBJECTS ===== */}
            <div className="section">
              <h3>Дисципліни</h3>

              {subjects && subjects.length > 0 ? (
                <div className="subjects-list">
                  {subjects.map(s => (
                    <div
  key={s.id}
  className="subject-item"
  onClick={() => {
    if (s.moodleLink) {
      const url = s.moodleLink.startsWith("http")
        ? s.moodleLink
        : `https://${s.moodleLink}`;

      window.open(url, "_blank");
    }
  }}
  style={{ cursor: s.moodleLink ? "pointer" : "default" }}
>
  <div className="subject-info">
    <b>{s.name}</b>
    <span>{s.moodleLink}</span>
  </div>
</div>

                  ))}
                </div>
              ) : (
                <p className="muted">Немає дисциплін</p>
              )}
            </div>

            {/* ===== PLACEHOLDERS ===== */}
            <div className="section muted-box">Розклад</div>
            <div className="section muted-box">Чат групи</div>

          </div>

        </div>
      )}

    </div>
  );
}
