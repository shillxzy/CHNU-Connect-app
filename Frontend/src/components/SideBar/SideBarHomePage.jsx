import React, { useState, useEffect } from 'react';
import './SideBar.css';
import { getGroups } from '../../api/groupAPI';
import { getEvents } from '../../api/eventAPI';
import SideBarEvents from './SideBarEvents';
import SideBarGroups from './SideBarGroups';
import Loading from '../Loading/Loading';

const SidebarHomePage = () => {
  const [groups, setGroups] = useState([]);
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const groupsResponse = await getGroups();
        const eventsResponse = await getEvents();

        setGroups(
          Array.isArray(groupsResponse.data) ? groupsResponse.data : [],
        );
        setEvents(
          Array.isArray(eventsResponse.data) ? eventsResponse.data : [],
        );
      } catch (error) {
        console.error('Error fetching sidebar data:', error);
        setGroups([]);
        setEvents([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <Loading />;
  }

  return (
    <div className="sidebar-container">
      <SideBarGroups items={Array.isArray(groups) ? groups.slice(0, 5) : []} />
      <SideBarEvents items={Array.isArray(events) ? events.slice(0, 5) : []} />
    </div>
  );
};

export default SidebarHomePage;
