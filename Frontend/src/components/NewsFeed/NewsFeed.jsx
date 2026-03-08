import React, { useState, useEffect } from "react";
import Post from "./Post";
import { getPosts, createPostWithImage, likePost, unlikePost } from "../../api/postAPI";
import { getProfile } from "../../api/userAPI";
import "../HomePage.css";
import "./NewsFeed.css";
import Avatar from "../Avatar/Avatar.jsx";

const NewsFeed = () => {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [currentUser, setCurrentUser] = useState(null);
    const [newPostContent, setNewPostContent] = useState("");
    const [newPostImage, setNewPostImage] = useState(null);
    const [previewImage, setPreviewImage] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const resUser = await getProfile();
                setCurrentUser(resUser.data);

                const postsData = await getPosts();

                // Використовуємо поле hasCurrentUserLiked для позначки лайку
                const postsWithUser = postsData.map((post) => ({
                    ...post,
                    hasCurrentUserLiked: post.hasCurrentUserLiked ?? false,
                    authorName: post.authorName || "Unknown",
                    authorAvatar: post.authorAvatar || "/images/default-avatar-icon.png",
                }));

                setPosts(postsWithUser.slice(0, 5));
            } catch (error) {
                console.error("Error fetching posts:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const handleLikeToggle = async (postId, currentlyLiked) => {
        if (!currentUser) return;

        try {
            if (currentlyLiked) {
                await unlikePost(postId);
            } else {
                await likePost(postId);
            }

            setPosts((prevPosts) =>
                prevPosts.map((post) =>
                    post.id === postId
                        ? {
                              ...post,
                              hasCurrentUserLiked: !currentlyLiked,
                              likeCount: currentlyLiked ? post.likeCount - 1 : post.likeCount + 1,
                          }
                        : post
                )
            );
        } catch (error) {
            console.error("Error toggling like:", error);
        }
    };

    const handleImageChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setNewPostImage(file);
            setPreviewImage(URL.createObjectURL(file));
        }
    };

    const handleCreatePost = async () => {
        if (!newPostContent.trim() && !newPostImage) return;

        try {
            const formData = new FormData();
            formData.append("content", newPostContent);

            if (newPostImage) {
                formData.append("image", newPostImage);
            }

            const res = await createPostWithImage(formData);

            const newPost = {
                ...res.data,
                authorName: currentUser.fullName,
                authorAvatar: currentUser.photoUrl || "/images/default-avatar-icon.png",
                hasCurrentUserLiked: false,
                likeCount: 0,
            };

            setPosts((prev) => [newPost, ...prev]);
            setNewPostContent("");
            setNewPostImage(null);
            setPreviewImage(null);
        } catch (error) {
            console.error("Помилка створення поста:", error);
        }
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

            <div className="post-creator card">
                <div className="post-creator-top">
                    {currentUser && (
                        <Avatar
                            photoUrl={currentUser.photoUrl || "/images/default-avatar-icon.png"}
                            size={38}
                            className="creator-avatar"
                        />
                    )}
                    <textarea
                        className="creator-input"
                        placeholder="Що нового?"
                        value={newPostContent}
                        onChange={(e) => setNewPostContent(e.target.value)}
                    />
                </div>

                {previewImage && (
                    <div className="post-image-preview">
                        <img src={previewImage} alt="preview" />
                    </div>
                )}

                <div className="post-creator-actions">
                    <label className="creator-add-btn">
                        📷 Фото
                        <input type="file" accept="image/*" onChange={handleImageChange} style={{ display: "none" }} />
                    </label>

                    <button className="creator-post-btn" onClick={handleCreatePost}>
                        Опублікувати
                    </button>
                </div>
            </div>

            <Post posts={posts} onLikeToggle={handleLikeToggle} />

            <button className="news-feed-more-button">Побачити ще</button>
        </div>
    );
};

export default NewsFeed;
