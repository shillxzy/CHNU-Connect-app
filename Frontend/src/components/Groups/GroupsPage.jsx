import { useEffect, useState, useContext } from 'react';
import { getGroups, getGroupById } from '../../api/groupAPI';
import { getSubjectsByGroup } from '../../api/subjectAPI';
import { getChatsByUser, createChat } from '../../api/chatAPI';
import AuthContext from '../../context/AuthContext';
import './GroupsPage.css';
import { useNavigate } from 'react-router-dom';
import UserTooltip from '../ToolTip/UserTooltip';

export default function GroupsPage() {
  const { role, user } = useContext(AuthContext);
  const currentUserId = user?.id;
  const navigate = useNavigate();

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [subjects, setSubjects] = useState([]);

  const isAdmin = role === 'admin' || role === 'superAdmin';
  const isTeacher = role === 'teacher';
  const isTeacherCurator =
    isTeacher && selectedGroup?.curator?.id === currentUserId;

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
    const subjectsRes = await getSubjectsByGroup(group.id);
    setSubjects(subjectsRes.data);
  };

  const backToList = () => {
    setSelectedGroup(null);
    setSubjects([]);
  };

  const openGroupChat = async () => {
    try {
      const res = await getChatsByUser(currentUserId);
      const existing = (res.data || []).find(
        (c) => c.type === 'group' && c.title === selectedGroup.name,
      );
      if (existing) {
        navigate(`/chats/${existing.id}`);
        return;
      }
      const memberIds = [
        currentUserId,
        ...(selectedGroup.users || []).map((u) => u.id),
      ];
      if (
        selectedGroup.curator &&
        !memberIds.includes(selectedGroup.curator.id)
      ) {
        memberIds.push(selectedGroup.curator.id);
      }
      const newChat = await createChat({
        type: 'group',
        title: selectedGroup.name,
        createdBy: currentUserId,
        memberIds: [...new Set(memberIds)],
      });
      navigate(`/chats/${newChat.data.id}`);
    } catch {
      alert('Помилка відкриття чату групи');
    }
  };

  return (
    <div className="groups-page">
      {/* ===== LIST ===== */}
      {!selectedGroup && (
        <div className="container">
          <div className="page-header">
            <h1>Групи</h1>
            {isAdmin && (
              <button
                className="btn-primary"
                onClick={() => navigate('/groups/create')}
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
                  Куратор: <b>{g.curator?.fullName || 'Нікого'}</b>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* ===== DETAILS ===== */}
      {selectedGroup && (
        <div className="container">
          <div
            style={{
              display: 'flex',
              gap: '10px',
              marginBottom: '16px',
              flexWrap: 'wrap',
            }}
          >
            <button className="btn-back" onClick={backToList}>
              ← Назад
            </button>

            {isAdmin && (
              <button
                className="btn-primary"
                onClick={() => navigate('/groups/edit/' + selectedGroup.id)}
              >
                ✏️ Редагувати групу
              </button>
            )}

            {(isAdmin || isTeacherCurator) && (
              <button
                onClick={() =>
                  navigate('/group/edit/schedule/' + selectedGroup.id)
                }
                style={{
                  padding: '8px 16px',
                  background: '#6610f2',
                  color: 'white',
                  border: 'none',
                  borderRadius: '8px',
                  cursor: 'pointer',
                  fontWeight: 600,
                }}
              >
                📅 Редагувати розклад
              </button>
            )}

            {/* Кнопка перегляду розкладу для всіх */}
            <button
              onClick={() => navigate('/group/schedule/' + selectedGroup.id)}
              style={{
                padding: '8px 16px',
                background: '#0d6efd',
                color: 'white',
                border: 'none',
                borderRadius: '8px',
                cursor: 'pointer',
                fontWeight: 600,
              }}
            >
              📋 Переглянути розклад
            </button>
          </div>

          <div className="card">
            <h2>{selectedGroup.name}</h2>
            <p className="desc">{selectedGroup.description}</p>

            {/* CURATOR */}
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

            {/* STUDENTS */}
            <div className="section">
              <h3>Студенти</h3>
              {selectedGroup.users && selectedGroup.users.length > 0 ? (
                <div className="users-list">
                  {selectedGroup.users.map((u) => (
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

            {/* SUBJECTS */}
            <div className="section">
              <h3>Дисципліни</h3>
              {subjects && subjects.length > 0 ? (
                <div className="subjects-list">
                  {subjects.map((s) => (
                    <div
                      key={s.id}
                      className="subject-item"
                      onClick={() => {
                        if (s.moodleLink) {
                          const url = s.moodleLink.startsWith('http')
                            ? s.moodleLink
                            : 'https://' + s.moodleLink;
                          window.open(url, '_blank');
                        }
                      }}
                      style={{ cursor: s.moodleLink ? 'pointer' : 'default' }}
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

            <div className="section">
              <button
                onClick={openGroupChat}
                style={{
                  padding: '10px 20px',
                  background: '#0d6efd',
                  color: 'white',
                  border: 'none',
                  borderRadius: '8px',
                  cursor: 'pointer',
                  fontWeight: 600,
                  fontSize: '15px',
                }}
              >
                💬 Відкрити чат групи
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
