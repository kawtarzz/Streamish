using Streamish.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetAll();

    }
}