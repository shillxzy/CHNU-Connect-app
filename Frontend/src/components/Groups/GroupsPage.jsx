import { useEffect, useRef, useState, useContext } from 'react';
import {
  getGroups,
  getAllGroups,
  getGroupById,
  addUserToGroup,
} from '../../api/groupAPI';
import { getSubjectsByGroup } from '../../api/subjectAPI';
import { getChatsByUser, createChat } from '../../api/chatAPI';
import { searchUsers } from '../../api/userAPI';
import AuthContext from '../../context/AuthContext';
import './GroupsPage.css';
import { useNavigate, useParams } from 'react-router-dom';
import UserTooltip from '../ToolTip/UserTooltip';

export default function GroupsPage() {
  const { role, user } = useContext(AuthContext);
  const currentUserId = user?.id;
  const navigate = useNavigate();
  const { id: groupIdParam } = useParams();

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [subjects, setSubjects] = useState([]);

  // invite state
  const [showInvite, setShowInvite] = useState(false);
  const [inviteQuery, setInviteQuery] = useState('');
  const [inviteResults, setInviteResults] = useState([]);
  const [inviteFeedback, setInviteFeedback] = useState(null);
  const [inviteBusy, setInviteBusy] = useState(false);
  const searchTimer = useRef(null);

  const isAdmin = role === 'admin' || role === 'superAdmin';
  const isTeacher = role === 'teacher';
  const isTeacherCurator =
    isTeacher && selectedGroup?.curator?.id === currentUserId;
  const canInvite = isAdmin || isTeacherCurator;

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    if (!groupIdParam) {
      setSelectedGroup(null);
      setSubjects([]);
      setShowInvite(false);
      return;
    }
    getGroupById(groupIdParam)
      .then((res) => {
        setSelectedGroup(res.data);
        return getSubjectsByGroup(groupIdParam);
      })
      .then((res) => {
        if (res) {setSubjects(res.data);}
      })
      .catch(() => navigate('/groups'));
  }, [groupIdParam]);

  const load = async () => {
    const res = isAdmin ? await getAllGroups() : await getGroups();
    setGroups(res.data);
  };

  const backToList = () => navigate('/groups');

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
      )
        {memberIds.push(selectedGroup.curator.id);}
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

  const handleInviteSearch = (q) => {
    setInviteQuery(q);
    setInviteFeedback(null);
    clearTimeout(searchTimer.current);
    if (!q.trim()) {
      setInviteResults([]);
      return;
    }
    searchTimer.current = setTimeout(async () => {
      try {
        const res = await searchUsers(q);
        setInviteResults(Array.isArray(res.data) ? res.data : []);
      } catch {
        setInviteResults([]);
      }
    }, 350);
  };

  const handleAddUser = async (targetUser) => {
    setInviteBusy(true);
    setInviteFeedback(null);
    try {
      await addUserToGroup(selectedGroup.id, targetUser.id, 'Student');
      setInviteFeedback({
        type: 'success',
        text: `${targetUser.fullName || targetUser.email} додано до групи!`,
      });
      setInviteResults([]);
      setInviteQuery('');
      // refresh group
      const res = await getGroupById(selectedGroup.id);
      setSelectedGroup(res.data);
    } catch (err) {
      setInviteFeedback({
        type: 'error',
        text: err.response?.data?.message || 'Помилка',
      });
    } finally {
      setInviteBusy(false);
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
                onClick={() => navigate('/groups/' + g.id)}
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

            {canInvite && (
              <button
                onClick={() => {
                  setShowInvite((v) => !v);
                  setInviteFeedback(null);
                }}
                style={{
                  padding: '8px 16px',
                  background: '#0ca678',
                  color: 'white',
                  border: 'none',
                  borderRadius: '8px',
                  cursor: 'pointer',
                  fontWeight: 600,
                }}
              >
                👤 {showInvite ? 'Сховати' : 'Запросити учасника'}
              </button>
            )}
          </div>

          {/* Invite panel */}
          {showInvite && canInvite && (
            <div className="gp-invite-panel">
              <p className="gp-invite-title">Запросити у групу</p>
              <input
                className="gp-invite-input"
                placeholder="Пошук за іменем або email..."
                value={inviteQuery}
                onChange={(e) => handleInviteSearch(e.target.value)}
              />
              {inviteResults.length > 0 && (
                <ul className="gp-invite-list">
                  {inviteResults.map((u) => (
                    <li key={u.id} className="gp-invite-item">
                      <span>{u.fullName || u.email}</span>
                      <button
                        className="gp-invite-btn"
                        onClick={() => handleAddUser(u)}
                        disabled={inviteBusy}
                      >
                        Додати
                      </button>
                    </li>
                  ))}
                </ul>
              )}
              {inviteFeedback && (
                <p
                  className={`gp-invite-feedback gp-invite-feedback--${inviteFeedback.type}`}
                >
                  {inviteFeedback.text}
                </p>
              )}
            </div>
          )}

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
