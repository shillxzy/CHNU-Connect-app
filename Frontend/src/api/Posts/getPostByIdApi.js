export const getPostById = async (id, token) => {
  const res = await fetch(`${BASE_URL}/${id}`, {
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    }
  });
  if (!res.ok) throw new Error(`Error: ${res.status}`);
  return res.json();
};