import React, { useState, useEffect } from 'react';
import Post from './Post';
import { getPosts } from '../../api/postAPI';
import '../HomePage.css';
import './NewsFeed.css';

const NewsFeed = () => {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchPosts = async () => {
            try {
                const postsData = await getPosts();

                if (Array.isArray(postsData)) {
                    setPosts(postsData);
                } else if (postsData && Array.isArray(postsData.posts)) {
                    setPosts(postsData.posts);
                } else {
                    setPosts([]);
                }

            } catch (error) {
                console.error("Error fetching posts:", error);
                setPosts([]);
            } finally {
                setLoading(false);
            }
        };

        fetchPosts();
    }, []);

    // Функція для оновлення лайків у локальному стані
    const handleLikeToggle = (postId) => {
        setPosts(prevPosts => prevPosts.map(post => {
            if (post.id === postId) {
                const liked = post.liked ? false : true;
                const likeCount = liked ? post.likeCount + 1 : post.likeCount - 1;
                return { ...post, liked, likeCount };
            }
            return post;
        }));
    };

    if (loading) {
        return (
            <div className="news-feed-container">
                <h2 className="section-title">Стрічка новин</h2>
                <p>Завантаження...</p>
            </div>
        );
    }

    return (
        <div className="news-feed-container">
            <h2 className="section-title">Стрічка новин</h2>
            
            {/* Поле створення посту */}
            <div className="post-creator card">
                <div className="post-creator-top">
                    <div className="creator-avatar"></div>
                    <textarea
                        className="creator-input"
                        placeholder="Що нового?"
                    />
                </div>
                <div className="post-creator-actions">
                    <button className="creator-add-btn">📷 Фото</button>
                    <button className="creator-post-btn">Опублікувати</button>
                </div>
            </div>

            {/* Список постів */}
            {posts.map(post => (
                <Post
                    key={post.id}
                    data={post}
                    onLikeToggle={() => handleLikeToggle(post.id)}
                />
            ))}
        </div>
    );
};

export default NewsFeed;
