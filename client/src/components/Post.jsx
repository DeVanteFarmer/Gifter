/* eslint-disable react/prop-types */
import { Card, CardImg, CardBody } from "reactstrap";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Link } from "react-router-dom";


export const Post = ({ post }) => {
  return (
    <Card className="m-4">
      <p className="text-left px-2">Posted by: {post.userProfile.name}</p>
      <CardImg top src={post.imageUrl} alt={post.title} />
      <CardBody>
        <p>
          <Link to={`/posts/${post.id}`}>
          <strong>{post.title}</strong>
          </Link>
        </p>
        <p>{post.caption}</p>
      </CardBody>
    </Card>
  );
};