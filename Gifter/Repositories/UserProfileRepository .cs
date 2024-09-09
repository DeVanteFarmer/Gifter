using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public UserProfile GetUserProfileWithPosts(int userProfileId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT up.Id AS UserProfileId, up.Name AS UserName, up.Email,
                       up.ImageUrl AS UserProfileImageUrl, up.DateCreated AS UserProfileDateCreated,
                       p.Id AS PostId, p.Title AS PostTitle, p.Caption AS PostCaption, 
                       p.ImageUrl AS PostImageUrl, p.DateCreated AS PostDateCreated,
                       c.Id AS CommentId, c.Message AS CommentMessage, 
                       c.UserProfileId AS CommentUserProfileId, c.PostId AS CommentPostId
                FROM UserProfile up
                LEFT JOIN Post p ON p.UserProfileId = up.Id
                LEFT JOIN Comment c ON c.PostId = p.Id
                WHERE up.Id = @UserProfileId
                ORDER BY p.DateCreated, c.Id";

                    DbUtils.AddParameter(cmd, "@UserProfileId", userProfileId);

                    var reader = cmd.ExecuteReader();

                    UserProfile user = null;

                    while (reader.Read())
                    {
                        if (user == null)
                        {
                            // Initialize the UserProfile object once
                            user = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "UserProfileId"),
                                Name = DbUtils.GetString(reader, "UserName"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetNullableString(reader, "UserProfileImageUrl"),
                                Posts = new List<Post>() // Initialize empty Post list
                            };
                        }

                        // If a post exists, find or add it to the user's Post list
                        if (DbUtils.IsNotDbNull(reader, "PostId"))
                        {
                            var postId = DbUtils.GetInt(reader, "PostId");
                            var post = user.Posts.FirstOrDefault(p => p.Id == postId);

                            if (post == null)
                            {
                                // If post does not exist in the list, create it
                                post = new Post()
                                {
                                    Id = postId,
                                    Title = DbUtils.GetString(reader, "PostTitle"),
                                    Caption = DbUtils.GetString(reader, "PostCaption"),
                                    DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                                    ImageUrl = DbUtils.GetNullableString(reader, "PostImageUrl"),
                                    UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                    Comments = new List<Comment>() // Initialize empty Comments list
                                };
                                user.Posts.Add(post);
                            }

                            // If a comment exists, add it to the post's Comments list
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                post.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "CommentMessage"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                    PostId = DbUtils.GetInt(reader, "CommentPostId")
                                });
                            }
                        }
                    }

                    reader.Close();

                    return user;
                }
            }
        }



        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Id, Name, DateCreated, ImageUrl, Email)
                        OUTPUT INSERTED.ID
                        VALUES (@Id, @Name, @DateCreated, @ImageUrl, @Email)";

                    DbUtils.AddParameter(cmd, "@Id", user.Id);
                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Id = @Id,
                               Name = @Name,
                               DateCreated = @DateCreated,
                               ImageUrl = @ImageUrl,
                               Email = @Email
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", user.Id);
                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@Id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
