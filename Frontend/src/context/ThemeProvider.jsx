import { useEffect, useState } from 'react';
import ThemeContext from './ThemeContext';

export function ThemeProvider({ children }) {
  const [isDark, setIsDark] = useState(() => {
    const saved = localStorage.getItem('theme');
    if (saved) {return saved === 'dark';}
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
  });

  useEffect(() => {
    document.body.classList.toggle('dark', isDark);
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
  }, [isDark]);

  // Follow OS changes only when user hasn't set a manual preference
  useEffect(() => {
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = (e) => {
      if (localStorage.getItem('theme') === null) {setIsDark(e.matches);}
    };
    mq.addEventListener('change', handler);
    return () => mq.removeEventListener('change', handler);
  }, []);

  const toggle = () => setIsDark((d) => !d);

  // Allow settings to reset to OS preference
  const resetToSystem = () => {
    localStorage.removeItem('theme');
    setIsDark(window.matchMedia('(prefers-color-scheme: dark)').matches);
  };

  return (
    <ThemeContext.Provider value={{ isDark, toggle, resetToSystem }}>
      {children}
    </ThemeContext.Provider>
  );
}
