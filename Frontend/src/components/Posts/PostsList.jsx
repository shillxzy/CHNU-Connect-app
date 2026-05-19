import { useState, useEffect, useContext } from 'react';
import { getPosts } from '../../api/postAPI';
import { likePost } from '../../api/postAPI';
import Post from './Post';
import Loading from '../Loading/Loading';
import AuthContext from '../../context/AuthContext';

export default function PostsList() {
  const { user: currentUser } = useContext(AuthContext);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const postsData = await getPosts();

        if (Array.isArray(postsData)) {
          setPosts(postsData);
        } else if (postsData && Array.isArray(postsData.data)) {
          setPosts(postsData.data);
        } else if (postsData && Array.isArray(postsData.posts)) {
          setPosts(postsData.posts);
        } else {
          setPosts([]);
        }
      } catch (error) {
        console.error('Error fetching posts:', error);
        setPosts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchPosts();
  }, []);

  // #5 FIX: обробка лайків у PostsList (раніше не було)
  const handleLikeToggle = async (postId, hasLiked) => {
    try {
      await likePost(postId, currentUser?.id, hasLiked);
      setPosts((prev) =>
        prev.map((p) =>
          p.id === postId
            ? {
                ...p,
                hasCurrentUserLiked: !hasLiked,
                likeCount: hasLiked ? p.likeCount - 1 : p.likeCount + 1,
              }
            : p,
        ),
      );
    } catch (e) {
      console.error('Like error:', e);
    }
  };

  if (loading) {
    return <Loading />;
  }

  // #5 FIX: передаємо currentUser щоб меню редагування/видалення з'являлось
  return (
    <Post
      posts={posts}
      onLikeToggle={handleLikeToggle}
      currentUser={currentUser}
    />
  );
}