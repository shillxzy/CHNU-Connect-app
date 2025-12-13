const BASE_URL = "/api/auth";

export const registerUser = async ({ username, email, password }) => {
  const res = await fetch(`${BASE_URL}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, email, password }),
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({}));
    throw new Error(data.message || `Registration failed: ${res.status}`);
  }

  return res.json();
};
