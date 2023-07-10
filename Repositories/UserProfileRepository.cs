using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Streamish.Models;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        private readonly IConfiguration _config;

        public UserProfileRepository(IConfiguration config) : base(config)
        {
            _config = config;
        }

        public List<UserProfile> GetAll() { return null; }

        public UserProfile GetUserById(int userId) { return null; }




    }
}
