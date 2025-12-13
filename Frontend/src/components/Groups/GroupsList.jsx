import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getAllGroups } from "../../api/getAllGroupsApi";
import "./Groups.css";

export default function GroupsList() {
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchGroups = async () => {
      try {
        const token = localStorage.getItem("token");
        const data = await getAllGroups(token);
        setGroups(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchGroups();
  }, []);

  if (loading) return <p>Завантаження груп...</p>;
  if (error) return <p>Помилка: {error}</p>;

  return (
    <div className="groups-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <h1 className="page-title">Група / Спільноти</h1>
            <Link to="/groups/create" className="btn-create">
              Створити групу
            </Link>
          </div>

          <h2 className="section-subtitle">Список Груп</h2>

          <div className="groups-list">
            {groups.map((group) => (
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
                <button className="btn-join">Приєднатися</button>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
