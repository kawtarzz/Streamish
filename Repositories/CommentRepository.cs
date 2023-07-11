using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private readonly IConfiguration _config;

        public CommentRepository(IConfiguration config) : base(config)
        {
            _config = config;
        }
        public List<Comment> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS CommentId, 
                               c.Message, c.UserProfileId, c.VideoId, up.[Name] AS UserProfileName, up.Id AS UserProfileId, v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated 
                     FROM Comment c 
                    JOIN UserProfile up ON c.UserProfileId = up.Id
                    JOIN Video v ON c.VideoId = v.Id
                    ORDER BY DateCreated";
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var comments = new List<Comment>();
                        while (reader.Read())
                        {
                            comments.Add(new Comment()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Message = DbUtils.GetString(reader, "Message"),
                                VideoId = DbUtils.GetInt(reader, "VideoId"),
                                Video = new Video()
                                {
                                    Id = DbUtils.GetInt(reader, "Id"),
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                },
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name")
                                }
                            });
                        }

                        return comments;

                    }
                }
            }
        }
    }
}

