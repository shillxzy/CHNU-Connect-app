import { Link } from 'react-router-dom';
import './SideBar.css';

// #8 FIX: елементи груп тепер є клікабельними посиланнями на /groups
const SideBarGroups = ({ items }) => (
  <div className="sidebar-section">
    <h3 className="sidebar-title">Популярні групи</h3>
    <ul className="sidebar-list">
      {items.map((item, index) => (
        <li className="sidebar-item" key={index}>
          <Link className="sidebar-link" to="/groups">
            {item.name || item.title || item}
          </Link>
        </li>
      ))}
    </ul>
    <Link to="/groups" className="see-more-button">
      Побачити ще
    </Link>
  </div>
);

export default SideBarGroups;