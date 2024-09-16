import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getPostsByUserId } from "../services/PostService";
import Post from "./Post";

const UserPosts = () => {
  const { id } = useParams(); // Get user ID from the URL
  const [posts, setPosts] = useState([]);

  // Fetch posts by user ID
  useEffect(() => {
    getPostsByUserId(id).then((userPosts) => setPosts(userPosts));
  }, [id]);

  return (
    <div className="container">
      <h2>Posts by User {id}</h2>
      {posts.length > 0 ? (
        <div className="row justify-content-center">
          <div className="cards-column">
            {posts.map((post) => (
              <Post key={post.id} post={post} />
            ))}
          </div>
        </div>
      ) : (
        <p>No posts available for this user.</p>
      )}
    </div>
  );
};

export default UserPosts;
