using System.Collections.Generic;

using Streamish.Models;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        List<UserProfile> GetAll();
        UserProfile GetUserById(int userId);
    }
}