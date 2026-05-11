import { useEffect, useState, React } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

import {
  getGroupById,
  updateGroup,
  assignCurator,
  addUserToGroup,
} from '../../api/groupAPI';

import {
  getSubjectsByGroup,
  createSubject,
  deleteSubject,
} from '../../api/subjectAPI';

import { getAllUsers } from '../../api/userAPI';

import Avatar from '../Avatar/Avatar';
import './GroupEdit.css';

export default function GroupEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  /* ===== GROUP ===== */
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');

  /* ===== USERS ===== */
  const [users, setUsers] = useState([]);
  const [mode, setMode] = useState('teacher');

  const [search, setSearch] = useState('');
  const [debounced, setDebounced] = useState('');

  const [selectedCurator, setSelectedCurator] = useState(null);
  const [selectedStudents, setSelectedStudents] = useState([]);

  // 👇 важливо: зберігаємо існуючих
  const [existingStudents, setExistingStudents] = useState([]);

  /* ===== SUBJECTS ===== */
  const [subjects, setSubjects] = useState([]);
  const [newSubject, setNewSubject] = useState({
    name: '',
    moodleLink: '',
    teacherId: null,
  });

  /* ================= LOAD ================= */
  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    const groupRes = await getGroupById(id);
    const g = groupRes.data;

    setName(g.name);
    setDescription(g.description);

    if (g.curator) {
      setSelectedCurator(g.curator);
    }

    if (g.users) {
      setSelectedStudents(g.users);
      setExistingStudents(g.users);
    }

    const subjectsRes = await getSubjectsByGroup(id);
    setSubjects(subjectsRes.data);

    const usersRes = await getAllUsers();
    setUsers(usersRes.data);
  };

  /* ================= SEARCH ================= */
  useEffect(() => {
    const t = setTimeout(() => setDebounced(search), 300);
    return () => clearTimeout(t);
  }, [search]);

  useEffect(() => {
    setSearch('');
  }, [mode]);

  const filteredUsers = users
    .filter((u) => (u.role || '').toLowerCase() === mode)
    .filter((u) =>
      (u.fullName || '').toLowerCase().includes(debounced.toLowerCase()),
    );

  /* ================= USERS ACTIONS ================= */

  const addStudent = (user) => {
    if (!selectedStudents.some((s) => s.id === user.id)) {
      setSelectedStudents((prev) => [...prev, user]);
    }
  };

  const removeStudent = (id) => {
    setSelectedStudents((prev) => prev.filter((s) => s.id !== id));
  };

  /* ================= SUBJECTS ================= */

  const handleAddSubject = () => {
    if (!newSubject.name.trim()) {
      return;
    }

    const subjectToAdd = {
      name: newSubject.name,
      moodleLink: newSubject.moodleLink || null,
      teacherId: newSubject.teacherId || null,
      isNew: true,
      id: Date.now(),
    };

    setSubjects((prev) => [...prev, subjectToAdd]);

    setNewSubject({
      name: '',
      moodleLink: '',
      teacherId: null,
    });
  };

  const removeSubject = async (subject) => {
    try {
      if (subject.isNew) {
        setSubjects((prev) => prev.filter((s) => s.id !== subject.id));
        return;
      }

      await deleteSubject(subject.id);

      setSubjects((prev) => prev.filter((s) => s.id !== subject.id));
    } catch {
      alert('Не вдалося видалити дисципліну');
    }
  };

  /* ================= SAVE ================= */

  const handleSave = async () => {
    try {
      // 1. group
      await updateGroup(id, { name, description });

      // 2. curator
      if (selectedCurator) {
        await assignCurator(id, selectedCurator.id);
      }

      // 3. students (тільки нові)
      const existingIds = existingStudents.map((s) => s.id);

      await Promise.all(
        selectedStudents.map((s) => {
          if (existingIds.includes(s.id)) {
            return;
          }

          return addUserToGroup(id, s.id, 'student');
        }),
      );

      // 4. subjects
      for (const s of subjects) {
        if (s.isNew) {
          await createSubject({
            name: s.name,
            groupId: id,
            teacherId: s.teacherId,
            moodleLink: s.moodleLink,
          });
        }
      }

      alert('Збережено');
      navigate(`/groups/${id}`);
    } catch (err) {
      console.error(err);
      alert('Помилка збереження');
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
    <div className="edit-page">
      <h2>Редагування групи</h2>

      {/* ===== GROUP ===== */}
      <div className="card">
        <h3>Інформація</h3>

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
      </div>

      {/* ===== CURATOR + STUDENTS ===== */}
      <div className="card">
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

        <input
          className="search"
          placeholder="Пошук..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <div className="list">
          {filteredUsers.map((u) =>
            mode === 'teacher'
              ? renderUser(u, setSelectedCurator, selectedCurator?.id === u.id)
              : renderUser(
                  u,
                  addStudent,
                  selectedStudents.some((s) => s.id === u.id),
                ),
          )}
        </div>

        {/* SELECTED */}
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
      </div>

      <button
        className="schedule-btn"
        onClick={() => navigate(`/group/edit/schedule/${id}`)}
      >
        Редагувати розклад
      </button>

      {/* ===== SUBJECTS ===== */}
      <div className="card">
        <h3>Дисципліни</h3>

        <div className="add-subject">
          <input
            className="add-subject-input"
            placeholder="Назва"
            value={newSubject.name}
            onChange={(e) =>
              setNewSubject({ ...newSubject, name: e.target.value })
            }
          />

          <input
            className="add-subject-input"
            placeholder="Moodle"
            value={newSubject.moodleLink}
            onChange={(e) =>
              setNewSubject({ ...newSubject, moodleLink: e.target.value })
            }
          />

          <button className="add-subject-button" onClick={handleAddSubject}>
            Додати
          </button>
        </div>

        <div className="subjects-list">
          {subjects.map((s) => (
            <div key={s.id} className="subject-item">
              <div
                className="subject-info"
                onClick={() => s.moodleLink && window.open(s.moodleLink)}
              >
                <b>{s.name}</b>
                <span>{s.moodleLink}</span>
              </div>

              <button onClick={() => removeSubject(s)}>✕</button>
            </div>
          ))}
        </div>
      </div>

      <button className="save-btn" onClick={handleSave}>
        Зберегти зміни
      </button>
    </div>
  );
}
