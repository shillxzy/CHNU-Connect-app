const USER_URL = "/api/User/profile";

export const getUserProfile = async (token) => {
    const res = await fetch(USER_URL, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) throw new Error(`Error: ${res.status}`);
    return res.json();
};
