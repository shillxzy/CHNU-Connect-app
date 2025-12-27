import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { LockIcon, EyeIcon, EyeOffIcon } from "../Icons";
import { resetPassword } from "../../api/Auth/resetPasswordApi";
import "./Login.css";

export default function ResetPassword() {
  const navigate = useNavigate();
  const [params] = useSearchParams();

  const email = params.get("email");
  const token = params.get("token");

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [show1, setShow1] = useState(false);
  const [show2, setShow2] = useState(false);

  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!email || !token) {
      setError("Недійсне або застаріле посилання");
    }
  }, [email, token]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (newPassword !== confirmPassword) {
      setError("Паролі не співпадають");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await resetPassword({ email, token, newPassword });
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
          <h2 className="header-text">Новий пароль</h2>
          <p className="subtitle">
            Введіть новий пароль для акаунту <b>{email}</b>
          </p>
        </div>

        {error && <p className="error-message">{error}</p>}

        {success ? (
          <p className="back-text">
            ✅ Пароль успішно змінено
            <br />
            <span
              className="back-link"
              style={{ cursor: "pointer" }}
              onClick={() => navigate("/login")}
            >
              Увійти
            </span>
          </p>
        ) : (
          <form onSubmit={handleSubmit}>
            <label className="label">Новий пароль</label>
            <div className="input-group">
              <img src={LockIcon} alt="lock" className="icon" />
              <input
                type={show1 ? "text" : "password"}
                className="input-field"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
              />
              <button
                type="button"
                className="eye-button"
                onClick={() => setShow1(!show1)}
              >
                <img
                  src={show1 ? EyeOffIcon : EyeIcon}
                  className="eye-icon"
                  alt=""
                />
              </button>
            </div>

            <label className="label">Підтвердження паролю</label>
            <div className="input-group">
              <img src={LockIcon} alt="lock" className="icon" />
              <input
                type={show2 ? "text" : "password"}
                className="input-field"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
              <button
                type="button"
                className="eye-button"
                onClick={() => setShow2(!show2)}
              >
                <img
                  src={show2 ? EyeOffIcon : EyeIcon}
                  className="eye-icon"
                  alt=""
                />
              </button>
            </div>

            <button
              type="submit"
              className="primary-button"
              disabled={loading}
            >
              {loading ? "Збереження..." : "Змінити пароль"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
