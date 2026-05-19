import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { searchUsers } from '../../api/userAPI';
import { searchPosts } from '../../api/postAPI';
import { getEvents } from '../../api/eventAPI';
import Avatar from '../Avatar/Avatar';
import './SearchPage.css';

export default function SearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  const [query, setQuery] = useState(searchParams.get('q') || '');
  const [users, setUsers] = useState([]);
  const [events, setEvents] = useState([]);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(false);

  const debounceRef = useRef(null);

  const runSearch = useCallback(async (q) => {
    if (!q.trim()) {
      setUsers([]);
      setEvents([]);
      setPosts([]);
      return;
    }
    setLoading(true);
    try {
      const [uRes, pRes, eRes] = await Promise.allSettled([
        searchUsers(q),
        searchPosts(q),
        getEvents(),
      ]);

      setUsers(uRes.status === 'fulfilled' ? uRes.value.data || [] : []);
      setPosts(pRes.status === 'fulfilled' ? pRes.value.data || [] : []);

      if (eRes.status === 'fulfilled') {
        const all = eRes.value.data || [];
        const ql = q.toLowerCase();
        setEvents(
          all.filter(
            (e) =>
              (e.title || '').toLowerCase().includes(ql) ||
              (e.description || '').toLowerCase().includes(ql),
          ),
        );
      } else {
        setEvents([]);
      }
    } finally {
      setLoading(false);
    }
  }, []);

  const handleInput = (e) => {
    const val = e.target.value;
    setQuery(val);
    clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => {
      setSearchParams(val ? { q: val } : {});
      runSearch(val);
    }, 350);
  };

  useEffect(() => {
    const q = searchParams.get('q') || '';
    if (q) {
      runSearch(q);
    }
  }, []);

  const total = users.length + events.length + posts.length;

  return (
    <div className="sp-wrap">
      <div className="sp-header">
        {/* #9 FIX: "Пошук користувачів" замість "Пошук" */}
        <h1 className="sp-title">Пошук користувачів</h1>
        <div className="sp-input-wrap">
          <input
            className="sp-input"
            type="text"
            placeholder="Введіть запит..."
            value={query}
            onChange={handleInput}
            autoFocus
          />
        </div>
        {query && !loading && (
          <p className="sp-count">Знайдено: {total} результатів</p>
        )}
      </div>

      {loading && <p className="sp-msg">Пошук…</p>}

      {!loading && query && (
        <>
          {/* ── USERS ── */}
          <section className="sp-section">
            <h2 className="sp-section-title">Користувачі ({users.length})</h2>
            {users.length === 0 ? (
              <p className="sp-empty">Нічого не знайдено</p>
            ) : (
              <div className="sp-users-grid">
                {users.map((u) => (
                  <div
                    key={u.id}
                    className="sp-user-card"
                    // #6 FIX: navigate by id → ProfileView, а не Profile (свій профіль)
                    onClick={() => navigate(`/profile/view/${u.id}`)}
                  >
                    <Avatar photoUrl={u.photoUrl} size={48} />
                    <div className="sp-user-info">
                      <div className="sp-user-name">
                        {u.fullName || u.email}
                      </div>
                      <div className="sp-user-meta">
                        {u.faculty || ''}
                        {u.course ? `, ${u.course} курс` : ''}
                      </div>
                      <div className="sp-user-role">{u.role}</div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* ── EVENTS ── */}
          <section className="sp-section">
            <h2 className="sp-section-title">Події ({events.length})</h2>
            {events.length === 0 ? (
              <p className="sp-empty">Нічого не знайдено</p>
            ) : (
              <div className="sp-cards-grid">
                {events.map((e) => (
                  <div
                    key={e.id}
                    className="sp-card"
                    onClick={() => navigate(`/events/${e.id}`)}
                  >
                    <div className="sp-card-title">{e.title}</div>
                    <div className="sp-card-meta">
                      {new Date(e.startTime).toLocaleDateString('uk-UA')}
                      <span
                        className={`sp-card-badge ${e.isPublic ? 'sp-card-badge--public' : 'sp-card-badge--private'}`}
                      >
                        {e.isPublic ? 'Публічна' : 'Приватна'}
                      </span>
                    </div>
                    {e.description && (
                      <div className="sp-card-sub">
                        {e.description.slice(0, 100)}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* ── POSTS ── */}
          <section className="sp-section">
            <h2 className="sp-section-title">Пости ({posts.length})</h2>
            {posts.length === 0 ? (
              <p className="sp-empty">Нічого не знайдено</p>
            ) : (
              <div className="sp-posts-list">
                {posts.map((p) => (
                  <div
                    key={p.id}
                    className="sp-post-card"
                    onClick={() => navigate(`/posts/${p.id}`)}
                  >
                    <div className="sp-post-author">
                      <Avatar
                        photoUrl={p.authorAvatar || p.author?.photoUrl}
                        size={32}
                      />
                      <span>
                        {p.authorName || p.author?.fullName || 'Автор'}
                      </span>
                      <span className="sp-post-date">
                        {p.createdAt
                          ? new Date(p.createdAt).toLocaleDateString('uk-UA')
                          : ''}
                      </span>
                    </div>
                    <p className="sp-post-content">
                      {(p.content || '').slice(0, 200)}
                      {p.content?.length > 200 ? '…' : ''}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </section>
        </>
      )}

      {!loading && !query && (
        <div className="sp-placeholder">
          <div className="sp-placeholder-icon">🔍</div>
          <p>Почніть вводити запит для пошуку</p>
        </div>
      )}
    </div>
  );
}