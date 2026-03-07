import React, { useState, useEffect } from 'react';
import '../HomePage.css';
import { getGroups } from "../../api/groupAPI";
import { getEvents } from "../../api/eventAPI";

const SidebarSection = ({ title, items }) => (
    <div className="sidebar-section">
        <h3 className="sidebar-title">{title}</h3>
        <ul className="sidebar-list">
            {items.map((item, index) => (
                <li key={index}>{item.name || item.title || item}</li>
            ))}
        </ul>
        <button className="see-more-button">Побачити ще</button>
    </div>
);

const SidebarHomePage = () => {
    const [groups, setGroups] = useState([]);
    const [events, setEvents] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const groupsResponse = await getGroups();
                const eventsResponse = await getEvents();

                setGroups(Array.isArray(groupsResponse.data) ? groupsResponse.data : []);
                setEvents(Array.isArray(eventsResponse.data) ? eventsResponse.data : []);
            } catch (error) {
                console.error("Error fetching sidebar data:", error);
                setGroups([]);
                setEvents([]);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    if (loading) {
        return <div className="sidebar-container"><p>Завантаження...</p></div>;
    }

    return (
        <div className="sidebar-container">
            <SidebarSection
                title="Популярні групи"
                items={Array.isArray(groups) ? groups.slice(0, 5) : []}
            />
            <SidebarSection
                title="Події"
                items={Array.isArray(events) ? events.slice(0, 5) : []}
            />
        </div>
    );
};

export default SidebarHomePage;
