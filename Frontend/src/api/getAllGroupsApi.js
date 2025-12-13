const GROUP_URL = "/api/Group";

export const getAllGroups = async (token) => {
    const res = await fetch(GROUP_URL, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) throw new Error(`Error: ${res.status}`);
    return res.json();
};
