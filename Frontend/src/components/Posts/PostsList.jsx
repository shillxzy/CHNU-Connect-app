import { useState, useEffect } from "react";
import { getPosts } from "../../api/postAPI";
import Post from "./Post";
import Loading from "../Loading/Loading";

export default function PostsList() {
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

  if (loading) return <Loading />;

  return (
    <Post posts={posts} />
  );
}
