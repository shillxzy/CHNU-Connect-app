import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createEvent } from "../../api/eventAPI";
import "./Events.css";

export default function CreateEvent() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    title: "",
    description: "",
    startTime: "",
    endTime: "",
    isPublic: true,
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    // Перевірка обов’язкових полів
    if (!formData.title || !formData.description || !formData.startTime || !formData.endTime) {
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

      console.log("Event payload:", payload); // для дебагу

      await createEvent(payload);
      navigate("/events");
    } catch (err) {
      console.error("Error creating event:", err);
      setError(err.response?.data?.message || "Помилка при створенні події");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="events-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <button className="btn-back" onClick={() => navigate(-1)}>← Назад</button>
            <h1 className="page-title">Створити подію</h1>
          </div>

          <div className="event-form">
            {error && <p style={{ color: "red" }}>{error}</p>}

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
            </div>

            <div className="form-actions">
              <button type="button" className="btn-secondary" onClick={() => navigate("/events")}>
                Відмінити
              </button>
              <button type="submit" className="btn-primary" onClick={handleSubmit} disabled={loading}>
                {loading ? "Створюється..." : "Створити подію"}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
