import { LikeIcon, CommentIcon } from '../Icons';
import './Post.css';

export const PostActions = ({
  likes,
  comments,
  liked,
  onLikeToggle,
  onCommentToggle,
}) => (
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

    <button type="button" className="action-button">
      🔗 Share
    </button>
  </div>
);
