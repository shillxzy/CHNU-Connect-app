const BASE_URL = "/api/auth";

export const resetPassword = async ({ email, token, newPassword }) => {
  const response = await fetch(`${BASE_URL}/reset-password`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ email, token, newPassword }),
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.message || "Не вдалося змінити пароль");
  }

  return data;
};