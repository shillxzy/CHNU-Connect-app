import React, { useState, useEffect } from 'react';
import '../HomePage.css';
import { getAllGroups } from '../../api/getAllGroupsApi';
import { getAllEvents } from '../../api/getAllEventsApi';

const SidebarSection = ({ title, items }) => (
    <div className="sidebar-section">
        <h3 className="sidebar-title">{title}</h3>
        <ul className="sidebar-list">
            {items.map((item, index) => <li key={index}>{item.name || item.title || item}</li>)}
        </ul>
        <button className="see-more-button">See more</button>
    </div>
);

const SidebarHomePage = () => {
    const [groups, setGroups] = useState([]);
    const [events, setEvents] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem("token");
                
                const groupsData = await getAllGroups(token);
                const eventsData = await getAllEvents(token);
                
                setGroups(groupsData || []);
                setEvents(eventsData || []);
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
            <SidebarSection title="Популярні групи" items={groups.slice(0, 5)} />
            <SidebarSection title="Події" items={events.slice(0, 5)} />
        </div>
    );
};

export default SidebarHomePage;
