using System.Collections.Generic;

using Streamish.Models;

namespace Streamish.Repositories
{
    public interface IVideoRepository
    {
        void Add(Video video);
        void Add(Video video);
        void Delete(int id);
        void Delete(int id);
        List<Video> GetAll();
        List<Video> GetAll();
        Video GetById(int id);
        Video GetById(int id);
        void Update(Video video);
        void Update(Video video);
    }
}