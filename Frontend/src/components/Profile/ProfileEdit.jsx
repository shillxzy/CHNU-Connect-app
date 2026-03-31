import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getProfile, updateProfile, uploadPhoto } from '../../api/userAPI';
import './ProfileEdit.css';
import Avatar from '../Avatar/Avatar.jsx';
import Loading from '../Loading/Loading.jsx';

const faculties = [
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
  'Відокремлений структурний підрозділ «Фаховий коледж Чернівецького національного університету імені Юрія Федьковича»',
];

const courses = [1, 2, 3, 4, 5];

const ProfileEdit = () => {
  const { fullname } = useParams();
  const navigate = useNavigate();
  const fileInputRef = useRef(null);

  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [photoFile, setPhotoFile] = useState(null);

  const [formData, setFormData] = useState({
    fullName: '',
    faculty: '',
    course: 1,
    bio: '',
  });

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const res = await getProfile();
        const data = res.data;
        setUser(data);
        setFormData({
          fullName: data.fullName || '',
          faculty: data.faculty || faculties[0],
          course: data.course || 1,
          bio: data.bio || '',
        });
      } catch (err) {
        setError(
          err.response?.data?.message ||
            err.message ||
            'Помилка при завантаженні профілю',
        );
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, [fullname]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handlePhotoChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setPhotoFile(file);
    }
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      await updateProfile({
        fullName: formData.fullName,
        faculty: formData.faculty,
        course: formData.course,
        bio: formData.bio,
      });

      if (photoFile) {
        const photoData = new FormData();
        photoData.append('photo', photoFile);
        await uploadPhoto(photoData);
      }
      alert('Профіль оновлено!');
      navigate(`/profile/${formData.fullName}`);
    } catch (err) {
      alert(
        err.response?.data?.message ||
          err.message ||
          'Помилка збереження профілю',
      );
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return Loading();
  }
  if (error) {
    return <p>Помилка: {error}</p>;
  }
  if (!user) {
    return <p>Користувач не знайдений</p>;
  }

  return (
    <div className="profile-edit-container">
      <h2>Редагування профілю</h2>
      <div className="profile-form">
        <label>
          <div className="photo-upload-wrapper">
            <Avatar
              photoUrl={
                photoFile ? URL.createObjectURL(photoFile) : user.photoUrl
              }
              size={150}
            />

            <button
              type="button"
              className="profile-edit-btn btn-upload"
              onClick={() => fileInputRef.current.click()}
            >
              {photoFile ? 'Змінити фото' : 'Вибрати фото'}
            </button>

            <input
              type="file"
              accept="image/*"
              ref={fileInputRef}
              style={{ display: 'none' }}
              onChange={handlePhotoChange}
            />
          </div>
        </label>

        <label>
          Повне ім’я:
          <input
            type="text"
            name="fullName"
            value={formData.fullName}
            onChange={handleChange}
          />
        </label>

        <label>
          Факультет:
          <select
            name="faculty"
            value={formData.faculty}
            onChange={handleChange}
          >
            {faculties.map((f, i) => (
              <option key={i} value={f}>
                {f}
              </option>
            ))}
          </select>
        </label>

        <label>
          Курс:
          <select name="course" value={formData.course} onChange={handleChange}>
            {courses.map((c) => (
              <option key={c} value={c}>
                {c}
              </option>
            ))}
          </select>
        </label>

        <label>
          Біо:
          <textarea name="bio" value={formData.bio} onChange={handleChange} />
        </label>

        <div className="profile-form-actions">
          <button
            className="profile-edit-btn btn-save"
            onClick={handleSave}
            disabled={saving}
          >
            {saving ? 'Збереження...' : 'Зберегти'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProfileEdit;
