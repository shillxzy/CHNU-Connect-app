import React, { useContext, useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getUserById } from "../../api/userAPI";
import { getPostsByUser } from "../../api/postAPI";
import { getOrCreateDirectChat } from "../../api/chatAPI";
import Avatar from "../Avatar/Avatar.jsx";
import Post from "../Posts/Post.jsx";
import AuthContext from "../../context/AuthContext";
import "./Profile.css";
import Loading from "../Loading/Loading.jsx";

const ProfileView = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { user: currentUser } = useContext(AuthContext); 
  console.log("Current User in ProfileView:", currentUser);
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUserAndPosts = async () => {
      try {
        const resUser = await getUserById(id);
        const userData = resUser.data;
        setUser(userData);

        const resPosts = await getPostsByUser(userData.id);
        const userPosts = Array.isArray(resPosts.data) ? resPosts.data : [];

        const postsWithAuthor = userPosts.map(post => ({
          ...post,
          authorId: userData.id,
          authorName: userData.fullName,
          authorAvatar: userData.photoUrl || "../Icons/default-avatar-profile-icon.png",
          hasCurrentUserLiked: post.hasCurrentUserLiked ?? false,
        }));

        setPosts(postsWithAuthor);
      } catch (err) {
        console.error("Error fetching user or posts:", err);
        setError(err.response?.data?.message || "Помилка при завантаженні профілю");
      } finally {
        setLoading(false);
      }
    };

    fetchUserAndPosts();
  }, [id]);

  const handleLikeToggle = (postId, currentlyLiked) => {
    setPosts(prev =>
      prev.map(post =>
        post.id === postId
          ? { ...post, hasCurrentUserLiked: !currentlyLiked, likeCount: currentlyLiked ? post.likeCount - 1 : post.likeCount + 1 }
          : post
      )
    );
  };

  const handleMessageClick = async () => {
  if (!currentUser?.id || !user?.id) return;
  try {
    // Отримуємо або створюємо чат
    const chat = await getOrCreateDirectChat(currentUser.id, user.id);
    
    // Переходимо у чат
    navigate(`/chats/${chat.id}`);
  } catch (err) {
    console.error("Не вдалося відкрити чат:", err);
  }
};


  if (loading) return <Loading />;
  if (error) return <p>Помилка: {error}</p>;
  if (!user) return <p>Користувач не знайдений</p>;

  return (
    <div className="profile-page-container">
      <div className="profile-header">
        <div className="profile-photo-area">
          <Avatar photoUrl={user.photoUrl} size={150} />
        </div>
        <div className="profile-info-actions">
          <div className="profile-details">
            <h2 className="profile-name">{user.fullName}</h2>
            <p><span className="profile-label">Факультет:</span> {user.faculty}</p>
            <p><span className="profile-label">Курс:</span> {user.course}</p>
            <p><span className="profile-label">Біо:</span> {user.bio}</p>

            {currentUser?.id && user?.id && currentUser.id !== user.id && (
              <button className="btn btn-messages" onClick={handleMessageClick}>
                Повідомлення
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="recent-posts-section">
        <h3 className="section-title">Пости користувача</h3>
        {posts.length > 0 ? (
          <Post posts={posts} onLikeToggle={handleLikeToggle} currentUser={currentUser} />
        ) : (
          <p>У цього користувача ще немає постів</p>
        )}
      </div>
    </div>
  );
};

export default ProfileView;
