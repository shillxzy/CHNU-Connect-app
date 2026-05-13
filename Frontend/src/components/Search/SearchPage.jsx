import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { searchUsers } from '../../api/userAPI';
import { searchPosts } from '../../api/postAPI';
import { getAllGroups } from '../../api/groupAPI';
import Avatar from '../Avatar/Avatar';
import './SearchPage.css';

export default function SearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  const [query, setQuery] = useState(searchParams.get('q') || '');
  const [users, setUsers] = useState([]);
  const [groups, setGroups] = useState([]);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(false);

  const debounceRef = useRef(null);

  const runSearch = useCallback(async (q) => {
    if (!q.trim()) {
      setUsers([]);
      setGroups([]);
      setPosts([]);
      return;
    }
    setLoading(true);
    try {
      const [uRes, pRes, gRes] = await Promise.allSettled([
        searchUsers(q),
        searchPosts(q),
        getAllGroups(),
      ]);

      setUsers(uRes.status === 'fulfilled' ? uRes.value.data || [] : []);
      setPosts(pRes.status === 'fulfilled' ? pRes.value.data || [] : []);

      if (gRes.status === 'fulfilled') {
        const all = gRes.value.data || [];
        const ql = q.toLowerCase();
        setGroups(
          all.filter(
            (g) =>
              (g.name || '').toLowerCase().includes(ql) ||
              (g.description || '').toLowerCase().includes(ql),
          ),
        );
      } else {
        setGroups([]);
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

  // Run search on initial load if q param exists
  useEffect(() => {
    const q = searchParams.get('q') || '';
    if (q) {runSearch(q);}
  }, []);

  const total = users.length + groups.length + posts.length;

  return (
    <div className="sp-wrap">
      <div className="sp-header">
        <h1 className="sp-title">Пошук</h1>
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
                    onClick={() =>
                      navigate(
                        `/profile/${encodeURIComponent(u.fullName || u.email)}`,
                      )
                    }
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

          {/* ── GROUPS ── */}
          <section className="sp-section">
            <h2 className="sp-section-title">Групи ({groups.length})</h2>
            {groups.length === 0 ? (
              <p className="sp-empty">Нічого не знайдено</p>
            ) : (
              <div className="sp-cards-grid">
                {groups.map((g) => (
                  <div
                    key={g.id}
                    className="sp-card"
                    onClick={() => navigate('/groups')}
                  >
                    <div className="sp-card-title">{g.name}</div>
                    <div className="sp-card-sub">
                      {(g.description || '').slice(0, 100)}
                    </div>
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
