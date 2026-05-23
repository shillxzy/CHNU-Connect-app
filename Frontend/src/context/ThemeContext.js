import { createContext } from 'react';

const ThemeContext = createContext({ isDark: false, toggle: () => {} });

export default ThemeContext;
