const ADMIN_URL = "/api/Admin/users";

export const getAllUsers = async (token) => {
    const res = await fetch(ADMIN_URL, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) throw new Error(`Error: ${res.status}`);
    return res.json();
};
