const EVENT_URL = "/api/Event";

export const getAllEvents = async (token) => {
    const res = await fetch(EVENT_URL, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    if (!res.ok) throw new Error(`Error: ${res.status}`);
    return res.json();
};