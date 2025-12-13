import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Login.css";
import { MailIcon, LockIcon, EyeIcon, EyeOffIcon, GoogleIcon } from "../Icons";
import { registerUser } from "../../api/registrationApi";

const handleGoogleSignUp = () => alert("Button 'Sign up with Google' clicked!");

export default function Registration({ onRegister, onShowLogin }) {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (!username.trim()) return setError("Please enter your username.");
    if (password !== confirmPassword) return setError("Passwords do not match.");

    setLoading(true);
    try {
      const data = await registerUser({ username, email, password });
      if (typeof onShowLogin === 'function') {
        onShowLogin();
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="header">
          <h2 className="h2">Реєстрація</h2>
          <p className="subtitle">Створи акаунт, щоб почати користуватися додатком.</p>
        </div>

        {error && <p className="error-message">{error}</p>}

        <button onClick={handleGoogleSignUp} className="google-button" style={{ marginBottom: 16 }}>
          <img src={GoogleIcon} alt="google" className="google-icon" />
          <span>Sign up with Google</span>
        </button>

        <form onSubmit={handleSubmit}>
          <label htmlFor="name" className="label">Ім'я</label>
          <div className="input-group">
            <input
              id="username"
              type="text"
              placeholder="Ваше ім'я"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              className="input-field"
            />
          </div>

          <label htmlFor="email" className="label">Емейл</label>
          <div className="input-group">
            <img src={MailIcon} alt="mail" className="icon" />
            <input
              id="email"
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="input-field"
            />
          </div>

          <label htmlFor="password" className="label">Пароль</label>
          <div className="input-group">
            <img src={LockIcon} alt="lock" className="icon" />
            <input
              id="password"
              type={showPassword ? "text" : "password"}
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="input-field"
            />
            <button
              type="button"
              className="eye-button"
              onClick={() => setShowPassword(!showPassword)}
              aria-pressed={showPassword}
              aria-label={showPassword ? "Hide password" : "Show password"}
            >
              <img
                src={showPassword ? EyeOffIcon : EyeIcon}
                alt=""
                className="eye-icon"
                draggable="false"
              />
            </button>
          </div>

          <label htmlFor="confirmPassword" className="label">Підтвердження паролю</label>
          <div className="input-group">
            <img src={LockIcon} alt="lock" className="icon" />
            <input
              id="confirmPassword"
              type={showPassword ? "text" : "password"}
              placeholder="Confirm password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              className="input-field"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className={`primary-button ${loading ? 'loading' : ''}`}
          >
            {loading ? "Реєстрація..." : "Зареєструватися"}
          </button>
        </form>

        <p className="signup-text">
          Вже маєш акаунт?
          <span
            onClick={() => navigate("/login")}
            className="signup-link"
            style={{ cursor: "pointer" }}
          >
            Увійти
          </span>
        </p>
      </div>
    </div>
  );
}
