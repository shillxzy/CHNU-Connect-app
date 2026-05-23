import { useState, useRef, React } from 'react';
import { useNavigate } from 'react-router-dom';
import { createEvent, uploadEventImage } from '../../api/eventAPI';
import './Events.css';
import Loading from '../Loading/Loading';

export default function CreateEvent() {
  const navigate = useNavigate();
  const fileInputRef = useRef(null);

  const [formData, setFormData] = useState({
    title: '',
    description: '',
    startTime: '',
    endTime: '',
    isPublic: true,
  });

  const [imageFile, setImageFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    // Перевірка обов’язкових полів
    if (
      !formData.title ||
      !formData.description ||
      !formData.startTime ||
      !formData.endTime
    ) {
      setError("Будь ласка, заповніть всі обов'язкові поля");
      return;
    }

    setLoading(true);

    try {
      const formatDate = (value) => new Date(value).toISOString();

      // payload без createdById — бекенд підставить з токена
      const payload = {
        title: formData.title,
        description: formData.description,
        startTime: formatDate(formData.startTime),
        endTime: formatDate(formData.endTime),
        isPublic: formData.isPublic,
      };

      console.log('Event payload:', payload); // для дебагу

      const created = await createEvent(payload);
      if (imageFile && created.data?.id) {
        const fd = new FormData();
        fd.append('image', imageFile);
        await uploadEventImage(created.data.id, fd).catch(() => {});
      }
      navigate('/events');
    } catch (err) {
      console.error('Error creating event:', err);
      setError(err.response?.data?.message || 'Помилка при створенні події');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="events-page">
      <div className="page-content">
        <div className="content-container">
          {loading && <Loading />}
          <div className="page-header">
            <button className="btn-back" onClick={() => navigate(-1)}>
              ← Назад
            </button>
            <h1 className="page-title">Створити подію</h1>
          </div>

          <div className="event-form">
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <div className="form-section">
              <label className="form-label">Назва події</label>
              <input
                type="text"
                name="title"
                className="form-input"
                value={formData.title}
                onChange={handleChange}
              />

              <label className="form-label">Опис</label>
              <textarea
                name="description"
                className="form-textarea"
                value={formData.description}
                onChange={handleChange}
              />

              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Дата початку</label>
                  <input
                    type="datetime-local"
                    name="startTime"
                    className="form-input"
                    value={formData.startTime}
                    onChange={handleChange}
                  />
                </div>

                <div className="form-group">
                  <label className="form-label">Дата завершення</label>
                  <input
                    type="datetime-local"
                    name="endTime"
                    className="form-input"
                    value={formData.endTime}
                    onChange={handleChange}
                  />
                </div>
              </div>

              <div className="checkbox-group">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    name="isPublic"
                    checked={formData.isPublic}
                    onChange={handleChange}
                  />
                  Публічна подія
                </label>
              </div>

              <label className="form-label">
                {"Зображення події (необов'язково)"}
              </label>
              <div className="ev-image-upload">
                <button
                  type="button"
                  className="btn-secondary"
                  onClick={() => fileInputRef.current.click()}
                >
                  {imageFile ? 'Змінити фото' : 'Додати фото'}
                </button>
                {imageFile && (
                  <>
                    <span
                      style={{ fontSize: 13, color: '#4b5563', marginLeft: 8 }}
                    >
                      {imageFile.name}
                    </span>
                    <button
                      type="button"
                      className="btn-secondary"
                      style={{ marginLeft: 8 }}
                      onClick={() => setImageFile(null)}
                    >
                      ✕
                    </button>
                  </>
                )}
                <input
                  type="file"
                  accept="image/*"
                  ref={fileInputRef}
                  style={{ display: 'none' }}
                  onChange={(e) =>
                    e.target.files[0] && setImageFile(e.target.files[0])
                  }
                />
              </div>
              {imageFile && (
                <img
                  src={URL.createObjectURL(imageFile)}
                  alt="preview"
                  style={{
                    maxWidth: '100%',
                    maxHeight: 200,
                    borderRadius: 8,
                    marginTop: 8,
                    objectFit: 'cover',
                  }}
                />
              )}
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="btn-secondary"
                onClick={() => navigate('/events')}
              >
                Відмінити
              </button>
              <button
                type="submit"
                className="btn-primary"
                onClick={handleSubmit}
                disabled={loading}
              >
                {loading ? 'Створюється...' : 'Створити подію'}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
