import { Link } from 'react-router-dom';
import './SideBar.css';

const SideBarEvents = ({ items }) => (
    <div className="sidebar-section">
        <h3 className="sidebar-title">Події</h3>
        <ul className="sidebar-list">
            {items.map((item, index) => (
                <li key={index}>{item.name || item.title || item}</li>
            ))}
        </ul>
        <Link to="/groups" className="see-more-button">Побачити ще</Link>
    </div>
);

export default SideBarEvents;
