const BASE_URL = "/api/User";

export const updateAvatar = async (file, token) => {
  const formData = new FormData();
  formData.append("file", file);

  const res = await fetch(`${BASE_URL}/profile/upload-photo`, {
    method: "PUT",
    headers: {
        "Authorization": `Bearer ${token}`,
    },
    body: formData,
  });

  return res.json();
}