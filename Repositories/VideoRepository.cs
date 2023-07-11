using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class VideoRepository : BaseRepository, IVideoRepository
    {
        private readonly IConfiguration _config;

        public VideoRepository(IConfiguration config) : base(config) 
        {
            _config = config;
        }

        public List<Video> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated, v.UserProfileId,
                    up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl
                    FROM Video v 
                    JOIN UserProfile up ON v.UserProfileId = up.Id
                    ORDER BY DateCreated";

                    using (var reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        if (reader.Read())
                        {
                            var video = new Video()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                Url = DbUtils.GetString(reader, "Url"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                },
                            };
                            if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
                            {
                                video.UserProfile.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
                            }
                            videos.Add(video);
                        }
                            return videos;
                    }
                }
            }
        }
        public List<Video> GetAllWithComments()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT v.Id AS VideoId, v.Title, v.Description, v.Url,
                    v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,
                    up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl,
                    c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                    FROM Video v 
                    JOIN UserProfile up ON v.UserProfileId = up.Id
                    LEFT JOIN Comment c on c.VideoId = v.id
                    ORDER BY v.DateCreated";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            var videoId = DbUtils.GetInt(reader, "VideoId");

                            var existingVideo = videos.FirstOrDefault(p => p.Id == videoId);
                            if (existingVideo == null)
                            {
                                existingVideo = new Video()
                                {
                                    Id = videoId,
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                    UserProfile = new UserProfile()
                                    {
                                        Id = DbUtils.GetInt(reader, "VideoUserProfileId"),
                                        Name = DbUtils.GetString(reader, "Name"),
                                        Email = DbUtils.GetString(reader, "Email"),
                                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl")
                                    },
                                    Comments = new List<Comment>()
                                };

                                videos.Add(existingVideo);
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                existingVideo.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = videoId,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }

                        return videos;
                    }
                }
            }
        }


        public Video GetById(int id)
        {
        using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  v.Id AS VideoId, v.Title, v.Description, v.Url, 
                    v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,
                    up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl
                    FROM Video v
                    JOIN UserProfile up ON v.UserProfileId = up.Id
                    WHERE V.Id = @Id";

                    DbUtils.AddParameter(cmd, "@VideoId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Video video = null;
                        if (reader.Read())
                        {
                            video = new Video()
                            {
                                Id = id,
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                Url = DbUtils.GetString(reader, "Url"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile
                                {
                                    Id = DbUtils.GetInt(reader,("UserProfileId")),
                                    Name= DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                                }
                            };
                            if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
                            {
                                video.UserProfile.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
                            }
                        }

                        return video;
                    }
                }
            }
        }

        public void Add(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Video (Title, Description, DateCreated, Url, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (@Title, @Description, @DateCreated, @Url, @UserProfileId)";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);

                    video.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Video
                           SET Title = @Title,
                               Description = @Description,
                               DateCreated = @DateCreated,
                               Url = @Url,
                               UserProfileId = @UserProfileId
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);
                    DbUtils.AddParameter(cmd, "@Id", video.Id);

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
                    cmd.CommandText = "DELETE FROM Video WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Video GetVideoByIdWithComments(int videoId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, 
                    v.DateCreated AS VideoDateCreated, 
                    c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                    FROM Video v 
                    LEFT JOIN Comment c on c.VideoId = v.id
                    ORDER BY v.DateCreated";

                    DbUtils.AddParameter(cmd, "@VideoId", videoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Video video = null;
                        if (reader.Read())
                        {
                            video = new Video()
                            {
                                Id = videoId,
                                Title = DbUtils.GetString(reader, "Title"),
                                Description = DbUtils.GetString(reader, "Description"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                Url = DbUtils.GetString(reader, "Url"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                Comments = new List<Comment>()
                            };
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                video.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = videoId,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }
                        return video;
                    }
                }
            }
        }
    }
}
