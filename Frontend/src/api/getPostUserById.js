const POST_URL = "/api/Post/user";

export const getPostUserById = async (userId, token) => {
    const res = await fetch(`${POST_URL}/${userId}`, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) throw new Error(`Error: ${res.status}`);
    return res.json();
};
