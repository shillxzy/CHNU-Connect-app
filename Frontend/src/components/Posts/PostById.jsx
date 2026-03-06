import { useState } from "react";
import { getPostById } from "../api/postApi";

export default function PostById() {
  const [postId, setPostId] = useState("");
  const [post, setPost] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleFetch = async () => {
    if (!postId) return;
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem("token");
      const data = await getPostById(postId, token);
      setPost(data);
    } catch (err) {
      setError(err.message);
      setPost(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Get Post By ID</h2>
      <input
        type="number"
        value={postId}
        onChange={(e) => setPostId(e.target.value)}
        placeholder="Enter post ID"
      />
      <button onClick={handleFetch}>Load Post</button>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: "red" }}>Error: {error}</p>}

      {post && (
        <div style={{ border: "1px solid gray", marginTop: "10px", padding: "10px" }}>
          <h3>{post.title}</h3>
          <p>{post.content}</p>
          <p>CreatedAt: {post.createdAt}</p>
        </div>
      )}
    </div>
  );
}
