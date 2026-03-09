import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import Post from '../Posts/Post';
import { getPosts, createPostWithImage, likePost, unlikePost } from "../../api/postAPI";
import { getProfile } from "../../api/userAPI";
import Avatar from '../Avatar/Avatar';
import './NewsFeed.css';
import UserTooltip from '../ToolTip/UserTooltip';

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

                const postsWithUser = postsData.map((post) => ({
                    ...post,
                    hasCurrentUserLiked: post.hasCurrentUserLiked ?? false,
                    authorName: post.authorName || "Unknown",
                    authorAvatar: post.authorAvatar || "../Icons/default-avatar-profile-icon.png",
                    authorId: post.authorId || post.userId,
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

    console.log("Toggling like for post:", postId, "currentlyLiked:", currentlyLiked);

    try {
        if (currentlyLiked) {
            console.log("Calling unlikePost API for post:", postId);
            await unlikePost(postId);
        } else {
            console.log("Calling likePost API for post:", postId);
            await likePost(postId);
        }

        setPosts((prevPosts) => {
            const updatedPosts = prevPosts.map((post) =>
                post.id === postId
                    ? {
                          ...post,
                          hasCurrentUserLiked: !currentlyLiked,
                          likeCount: currentlyLiked ? post.likeCount - 1 : post.likeCount + 1,
                      }
                    : post
            );

            console.log("Updated posts state after toggle:", updatedPosts);
            return updatedPosts;
        });
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
                authorAvatar: currentUser.photoUrl || "../Icons/default-avatar-icon.png",
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
  <UserTooltip
    userId={currentUser.id}
    currentUserId={currentUser?.id}
    size={38}
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

            <Post
                posts={posts}
                onLikeToggle={handleLikeToggle}
                currentUser={currentUser}
            />
            
            <Link to="/posts" className='news-feed-more-button'>Побачити ще</Link>
        </div>
    );
};

export default NewsFeed;
