import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { MailIcon } from "../Icons";
import { sendResetLink } from "../../api/Auth/sendResetMailApi";
import "./Login.css";

export default function ForgotPassword() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await sendResetLink(email);
      setSuccess(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container">
      <div className="card">
        <div className="header-block">
          <h2 className="header-text">Відновлення паролю</h2>
          <p className="subtitle">
            Введіть вашу електронну адресу, і ми надішлемо посилання
          </p>
        </div>

        {error && <p className="error-message">{error}</p>}

        {success ? (
          <p className="back-text">
            📧 Лист для відновлення паролю надіслано на <b>{email}</b>
          </p>
        ) : (
          <form onSubmit={handleSubmit}>
            <label className="label">Електронна пошта</label>
            <div className="input-group">
              <img src={MailIcon} alt="mail" className="icon" />
              <input
                type="email"
                className="input-field"
                placeholder="you@chnu.edu.ua"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>

            <button
              type="submit"
              className="primary-button"
              disabled={loading}
            >
              {loading ? "Надсилання..." : "Надіслати посилання"}
            </button>
          </form>
        )}

        <p
          className="back-text"
          style={{ cursor: "pointer" }}
          onClick={() => navigate("/login")}
        >
          ← Повернутися до входу
        </p>
      </div>
    </div>
  );
}
