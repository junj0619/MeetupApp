using System.Collections.Generic;
using System.Threading.Tasks;
using MeetupApp.API.Models;

namespace MeetupApp.API.Data
{
    public interface IMeetupRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int userId);
        Task<IEnumerable<User>> GetUsers();
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}