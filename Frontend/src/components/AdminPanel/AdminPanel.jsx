import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import './AdminPanel.css';
import {
  getUsers,
  blockUser,
  unblockUser,
  setRole,
  updateUser,
  deleteEvent,
  deleteGroup,
  deletePost,
} from '../../api/adminAPI';
import { getEvents, updateEvent } from '../../api/eventAPI';
import { getAllGroups, updateGroup } from '../../api/groupAPI';
import { getPosts, updatePost } from '../../api/postAPI';
import Avatar from '../Avatar/Avatar';

const ROLES = ['student', 'teacher', 'admin'];
const SECTIONS = [
  { key: 'users', label: 'Користувачі' },
  { key: 'events', label: 'Події' },
  { key: 'groups', label: 'Групи' },
  { key: 'posts', label: 'Пости' },
];

/* ──────────────── helpers ──────────────── */
const fmt = (d) => (d ? new Date(d).toLocaleDateString('uk-UA') : '—');
const fmtDT = (d) =>
  d
    ? new Date(d).toLocaleString('uk-UA', {
        dateStyle: 'short',
        timeStyle: 'short',
      })
    : '—';
// format ISO → value for datetime-local input
const toLocal = (d) => {
  if (!d) {return '';}
  const dt = new Date(d);
  const pad = (n) => String(n).padStart(2, '0');
  return `${dt.getFullYear()}-${pad(dt.getMonth() + 1)}-${pad(dt.getDate())}T${pad(dt.getHours())}:${pad(dt.getMinutes())}`;
};

/* ──────────────── Edit Modal ──────────────── */
function EditModal({ title, fields, values, onSave, onClose, saving, extra }) {
  const [form, setForm] = useState(values);
  const set = (k, v) => setForm((p) => ({ ...p, [k]: v }));

  return (
    <div className="ap-overlay" onClick={onClose}>
      <div className="ap-modal" onClick={(e) => e.stopPropagation()}>
        <h3 className="ap-modal-title">{title}</h3>

        {fields.map(({ key, label, type = 'text', options }) => (
          <div className="ap-field" key={key}>
            <label>{label}</label>
            {options ? (
              <select
                value={form[key] ?? ''}
                onChange={(e) => set(key, e.target.value)}
              >
                {options.map((o) => (
                  <option key={o} value={o}>
                    {o}
                  </option>
                ))}
              </select>
            ) : type === 'textarea' ? (
              <textarea
                rows={4}
                value={form[key] ?? ''}
                onChange={(e) => set(key, e.target.value)}
              />
            ) : (
              <input
                type={type}
                value={form[key] ?? ''}
                onChange={(e) => set(key, e.target.value)}
              />
            )}
          </div>
        ))}

        {/* extra slot — for post image preview */}
        {extra && extra(form, set)}

        <div className="ap-modal-actions">
          <button className="ap-btn-cancel" onClick={onClose}>
            Скасувати
          </button>
          <button
            className="ap-btn-save"
            onClick={() => onSave(form)}
            disabled={saving}
          >
            {saving ? 'Збереження…' : 'Зберегти'}
          </button>
        </div>
      </div>
    </div>
  );
}

/* ──────────────── Main component ──────────────── */
export default function AdminPanel() {
  const navigate = useNavigate();
  const [section, setSection] = useState('users');
  const [search, setSearch] = useState('');

  const [users, setUsers] = useState([]);
  const [events, setEvents] = useState([]);
  const [groups, setGroups] = useState([]);
  const [posts, setPosts] = useState([]);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const [editTarget, setEditTarget] = useState(null);
  const [saving, setSaving] = useState(false);

  /* ── load ── */
  const load = useCallback(async (sec) => {
    setLoading(true);
    setError(null);
    try {
      if (sec === 'users') {
        const r = await getUsers();
        setUsers(Array.isArray(r.data) ? r.data : []);
      }
      if (sec === 'events') {
        const r = await getEvents();
        setEvents(Array.isArray(r.data) ? r.data : []);
      }
      if (sec === 'groups') {
        const r = await getAllGroups();
        setGroups(Array.isArray(r.data) ? r.data : []);
      }
      if (sec === 'posts') {
        const r = await getPosts();
        setPosts(Array.isArray(r) ? r : []);
      }
    } catch (e) {
      setError(e.response?.data?.message || 'Помилка завантаження');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load(section);
  }, [section, load]);

  /* ── user actions ── */
  const handleBlock = async (id) => {
    await blockUser(id);
    setUsers((p) =>
      p.map((u) => (u.id === id ? { ...u, isBlocked: true } : u)),
    );
  };
  const handleUnblock = async (id) => {
    await unblockUser(id);
    setUsers((p) =>
      p.map((u) => (u.id === id ? { ...u, isBlocked: false } : u)),
    );
  };
  const handleRole = async (id, role) => {
    await setRole({ userId: id, role });
    setUsers((p) => p.map((u) => (u.id === id ? { ...u, role } : u)));
  };

  /* ── save ── */
  const handleSave = async (form) => {
    setSaving(true);
    try {
      const { type, data } = editTarget;

      if (type === 'user') {
        await updateUser(data.id, {
          fullName: form.fullName,
          faculty: form.faculty,
          course: Number(form.course) || null,
          bio: form.bio,
        });
        setUsers((p) =>
          p.map((u) => (u.id === data.id ? { ...u, ...form } : u)),
        );
      }

      if (type === 'event') {
        await updateEvent(data.id, {
          title: form.title,
          description: form.description,
          startTime: form.startTime
            ? new Date(form.startTime).toISOString()
            : data.startTime,
          endTime: form.endTime
            ? new Date(form.endTime).toISOString()
            : data.endTime,
          isPublic: data.isPublic,
        });
        setEvents((p) =>
          p.map((e) => (e.id === data.id ? { ...e, ...form } : e)),
        );
      }

      if (type === 'group') {
        await updateGroup(data.id, {
          name: form.name,
          description: form.description,
        });
        setGroups((p) =>
          p.map((g) => (g.id === data.id ? { ...g, ...form } : g)),
        );
      }

      if (type === 'post') {
        await updatePost(data.id, {
          content: form.content,
          imageUrl: form.removeImage ? null : data.imageUrl,
        });
        setPosts((p) =>
          p.map((p2) =>
            p2.id === data.id
              ? {
                  ...p2,
                  content: form.content,
                  imageUrl: form.removeImage ? null : p2.imageUrl,
                }
              : p2,
          ),
        );
      }

      setEditTarget(null);
    } catch (e) {
      alert(e.response?.data?.message || 'Помилка збереження');
    } finally {
      setSaving(false);
    }
  };

  /* ── delete ── */
  const handleDelete = async (type, id) => {
    if (!window.confirm('Видалити? Цю дію не можна скасувати.')) {return;}
    try {
      if (type === 'event') {
        await deleteEvent(id);
        setEvents((p) => p.filter((e) => e.id !== id));
      }
      if (type === 'group') {
        await deleteGroup(id);
        setGroups((p) => p.filter((g) => g.id !== id));
      }
      if (type === 'post') {
        await deletePost(id);
        setPosts((p) => p.filter((p2) => p2.id !== id));
      }
    } catch (e) {
      alert(e.response?.data?.message || 'Помилка видалення');
    }
  };

  /* ── search filter ── */
  const q = search.toLowerCase();
  const filteredUsers = users.filter((u) =>
    [u.fullName, u.email, u.faculty]
      .filter(Boolean)
      .some((f) => f.toLowerCase().includes(q)),
  );
  const filteredEvents = events.filter((e) =>
    [e.title, e.description]
      .filter(Boolean)
      .some((f) => f.toLowerCase().includes(q)),
  );
  const filteredGroups = groups.filter((g) =>
    [g.name, g.description]
      .filter(Boolean)
      .some((f) => f.toLowerCase().includes(q)),
  );
  const filteredPosts = posts.filter((p) =>
    [p.content, p.authorName]
      .filter(Boolean)
      .some((f) => f.toLowerCase().includes(q)),
  );

  /* ── edit config ── */
  const editFields = () => {
    if (!editTarget) {return [];}
    const { type } = editTarget;
    if (type === 'user')
      {return [
        { key: 'fullName', label: "Повне ім'я" },
        { key: 'faculty', label: 'Факультет' },
        { key: 'course', label: 'Курс', type: 'number' },
        { key: 'bio', label: 'Про себе', type: 'textarea' },
      ];}
    if (type === 'event')
      {return [
        { key: 'title', label: 'Назва' },
        { key: 'description', label: 'Опис', type: 'textarea' },
        { key: 'startTime', label: 'Початок', type: 'datetime-local' },
        { key: 'endTime', label: 'Кінець', type: 'datetime-local' },
      ];}
    if (type === 'group')
      {return [
        { key: 'name', label: 'Назва' },
        { key: 'description', label: 'Опис', type: 'textarea' },
      ];}
    if (type === 'post')
      {return [{ key: 'content', label: 'Зміст', type: 'textarea' }];}
    return [];
  };

  const editInitial = () => {
    if (!editTarget) {return {};}
    const { type, data } = editTarget;
    if (type === 'user')
      {return {
        fullName: data.fullName || '',
        faculty: data.faculty || '',
        course: data.course || '',
        bio: data.bio || '',
      };}
    if (type === 'event')
      {return {
        title: data.title || '',
        description: data.description || '',
        startTime: toLocal(data.startTime),
        endTime: toLocal(data.endTime),
      };}
    if (type === 'group')
      {return { name: data.name || '', description: data.description || '' };}
    if (type === 'post')
      {return { content: data.content || '', removeImage: false };}
    return {};
  };

  /* ── post image extra slot ── */
  const postImageExtra = (form, set) => {
    const src = editTarget?.data?.imageUrl;
    if (!src) {return null;}
    return (
      <div className="ap-field">
        <label>Зображення</label>
        {!form.removeImage ? (
          <div className="ap-img-preview-wrap">
            <img src={src} alt="post" className="ap-img-preview" />
            <button
              type="button"
              className="ap-btn-remove-img"
              onClick={() => set('removeImage', true)}
            >
              Видалити зображення
            </button>
          </div>
        ) : (
          <div className="ap-img-removed">
            Зображення буде видалено після збереження
          </div>
        )}
      </div>
    );
  };

  /* ── render ── */
  return (
    <div className="ap-wrap">
      {/* SIDEBAR */}
      <aside className="ap-sidebar">
        <h2 className="ap-sidebar-title">Адмін панель</h2>
        <nav className="ap-nav">
          {SECTIONS.map(({ key, label }) => (
            <button
              key={key}
              className={`ap-nav-btn ${section === key ? 'active' : ''}`}
              onClick={() => {
                setSection(key);
                setSearch('');
              }}
            >
              {label}
            </button>
          ))}
        </nav>
      </aside>

      {/* CONTENT */}
      <main className="ap-content">
        <div className="ap-content-header">
          <h2 className="ap-section-title">
            {SECTIONS.find((s) => s.key === section)?.label}
          </h2>
          <input
            className="ap-search"
            type="text"
            placeholder="Пошук за назвою, автором, описом..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        {loading && <p className="ap-msg">Завантаження…</p>}
        {error && <p className="ap-msg ap-error">{error}</p>}

        {/* ── USERS ── */}
        {!loading && section === 'users' && (
          <div className="ap-table-wrap">
            <table className="ap-table">
              <thead>
                <tr>
                  <th>Користувач</th>
                  <th>Факультет / Курс</th>
                  <th>Роль</th>
                  <th>Статус</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {filteredUsers.map((u) => (
                  <tr
                    key={u.id}
                    className={u.isBlocked ? 'ap-row-blocked' : ''}
                  >
                    <td className="ap-cell-user">
                      <Avatar photoUrl={u.photoUrl} size={32} />
                      <div>
                        <div className="ap-user-name">{u.fullName || '—'}</div>
                        <div className="ap-user-email">{u.email}</div>
                      </div>
                    </td>
                    <td>
                      {u.faculty || '—'} / {u.course ?? '—'}
                    </td>
                    <td>
                      <select
                        className="ap-select"
                        value={u.role?.toLowerCase() || 'student'}
                        onChange={(e) => handleRole(u.id, e.target.value)}
                      >
                        {ROLES.map((r) => (
                          <option key={r} value={r}>
                            {r}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td>
                      {u.isBlocked ? (
                        <span className="ap-badge-blocked">Заблокований</span>
                      ) : (
                        <span className="ap-badge-active">Активний</span>
                      )}
                    </td>
                    <td className="ap-actions">
                      <button
                        className="ap-btn-edit"
                        onClick={() => setEditTarget({ type: 'user', data: u })}
                      >
                        Редагувати
                      </button>
                      {u.isBlocked ? (
                        <button
                          className="ap-btn-unblock"
                          onClick={() => handleUnblock(u.id)}
                        >
                          Розблокувати
                        </button>
                      ) : (
                        <button
                          className="ap-btn-block"
                          onClick={() => handleBlock(u.id)}
                        >
                          Заблокувати
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filteredUsers.length === 0 && !loading && (
              <p className="ap-msg">Нічого не знайдено</p>
            )}
          </div>
        )}

        {/* ── EVENTS ── */}
        {!loading && section === 'events' && (
          <div className="ap-table-wrap">
            <table className="ap-table">
              <thead>
                <tr>
                  <th>Назва / Опис</th>
                  <th>Початок</th>
                  <th>Кінець</th>
                  <th>Публічна</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {filteredEvents.map((e) => (
                  <tr key={e.id}>
                    <td>
                      <strong>{e.title}</strong>
                      <div className="ap-sub">
                        {(e.description || '').slice(0, 70)}
                        {e.description?.length > 70 ? '…' : ''}
                      </div>
                    </td>
                    <td className="ap-nowrap">{fmtDT(e.startTime)}</td>
                    <td className="ap-nowrap">{fmtDT(e.endTime)}</td>
                    <td>{e.isPublic ? 'Так' : 'Ні'}</td>
                    <td className="ap-actions">
                      <button
                        className="ap-btn-edit"
                        onClick={() =>
                          setEditTarget({ type: 'event', data: e })
                        }
                      >
                        Редагувати
                      </button>
                      <button
                        className="ap-btn-delete"
                        onClick={() => handleDelete('event', e.id)}
                      >
                        Видалити
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filteredEvents.length === 0 && !loading && (
              <p className="ap-msg">Нічого не знайдено</p>
            )}
          </div>
        )}

        {/* ── GROUPS ── */}
        {!loading && section === 'groups' && (
          <div className="ap-table-wrap">
            <table className="ap-table">
              <thead>
                <tr>
                  <th>Назва</th>
                  <th>Опис</th>
                  <th>Учасники</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {filteredGroups.map((g) => (
                  <tr key={g.id}>
                    <td>
                      <strong>{g.name}</strong>
                    </td>
                    <td className="ap-sub">
                      {(g.description || '').slice(0, 80)}
                    </td>
                    <td>{g.memberCount ?? g.members?.length ?? '—'}</td>
                    <td className="ap-actions">
                      <button
                        className="ap-btn-edit"
                        onClick={() =>
                          setEditTarget({ type: 'group', data: g })
                        }
                      >
                        Редагувати
                      </button>
                      <button
                        className="ap-btn-schedule"
                        onClick={() => navigate(`/group/edit/schedule/${g.id}`)}
                      >
                        Розклад
                      </button>
                      <button
                        className="ap-btn-delete"
                        onClick={() => handleDelete('group', g.id)}
                      >
                        Видалити
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filteredGroups.length === 0 && !loading && (
              <p className="ap-msg">Нічого не знайдено</p>
            )}
          </div>
        )}

        {/* ── POSTS ── */}
        {!loading && section === 'posts' && (
          <div className="ap-table-wrap">
            <table className="ap-table">
              <thead>
                <tr>
                  <th>Зміст</th>
                  <th>Зображення</th>
                  <th>Автор</th>
                  <th>Дата</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {filteredPosts.map((p) => (
                  <tr key={p.id}>
                    <td className="ap-post-content">
                      {(p.content || '').slice(0, 100)}
                      {p.content?.length > 100 ? '…' : ''}
                    </td>
                    <td>
                      {p.imageUrl ? (
                        <img
                          src={p.imageUrl}
                          alt="post"
                          className="ap-post-thumb"
                          onClick={() => window.open(p.imageUrl, '_blank')}
                        />
                      ) : (
                        <span className="ap-sub">—</span>
                      )}
                    </td>
                    <td>{p.authorName || '—'}</td>
                    <td className="ap-nowrap">{fmt(p.createdAt)}</td>
                    <td className="ap-actions">
                      <button
                        className="ap-btn-edit"
                        onClick={() => setEditTarget({ type: 'post', data: p })}
                      >
                        Редагувати
                      </button>
                      <button
                        className="ap-btn-delete"
                        onClick={() => handleDelete('post', p.id)}
                      >
                        Видалити
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filteredPosts.length === 0 && !loading && (
              <p className="ap-msg">Нічого не знайдено</p>
            )}
          </div>
        )}
      </main>

      {/* EDIT MODAL */}
      {editTarget && (
        <EditModal
          title={`Редагувати ${
            editTarget.type === 'user'
              ? 'користувача'
              : editTarget.type === 'event'
                ? 'подію'
                : editTarget.type === 'group'
                  ? 'групу'
                  : 'пост'
          }`}
          fields={editFields()}
          values={editInitial()}
          onSave={handleSave}
          onClose={() => setEditTarget(null)}
          saving={saving}
          extra={editTarget.type === 'post' ? postImageExtra : null}
        />
      )}
    </div>
  );
}
