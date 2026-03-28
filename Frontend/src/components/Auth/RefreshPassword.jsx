import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./RefreshPassword.css";
import { forgotPassword, resetPassword } from "../../api/authAPI";

export default function RefreshPassword() {
  const navigate = useNavigate();

  const [step, setStep] = useState("email"); // email → code → newPassword
  const [email, setEmail] = useState("");
  const [code, setCode] = useState("");
  const [passwords, setPasswords] = useState({ new: "", confirm: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  // Крок 1: Відправка коду на email
  const handleSendEmail = async () => {
    setError("");
    if (!email) return;
    setLoading(true);

    try {
      await forgotPassword({ email });
      setStep("code");
    } catch (err) {
      setError(err.response?.data?.message || "Сталася помилка при відправці коду");
    } finally {
      setLoading(false);
    }
  };

  // Крок 2: Перевірка коду
  const handleVerifyCode = () => {
    setError("");
    if (!/^\d{6}$/.test(code)) {
      setError("Введіть правильний 6-значний код");
      return;
    }
    setStep("newPassword");
  };

  // Крок 3: Зміна пароля
  const handleChangePassword = async () => {
    setError("");
    if (passwords.new !== passwords.confirm) {
      setError("Паролі не співпадають");
      return;
    }
    if (!passwords.new) {
      setError("Пароль не може бути порожнім");
      return;
    }

    setLoading(true);
    try {
      await resetPassword({ email, token: code, newPassword: passwords.new });
      alert("Пароль успішно змінено!");
      navigate("/login");
    } catch (err) {
      setError(err.response?.data?.message || "Сталася помилка під час зміни пароля");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <h2 className="h2">Відновлення паролю</h2>
          <p className="subtitle">
            {step === "email" && "Введіть ваш email, щоб отримати код"}
            {step === "code" && "Введіть 6-значний код з email"}
            {step === "newPassword" && "Введіть новий пароль"}
          </p>
        </div>

        {error && <div className="error-message">{error}</div>}

        {step === "email" && (
          <>
            <label className="label">Емайл</label>
            <div className="input-login-group">
              <input
                type="email"
                className="input-login-field"
                placeholder="Введіть ваш email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <button
              className="primary-button"
              onClick={handleSendEmail}
              disabled={loading || !email}
            >
              {loading ? "Відправка..." : "Відправити код"}
            </button>
          </>
        )}

        {step === "code" && (
          <>
            <label className="label">6-значний код</label>
            <div className="input-group">
              <input
                type="text"
                className="input-field"
                placeholder="Введіть код"
                value={code}
                onChange={(e) => setCode(e.target.value)}
                maxLength={6}
                required
              />
            </div>
            <button
              className="primary-button"
              onClick={handleVerifyCode}
              disabled={loading || code.length !== 6}
            >
              Перевірити код
            </button>
          </>
        )}

        {step === "newPassword" && (
          <>
            <label className="label">Новий пароль</label>
            <div className="input-group">
              <input
                type="password"
                className="input-field"
                placeholder="Новий пароль"
                value={passwords.new}
                onChange={(e) =>
                  setPasswords({ ...passwords, new: e.target.value })
                }
                required
              />
            </div>

            <label className="label">Підтвердіть пароль</label>
            <div className="input-group">
              <input
                type="password"
                className="input-field"
                placeholder="Підтвердіть пароль"
                value={passwords.confirm}
                onChange={(e) =>
                  setPasswords({ ...passwords, confirm: e.target.value })
                }
                required
              />
            </div>

            <button
              className="primary-button"
              onClick={handleChangePassword}
              disabled={loading || !passwords.new || !passwords.confirm}
            >
              {loading ? "Збереження..." : "Змінити пароль"}
            </button>
          </>
        )}
      </div>
    </div>
  );
}
