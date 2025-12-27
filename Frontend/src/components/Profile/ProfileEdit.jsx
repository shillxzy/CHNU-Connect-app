import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getUserProfile } from "../../api/Profile/getUserProfileApi";
import { updateProfile } from "../../api/Profile/updateUserApi";
import { updateAvatar } from "../../api/Profile/updateUserPhotoApi";
import "./ProfileEdit.css";

const DEFAULT_AVATAR = "https://ui-avatars.com/api/?name=User&background=004B8D&color=fff";

export default function ProfileEdit() {
  const navigate = useNavigate();

  const [visibility, setVisibility] = useState("all");
  const [profile, setProfile] = useState(null);
  const [avatarFile, setAvatarFile] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => { 
    const token = localStorage.getItem("token");

    if (!token) {
      navigate("/login"); 
      return; 
    }

    getUserProfile(token).then((data) => { 
      setProfile({ 
        email: data.email ?? "", 
        fullName: data.fullName ?? "", 
        faculty: data.faculty ?? "", 
        course: data.course ?? "", 
        specialty: data.specialty ?? "",
        contactInfo: data.contactInfo ?? "",
        bio: data.bio ?? "", 
        telegram: data.telegram ?? "",
        instagram: data.instagram ?? "",
        linkedin: data.linkedin ?? "",
        photoUrl: data.photoUrl || DEFAULT_AVATAR, 
      }); 
    }).catch(err => { 
      console.error(err); 
    }); 
  }, [navigate]);

  if (!profile) return <div className="page"><div className="loading">Завантаження...</div></div>;

  const handleSave = async () => {
    const token = localStorage.getItem("token"); // <- додано
    if (!token) {
      alert("Ви не авторизовані");
      navigate("/login");
      return;
    }

    try {
      setLoading(true);

      await updateProfile({
        email: profile.email,
        fullName: profile.fullName,
        faculty: profile.faculty,
        course: profile.course,
        specialty: profile.specialty,
        contactInfo: profile.contactInfo,
        bio: profile.bio,
        telegram: profile.telegram,
        instagram: profile.instagram,
        linkedin: profile.linkedin,
        visibility,
      }, token);

      if (avatarFile) {
        const { photoUrl } = await updateAvatar(avatarFile, token);
        setProfile((p) => ({ ...p, photoUrl }));
      }

      navigate(-1);
    } finally {
      setLoading(false);
    }
  };

  const handleAvatarChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setAvatarFile(file);
      const reader = new FileReader();
      reader.onload = (e) => {
        setProfile((p) => ({ ...p, photoUrl: e.target.result }));
      };
      reader.readAsDataURL(file);
    }
  };

  const handleDeleteAvatar = () => {
    setAvatarFile(null);
    setProfile((p) => ({ ...p, photoUrl: DEFAULT_AVATAR }));
  };

  return (
    <div className="page">
      <div className="profile-edit-container">
        <h1 className="title">Редагування профілю</h1>

        {/* Фото профілю */}
        <div className="photo-card">
          <div className="avatar-profile-edit">
            <img src={profile.photoUrl} alt="avatar" />
          </div>

          <div className="photo-actions">
            <label className="btn-profile-edit">
              Змінити фото
              <input
                type="file"
                hidden
                accept="image/*"
                onChange={handleAvatarChange}
              />
            </label>

            <button 
              className="btn-profile-edit btn-outline"
              onClick={handleDeleteAvatar}
            >
              Видалити фото
            </button>
          </div>
        </div>

        {/* Основна інформація */}
        <div className="form-card">
          <h2 className="section-title">Основна інформація</h2>
          
          <div className="form-grid">
            <div className="form-group">
              <label className="label-profile-edit">ПІБ</label>
              <input
                className="input-profile-edit"
                value={profile.fullName || ""}
                onChange={(e) =>
                  setProfile({ ...profile, fullName: e.target.value })
                }
                placeholder="Введіть ваше повне ім'я"
              />
            </div>

            <div className="form-grid-2">
              <div className="form-group">
                <label className="label-profile-edit">Факультет</label>
                <input
                  className="input-profile-edit"
                  value={profile.faculty || ""}
                  onChange={(e) =>
                    setProfile({ ...profile, faculty: e.target.value })
                  }
                  placeholder="Наприклад: ФКНТ"
                />
              </div>

              <div className="form-group">
                <label className="label-profile-edit">Курс</label>
                <input
                  className="input-profile-edit"
                  value={profile.course || ""}
                  onChange={(e) =>
                    setProfile({ ...profile, course: e.target.value })
                  }
                  placeholder="Наприклад: 3"
                />
              </div>
            </div>

            <div className="form-group">
              <label className="label-profile-edit">
                Спеціальність / напрямок
              </label>
              <input
                className="input-profile-edit"
                value={profile.specialty || ""}
                onChange={(e) =>
                  setProfile({ ...profile, specialty: e.target.value })
                }
                placeholder="Наприклад: Комп'ютерні науки"
              />
            </div>

            <div className="form-group">
              <label className="label-profile-edit">
                Контактна інформація
              </label>
              <input
                className="input-profile-edit"
                value={profile.contactInfo || ""}
                onChange={(e) =>
                  setProfile({ ...profile, contactInfo: e.target.value })
                }
                placeholder="Email або телефон"
              />
            </div>

            <div className="form-group">
              <label className="label-profile-edit">Коротке біо</label>
              <textarea
                className="textarea-profile-edit"
                rows="4"
                value={profile.bio || ""}
                onChange={(e) =>
                  setProfile({ ...profile, bio: e.target.value })
                }
                placeholder="Розкажіть трохи про себе..."
              />
            </div>
          </div>
        </div>

        {/* Соціальні мережі */}
        <div className="form-card">
          <h2 className="section-title">Соціальні мережі</h2>
          
          <div className="form-grid">
            <div className="form-group">
              <label className="label-profile-edit">Telegram</label>
              <input
                className="input-profile-edit"
                value={profile.telegram || ""}
                onChange={(e) =>
                  setProfile({ ...profile, telegram: e.target.value })
                }
                placeholder="@username"
              />
            </div>

            <div className="form-group">
              <label className="label-profile-edit">Instagram</label>
              <input
                className="input-profile-edit"
                value={profile.instagram || ""}
                onChange={(e) =>
                  setProfile({ ...profile, instagram: e.target.value })
                }
                placeholder="@username"
              />
            </div>

            <div className="form-group">
              <label className="label-profile-edit">LinkedIn</label>
              <input
                className="input-profile-edit"
                value={profile.linkedin || ""}
                onChange={(e) =>
                  setProfile({ ...profile, linkedin: e.target.value })
                }
                placeholder="linkedin.com/in/username"
              />
            </div>
          </div>
        </div>

        {/* Конфіденційність */}
        <div className="form-card">
          <h2 className="section-title">Конфіденційність</h2>
          
          <div className="privacy-section">
            <label className="privacy-label">Хто бачить профіль:</label>
            <div className="privacy">
              {["all", "friends", "only"].map((v) => (
                <button
                  key={v}
                  className={`chip ${visibility === v ? "active" : ""}`}
                  onClick={() => setVisibility(v)}
                >
                  {v === "all" ? "Всі" : v === "friends" ? "Друзі" : "Тільки я"}
                </button>
              ))}
            </div>
          </div>

          <div className="actions">
            <button
              className={`btn-profile-edit ${loading ? "btn-disabled" : ""}`}
              onClick={handleSave}
              disabled={loading}
            >
              {loading ? "Збереження..." : "Зберегти"}
            </button>

            <button
              className="btn-profile-edit btn-outline"
              onClick={() => navigate(-1)}
            >
              Скасувати
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}