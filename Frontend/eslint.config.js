import js from '@eslint/js';
import globals from 'globals';
import pluginReact from 'eslint-plugin-react';

export default [
  js.configs.recommended,
  pluginReact.configs.flat.recommended,

  {
    files: ['**/*.{js,mjs,cjs,jsx}'],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
      },
    },
    settings: {
      react: {
        version: 'detect',
      },
    },
    rules: {
      'no-unused-vars': 'warn',
      semi: ['error', 'always'],
      'no-console': 'warn',
      'no-unreachable': 'warn',
      eqeqeq: ['error', 'always'],
      curly: 'error',
      quotes: ['error', 'single', { avoidEscape: true }],
      'prefer-const': 'warn',
      'arrow-spacing': 'warn',
      'comma-dangle': ['error', 'always-multiline'],
      'object-curly-spacing': ['error', 'always'],
      'no-else-return': 'warn',
      'no-multiple-empty-lines': ['error', { max: 1, maxEOF: 0 }],

      'react/prop-types': 'off',

      'react/react-in-jsx-scope': 'off',
    },
  },
];
