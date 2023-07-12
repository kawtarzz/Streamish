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

    }
}
