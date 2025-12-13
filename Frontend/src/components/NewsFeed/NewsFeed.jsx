import React, { useState, useEffect } from 'react';
import Post from './Post';
import { getAllPosts } from '../../api/getAllPostsApi';
import '../HomePage.css';

const NewsFeed = () => {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchPosts = async () => {
            try {
                const token = localStorage.getItem("token");
                const postsData = await getAllPosts(token);
                setPosts(postsData || []);
            } catch (error) {
                console.error("Error fetching posts:", error);
                setPosts([]);
            } finally {
                setLoading(false);
            }
        };

        fetchPosts();
    }, []);

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
            <div className="post-creator">
                <div className="post-creator-buttons">
                    <button className="post-btn-base post-btn-post">Post</button>
                    <button className="post-btn-base post-btn-add">Add</button>
                </div>
            </div>

            {/* Список постів */}
            {posts.map(post => (
                <Post key={post.id} data={post} />
            ))}
        </div>
    );
};

export default NewsFeed;
