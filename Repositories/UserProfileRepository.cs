using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;


namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        private readonly IConfiguration _config;

        public UserProfileRepository(IConfiguration config) : base(config)
        {
            _config = config;
        }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT 
                    up.Id, up.[Name], up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl
                    FROM UserProfile up 
                    ORDER BY DateCreated";

                    using (var reader = cmd.ExecuteReader())
                    {
                        var userProfiles = new List<UserProfile>();
                        while (reader.Read())
                        {
                            var userProfile = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                            };
                            if (DbUtils.IsNotDbNull(reader, "UserProfileImageUrl"))
                            {
                                userProfile.ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl");
                            }
                            userProfiles.Add(userProfile);
                        }
                        return userProfiles;
                    }
                }
            }
        }

        public UserProfile GetUserById(int Id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT up.Id, up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl
                    FROM UserProfile up
                    WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        UserProfile userProfile = null;
                        if (reader.Read())
                        {
                            userProfile = new UserProfile()
                            {
                                Id = Id,
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated")
                            };
                            if (DbUtils.IsNotDbNull(reader, "UserProfileImageUrl"))
                            {
                                userProfile.ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl");
                            }
                        }
                        return userProfile;
                    }
                }
            }
        }

        public UserProfile GetUserVideosById(int userId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT up.Id AS UserId, up.[Name], up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl, up.Videos,
                    v.Id AS VideoId, v.Title, v.Description, v.Url, 
                    v.DateCreated AS VideoDateCreated, v.Comments, v.UserProfileId As VideoUserProfileId
                    FROM UserProfile up
                    LEFT JOIN Video v ON up.VideoId = videoId
                    WHERE up.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var usersVideos = new List<Video>();
                        UserProfile user = null;
                        while (reader.Read())
                        {
                            if (user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = userId,
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    Videos = new List<Video>()
                                };
                                if (DbUtils.IsNotDbNull(reader, "videoId"))
                                {
                                    user.Videos.Add(new Video()
                                    {
                                        Id = DbUtils.GetInt(reader, "videoId"),
                                        Title = DbUtils.GetString(reader, "Title"),
                                        Description = DbUtils.GetString(reader, "Description"),
                                        Url = DbUtils.GetString(reader, "Url"),
                                        DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                        UserProfileId = userId,
                                        Comments = new List<Comment>()
                                    });
                                }
                            }
                        }
                            return user;
                    }
                }
            }

        }
    }
}

