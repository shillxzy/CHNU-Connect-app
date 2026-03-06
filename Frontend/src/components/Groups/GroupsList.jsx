import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getGroups, joinGroup } from "../../api/groupAPI";
import "./Groups.css";

export default function GroupsList() {
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [joiningGroupId, setJoiningGroupId] = useState(null);

  // Завантаження всіх груп
  useEffect(() => {
    const fetchGroups = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await getGroups();
        const groupsData = Array.isArray(response.data) ? response.data : [];
        setGroups(groupsData);
      } catch (err) {
        console.error("Error fetching groups:", err);
        setError(err.response?.data?.message || err.message || "Помилка при завантаженні груп");
        setGroups([]);
      } finally {
        setLoading(false);
      }
    };

    fetchGroups();
  }, []);

  // Обробка приєднання до групи
  const handleJoinGroup = async (groupId) => {
    setJoiningGroupId(groupId);
    try {
      await joinGroup(groupId);
      alert("Ви приєдналися до групи!");
      // Оновлюємо кількість учасників локально
      setGroups((prev) =>
        prev.map((g) =>
          g.id === groupId ? { ...g, membersCount: g.membersCount + 1 } : g
        )
      );
    } catch (err) {
      console.error("Error joining group:", err);
      alert(err.response?.data?.message || "Не вдалося приєднатися до групи");
    } finally {
      setJoiningGroupId(null);
    }
  };

  if (loading) return <p>Завантаження груп...</p>;
  if (error) return <p>Помилка: {error}</p>;

  return (
    <div className="groups-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <h1 className="page-title">Групи / Спільноти</h1>
            <Link to="/groups/create" className="btn-create">
              Створити групу
            </Link>
          </div>

          <h2 className="section-subtitle">Список Груп</h2>

          <div className="groups-list">
            {groups.length > 0 ? (
              groups.map((group) => (
                <div className="group-card" key={group.id}>
                  <div className="group-info">
                    <div className="group-field">
                      <strong>Назва:</strong> {group.name}
                    </div>
                    <div className="group-field">
                      <strong>Опис:</strong> {group.description}
                    </div>
                    <div className="group-field">
                      <strong>Учасники:</strong> {group.membersCount}
                    </div>
                  </div>
                  <button
                    className="btn-join"
                    disabled={joiningGroupId === group.id}
                    onClick={() => handleJoinGroup(group.id)}
                  >
                    {joiningGroupId === group.id ? "Приєднання..." : "Приєднатися"}
                  </button>
                </div>
              ))
            ) : (
              <p>Немає груп</p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
