import React, { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getProfile, updateProfile, uploadPhoto, deletePhoto } from "../../api/userAPI";
import "./ProfileEdit.css";

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
    fullName: "",
    faculty: "",
    course: "",
    bio: "",
  });

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const res = await getProfile();
        const data = res.data;
        setUser(data);
        setFormData({
          fullName: data.fullName || "",
          faculty: data.faculty || "",
          course: data.course || "",
          bio: data.bio || "",
        });
      } catch (err) {
        setError(err.response?.data?.message || err.message || "Помилка при завантаженні профілю");
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [fullname]);

  const handleChange = (e) => {
    setFormData((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handlePhotoChange = (e) => {
    const file = e.target.files[0];
    if (file) setPhotoFile(file);
  };

  const handlePhotoUpload = async () => {
    if (!photoFile) return;
    try {
      const form = new FormData();
      form.append("photo", photoFile);

      const res = await updateProfile(formData);
      setUser((prev) => ({ ...prev, photoUrl: res.data.photoUrl }));
      setPhotoFile(null);
    } catch (err) {
      alert(err.response?.data?.message || err.message || "Помилка завантаження фото");
    }
  };

  const handlePhotoDelete = async () => {
    try {
      const token = localStorage.getItem("accessToken");
      await deletePhoto(token);
      setUser((prev) => ({ ...prev, photoUrl: null }));
      setPhotoFile(null);
    } catch (err) {
      alert(err.response?.data?.message || err.message || "Помилка видалення фото");
    }
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      await uploadPhoto(formData);
      alert("Профіль оновлено!");
      navigate(`/profile/${formData.fullName}`);
    } catch (err) {
      alert(err.response?.data?.message || err.message || "Помилка збереження профілю");
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <p>Завантаження профілю...</p>;
  if (error) return <p>Помилка: {error}</p>;
  if (!user) return <p>Користувач не знайдений</p>;

  return (
    <div className="profile-edit-container">
      <h2>Редагування профілю</h2>

      <div className="profile-edit-section">
        <div className="profile-photo-area">
          {user.photoUrl ? (
            <img src={user.photoUrl} alt="Фото профілю" className="profile-photo" />
          ) : (
            <span className="profile-photo-placeholder">Фото профілю</span>
          )}

          {/* Окрема кнопка для відкриття провідника */}
          <button
            className="btn btn-upload profile-photo-button"
            onClick={() => fileInputRef.current.click()}
          >
            {photoFile ? "Змінити фото" : "Вибрати фото"}
          </button>

          <input
            type="file"
            accept="image/*"
            ref={fileInputRef}
            style={{ display: "none" }}
            onChange={handlePhotoChange}
          />

          {user.photoUrl && !photoFile && (
            <button
              className="btn btn-delete profile-photo-button"
              onClick={handlePhotoDelete}
            >
              Видалити фото
            </button>
          )}

          {photoFile && (
            <button
              className="btn btn-save profile-photo-button"
              onClick={handlePhotoUpload}
            >
              Зберегти аватарку
            </button>
          )}
        </div>

        <div className="profile-form">
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
            <input
              type="text"
              name="faculty"
              value={formData.faculty}
              onChange={handleChange}
            />
          </label>
          <label>
            Курс:
            <input
              type="text"
              name="course"
              value={formData.course}
              onChange={handleChange}
            />
          </label>
          <label>
            Біо:
            <textarea
              name="bio"
              value={formData.bio}
              onChange={handleChange}
            />
          </label>

          <div className="profile-form-actions">
            <button className="btn btn-save" onClick={handleSave} disabled={saving}>
              {saving ? "Збереження..." : "Зберегти"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfileEdit;
