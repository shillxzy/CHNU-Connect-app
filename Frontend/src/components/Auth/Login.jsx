import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Login.css";
import { MailIcon, LockIcon, EyeIcon, GoogleIcon, EyeOffIcon } from "../Icons";
import { loginUser } from "../../api/loginApi";

const handleForgotPassword = () => alert("Link 'Forgot Password' clicked!");
const handleGoogleSignIn = () => alert("Button 'Sign in with Google' clicked!");

export default function Login({ onLogin, onShowRegister }) {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
  e.preventDefault();
  setLoading(true);
  setError(null);

  try {
    const data = await loginUser({ email, password });

    if (data.token) {
      localStorage.setItem("token", data.token);
    }

    if (onLogin) onLogin(data.role); // зберігаємо роль у App

    // Редірект робимо тільки на HomePage
    navigate("/"); 

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
          <h2 className="h2">Увійти</h2>
          <p className="subtitle">
            Увійдіть, використовуючи своє ім'я користувача та пароль.
          </p>
        </div>

        {error && <p className="error-message">{error}</p>}

        <form onSubmit={handleSubmit}>
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
            >
              <img
                src={showPassword ? EyeOffIcon : EyeIcon}
                alt=""
                className="eye-icon"
                draggable="false"
              />
            </button>
          </div>

          <a href="#" onClick={handleForgotPassword} className="forgot-password-link">
            Забули пароль?
          </a>

          <button
            type="submit"
            disabled={loading}
            className={`primary-button ${loading ? 'loading' : ''}`}
          >
            {loading ? "Завантаження..." : "Увійти"}
          </button>
        </form>

        <div className="remember-me-container">
          <div 
            className={`toggle ${rememberMe ? 'checked' : ''}`} 
            onClick={() => setRememberMe(!rememberMe)}
          >
            <div className="toggle-circle"></div>
          </div>
          <span className="remember-me-label">Запам'ятати мене</span>
        </div>

        <hr className="divider" />

        <button onClick={handleGoogleSignIn} className="google-button">
          <img src={GoogleIcon} alt="google" className="google-icon" />
          <span>Sign in with Google</span>
        </button>

        <p className="signup-text">
          Не маєте акаунту? 
          <span
            onClick={() => navigate("/register")}
            className="signup-link"
            style={{ cursor: "pointer" }}
          >
            Зареєструйтесь
          </span>
        </p>
      </div>
    </div>
  );
}
