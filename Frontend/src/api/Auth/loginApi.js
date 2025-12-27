const BASE_URL = "/api/auth";

export const loginUser = async ({ email, password }) => {
  const res = await fetch(`${BASE_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({}));
    throw new Error(data.message || `Login failed: ${res.status}`);
  }

  return res.json();
};
