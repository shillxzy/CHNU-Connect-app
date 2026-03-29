import { useEffect, useState, React } from 'react';
import { createGroup, assignCurator, addUserToGroup } from '../../api/groupAPI';
import { getAllUsers } from '../../api/userAPI';
import { useNavigate } from 'react-router-dom';
import Avatar from '../Avatar/Avatar';
import './GroupCreate.css';

export default function GroupCreate() {
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');

  const [users, setUsers] = useState([]);

  const [mode, setMode] = useState('teacher'); // teacher | student
  const [search, setSearch] = useState('');
  const [debounced, setDebounced] = useState('');

  const [selectedCurator, setSelectedCurator] = useState(null);
  const [selectedStudents, setSelectedStudents] = useState([]);

  /* ================= LOAD ================= */
  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    const res = await getAllUsers();
    setUsers(res.data);
  };

  /* ================= DEBOUNCE ================= */
  useEffect(() => {
    const t = setTimeout(() => setDebounced(search), 300);
    return () => clearTimeout(t);
  }, [search]);

  /* ================= RESET SEARCH ================= */
  useEffect(() => {
    setSearch('');
  }, [mode]);

  /* ================= FILTER ================= */
  const filteredUsers = users
    .filter((u) => (u.role || '').toLowerCase() === mode)
    .filter((u) =>
      (u.fullName || '').toLowerCase().includes(debounced.toLowerCase()),
    );

  console.log('USERS:', users);
  console.log('MODE:', mode);
  console.log('FILTERED:', filteredUsers);

  /* ================= ACTIONS ================= */
  const addStudent = (user) => {
    if (!selectedStudents.some((s) => s.id === user.id)) {
      setSelectedStudents((prev) => [...prev, user]);
    }
  };

  const removeStudent = (id) => {
    setSelectedStudents((prev) => prev.filter((s) => s.id !== id));
  };

  const handleCreate = async () => {
    if (!name.trim()) {
      return alert('Введи назву');
    }

    try {
      const res = await createGroup({ name, description });
      const groupId = res.data.id;

      if (selectedCurator) {
        await assignCurator(groupId, selectedCurator.id);
      }

      await Promise.all(
        selectedStudents.map((s) => addUserToGroup(groupId, s.id, 'student')),
      );

      navigate('/groups');
    } catch (err) {
      console.error(err);
      alert('Помилка створення групи');
    }
  };

  /* ================= RENDER USER ================= */
  const renderUser = (user, onClick, isSelected) => (
    <div
      key={user.id}
      className={`user-card ${isSelected ? 'selected' : ''}`}
      onClick={() => onClick(user)}
    >
      <Avatar photoUrl={user.photoUrl} size={42} />

      <div className="user-info">
        <div className="name">{user.fullName}</div>
        <div className="role">{user.role}</div>
      </div>
    </div>
  );

  return (
    <div className="create-wrapper">
      <div className="create-card">
        <h2>Створення групи</h2>

        <input
          placeholder="Назва групи"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />

        <textarea
          placeholder="Опис"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
        />

        {/* ================= MODE ================= */}
        <div className="mode-switch">
          <button
            className={mode === 'teacher' ? 'active' : ''}
            onClick={() => setMode('teacher')}
          >
            Куратор
          </button>

          <button
            className={mode === 'student' ? 'active' : ''}
            onClick={() => setMode('student')}
          >
            Студенти
          </button>
        </div>

        {/* ================= SEARCH ================= */}
        <input
          className="search"
          placeholder="Пошук..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        {/* ================= LIST ================= */}
        <div className="list">
          {filteredUsers.length > 0 ? (
            filteredUsers.map((u) =>
              mode === 'teacher'
                ? renderUser(
                    u,
                    setSelectedCurator,
                    selectedCurator?.id === u.id,
                  )
                : renderUser(
                    u,
                    addStudent,
                    selectedStudents.some((s) => s.id === u.id),
                  ),
            )
          ) : (
            <div className="empty">Нічого не знайдено</div>
          )}
        </div>

        {/* ================= SELECTED ================= */}
        {mode === 'teacher' && selectedCurator && (
          <div className="selected-block">
            <Avatar photoUrl={selectedCurator.photoUrl} size={50} />
            <span>{selectedCurator.fullName}</span>
          </div>
        )}

        {mode === 'student' && (
          <div className="chips">
            {selectedStudents.map((s) => (
              <div
                key={s.id}
                className="chip"
                onClick={() => removeStudent(s.id)}
              >
                {s.fullName} ✕
              </div>
            ))}
          </div>
        )}

        <button className="btn" onClick={handleCreate}>
          Створити
        </button>
      </div>
    </div>
  );
}
