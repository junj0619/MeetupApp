using System.Collections.Generic;
using System.Threading.Tasks;
using MeetupApp.API.Helpers;
using MeetupApp.API.Models;

namespace MeetupApp.API.Data
{
    public interface IMeetupRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int userId);
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}