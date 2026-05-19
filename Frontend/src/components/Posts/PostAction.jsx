import { LikeIcon, CommentIcon } from '../Icons';
import './Post.css';

// #7 FIX: кнопка Share копіює посилання на пост у буфер обміну
export const PostActions = ({
  likes,
  comments,
  liked,
  onLikeToggle,
  onCommentToggle,
  postId,
}) => {
  const handleShare = async () => {
    const url = postId
      ? `${window.location.origin}/posts/${postId}`
      : window.location.href;

    try {
      if (navigator.share) {
        await navigator.share({ url });
      } else {
        await navigator.clipboard.writeText(url);
        alert('Посилання скопійовано!');
      }
    } catch {
      // fallback: просто копіюємо
      try {
        await navigator.clipboard.writeText(url);
        alert('Посилання скопійовано!');
      } catch {
        /* ігнорувати */
      }
    }
  };

  return (
    <div className="post-actions">
      <button
        type="button"
        className={`action-button like ${liked ? 'liked' : ''}`}
        onClick={onLikeToggle}
      >
        <img src={LikeIcon} alt="Like" className="action-icon" /> {likes || 0}
      </button>

      <button type="button" className="action-button" onClick={onCommentToggle}>
        <img src={CommentIcon} alt="Comment" className="action-icon" />{' '}
        {comments || 0}
      </button>

      <button type="button" className="action-button" onClick={handleShare}>
        🔗 Share
      </button>
    </div>
  );
};