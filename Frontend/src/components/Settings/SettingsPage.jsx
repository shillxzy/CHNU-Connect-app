import React, { useEffect, useState, useRef, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  getProfile,
  updateProfile,
  uploadPhoto,
  deletePhoto,
  changePassword,
  deleteAccount,
} from '../../api/userAPI';
import Avatar from '../Avatar/Avatar';
import Loading from '../Loading/Loading';
import AuthContext from '../../context/AuthContext';
import ThemeContext from '../../context/ThemeContext';
import './SettingsPage.css';

const TABS = [
  { key: 'profile', label: 'Профіль' },
  { key: 'account', label: 'Акаунт' },
  { key: 'appearance', label: 'Вигляд' },
  { key: 'danger', label: 'Небезпечна зона' },
];

const FACULTIES = [
  'Навчально-науковий інститут біології, хімії та біоресурсів',
  'Навчально-науковий інститут фізико-технічних та комп’ютерних наук',
  'Факультет архітектури, будівництва та декоративно-прикладного мистецтва',
  'Географічний факультет',
  'Економічний факультет',
  'Факультет іноземних мов',
  'Факультет історії, політології та міжнародних відносин',
  'Факультет математики та інформатики',
  'Факультет педагогіки, психології та соціальної роботи',
  'Факультет фізичної культури, спорту та реабілітації',
  'Філологічний факультет',
  'Юридичний факультет',
  'Відокремлений структурний підрозділ «Фаховий коледж ЧНУ»',
];

/* ─────────── Profile tab ─────────── */
function ProfileTab({ user, onSaved }) {
  const fileInputRef = useRef(null);
  const [form, setForm] = useState({
    fullName: user.fullName || '',
    faculty: user.faculty || FACULTIES[0],
    course: user.course || 1,
    bio: user.bio || '',
  });
  const [photoFile, setPhotoFile] = useState(null);
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  const handle = (e) => {
    const { name, value } = e.target;
    setForm((p) => ({ ...p, [name]: value }));
  };

  const handleSave = async () => {
    setSaving(true);
    setSuccess('');
    setError('');
    try {
      await updateProfile({
        fullName: form.fullName,
        faculty: form.faculty,
        course: Number(form.course),
        bio: form.bio,
      });
      if (photoFile) {
        const fd = new FormData();
        fd.append('photo', photoFile);
        await uploadPhoto(fd);
        setPhotoFile(null);
      }
      setSuccess('Профіль оновлено.');
      onSaved();
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка збереження.');
    } finally {
      setSaving(false);
    }
  };

  const handleDeletePhoto = async () => {
    if (!window.confirm('Видалити фото профілю?')) {
      return;
    }
    try {
      await deletePhoto();
      onSaved();
      setSuccess('Фото видалено.');
    } catch {
      setError('Помилка видалення фото.');
    }
  };

  const previewUrl = photoFile ? URL.createObjectURL(photoFile) : user.photoUrl;

  return (
    <div className="st-section">
      <h3 className="st-section-title">Редагування профілю</h3>

      <div className="st-photo-row">
        <Avatar photoUrl={previewUrl} size={90} />
        <div className="st-photo-actions">
          <button
            className="st-btn st-btn-secondary"
            onClick={() => fileInputRef.current.click()}
          >
            {photoFile ? 'Змінити фото' : 'Завантажити фото'}
          </button>
          {(previewUrl || photoFile) && (
            <button className="st-btn st-btn-ghost" onClick={handleDeletePhoto}>
              Видалити фото
            </button>
          )}
        </div>
        <input
          type="file"
          accept="image/*"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={(e) => e.target.files[0] && setPhotoFile(e.target.files[0])}
        />
      </div>

      <div className="st-field">
        <label>{"Повне ім'я"}</label>
        <input
          type="text"
          name="fullName"
          value={form.fullName}
          onChange={handle}
        />
      </div>

      <div className="st-field">
        <label>Факультет</label>
        <select name="faculty" value={form.faculty} onChange={handle}>
          {FACULTIES.map((f) => (
            <option key={f} value={f}>
              {f}
            </option>
          ))}
        </select>
      </div>

      <div className="st-field">
        <label>Курс</label>
        <select name="course" value={form.course} onChange={handle}>
          {[1, 2, 3, 4, 5].map((c) => (
            <option key={c} value={c}>
              {c}
            </option>
          ))}
        </select>
      </div>

      <div className="st-field">
        <label>Про себе</label>
        <textarea name="bio" value={form.bio} onChange={handle} rows={4} />
      </div>

      {success && <p className="st-success">{success}</p>}
      {error && <p className="st-error">{error}</p>}

      <div className="st-actions">
        <button
          className="st-btn st-btn-primary"
          onClick={handleSave}
          disabled={saving}
        >
          {saving ? 'Збереження…' : 'Зберегти'}
        </button>
      </div>
    </div>
  );
}

/* ─────────── Account tab ─────────── */
function AccountTab() {
  const [form, setForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  const handle = (e) => {
    const { name, value } = e.target;
    setForm((p) => ({ ...p, [name]: value }));
  };

  const handleSave = async () => {
    setSuccess('');
    setError('');

    if (!form.currentPassword || !form.newPassword || !form.confirmPassword) {
      setError('Заповніть усі поля.');
      return;
    }
    if (form.newPassword !== form.confirmPassword) {
      setError('Новий пароль та підтвердження не збігаються.');
      return;
    }
    if (form.newPassword.length < 6) {
      setError('Новий пароль має містити щонайменше 6 символів.');
      return;
    }

    setSaving(true);
    try {
      await changePassword({
        currentPassword: form.currentPassword,
        newPassword: form.newPassword,
      });
      setSuccess('Пароль успішно змінено.');
      setForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка зміни пароля.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="st-section">
      <h3 className="st-section-title">Зміна пароля</h3>

      <div className="st-field">
        <label>Поточний пароль</label>
        <input
          type="password"
          name="currentPassword"
          autoComplete="current-password"
          value={form.currentPassword}
          onChange={handle}
        />
      </div>

      <div className="st-field">
        <label>Новий пароль</label>
        <input
          type="password"
          name="newPassword"
          autoComplete="new-password"
          value={form.newPassword}
          onChange={handle}
        />
      </div>

      <div className="st-field">
        <label>Підтвердіть новий пароль</label>
        <input
          type="password"
          name="confirmPassword"
          autoComplete="new-password"
          value={form.confirmPassword}
          onChange={handle}
        />
      </div>

      {success && <p className="st-success">{success}</p>}
      {error && <p className="st-error">{error}</p>}

      <div className="st-actions">
        <button
          className="st-btn st-btn-primary"
          onClick={handleSave}
          disabled={saving}
        >
          {saving ? 'Збереження…' : 'Змінити пароль'}
        </button>
      </div>
    </div>
  );
}

/* ─────────── Danger zone tab ─────────── */
function DangerTab({ onDeleted }) {
  const [confirm, setConfirm] = useState('');
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState('');
  const CONFIRM_WORD = 'ВИДАЛИТИ';

  const handleDelete = async () => {
    if (confirm !== CONFIRM_WORD) {
      setError(`Введіть слово «${CONFIRM_WORD}» для підтвердження.`);
      return;
    }
    setDeleting(true);
    setError('');
    try {
      await deleteAccount();
      onDeleted();
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка видалення акаунту.');
      setDeleting(false);
    }
  };

  return (
    <div className="st-section">
      <h3 className="st-section-title st-danger-title">Небезпечна зона</h3>

      <div className="st-danger-card">
        <p className="st-danger-desc">
          Видалення акаунту є <strong>незворотньою дією</strong>. Всі ваші дані
          — пости, коментарі, повідомлення — будуть видалені назавжди.
        </p>

        <div className="st-field">
          <label>
            Введіть <strong>{CONFIRM_WORD}</strong> для підтвердження
          </label>
          <input
            type="text"
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
            placeholder={CONFIRM_WORD}
          />
        </div>

        {error && <p className="st-error">{error}</p>}

        <div className="st-actions">
          <button
            className="st-btn st-btn-danger"
            onClick={handleDelete}
            disabled={deleting}
          >
            {deleting ? 'Видалення…' : 'Видалити акаунт'}
          </button>
        </div>
      </div>
    </div>
  );
}

/* ─────────── Appearance tab ─────────── */
function AppearanceTab() {
  const { isDark, toggle, resetToSystem } = useContext(ThemeContext);

  const [mode, setMode] = useState(() => {
    const saved = localStorage.getItem('theme');
    if (!saved) {return 'system';}
    return saved;
  });

  const handleMode = (newMode) => {
    setMode(newMode);
    if (newMode === 'system') {
      resetToSystem();
    } else if (newMode === 'dark' && !isDark) {
      toggle();
    } else if (newMode === 'light' && isDark) {
      toggle();
    }
  };

  return (
    <div className="st-section">
      <h3 className="st-section-title">Зовнішній вигляд</h3>
      <div className="st-field">
        <label>Тема</label>
        <div className="st-theme-options">
          {[
            { key: 'light', label: '☀️ Світла' },
            { key: 'dark', label: '🌙 Темна' },
            { key: 'system', label: '💻 Системна' },
          ].map(({ key, label }) => (
            <button
              key={key}
              className={`st-theme-btn ${mode === key ? 'active' : ''}`}
              onClick={() => handleMode(key)}
            >
              {label}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}

/* ─────────── Main component ─────────── */
export default function SettingsPage() {
  const navigate = useNavigate();
  const { logout } = useContext(AuthContext);

  const [tab, setTab] = useState('profile');
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getProfile()
      .then((res) => setUser(res.data))
      .catch(() => navigate('/login'))
      .finally(() => setLoading(false));
  }, [navigate]);

  const handleProfileSaved = async () => {
    try {
      const res = await getProfile();
      setUser(res.data);
    } catch {
      // keep existing user state if refresh fails
    }
  };

  const handleAccountDeleted = () => {
    logout();
  };

  if (loading) {
    return <Loading />;
  }
  if (!user) {
    return null;
  }

  return (
    <div className="st-wrap">
      <div className="st-card">
        <h2 className="st-title">Налаштування</h2>

        <div className="st-tabs">
          {TABS.map(({ key, label }) => (
            <button
              key={key}
              className={`st-tab ${tab === key ? 'active' : ''} ${key === 'danger' ? 'danger' : ''}`}
              onClick={() => setTab(key)}
            >
              {label}
            </button>
          ))}
        </div>

        {tab === 'profile' && (
          <ProfileTab user={user} onSaved={handleProfileSaved} />
        )}
        {tab === 'account' && <AccountTab />}
        {tab === 'appearance' && <AppearanceTab />}
        {tab === 'danger' && <DangerTab onDeleted={handleAccountDeleted} />}
      </div>
    </div>
  );
}
