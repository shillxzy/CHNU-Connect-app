import { Link } from 'react-router-dom';
import './SideBar.css';

const SideBarEvents = ({ items }) => (
    <div className="sidebar-section">
        <h3 className="sidebar-title">Події</h3>
        <ul className="sidebar-list">
        {items.map((item, index) => (
          <li className='sidebar-item' key={index}>
            <Link className='sidebar-link' to={`/events/${item.id}`}>
              {item.name || item.title || item}
            </Link>
          </li>
        ))}
      </ul>

        <Link to="/events" className="see-more-button">Побачити ще</Link>
    </div>
);

export default SideBarEvents;
