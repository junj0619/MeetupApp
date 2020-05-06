using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetupApp.API.Helpers;
using MeetupApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.API.Data
{
    public class MeetupRepository : IMeetupRepository
    {
        private readonly DataContext _context;
        public MeetupRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            /* 
              1) filter out user itself
              2) filter user gender by default using opposite user's gender
              3) filter user age between min and max age, by default user's age must in between 18 and 99
              4) filter user Likers/Likees
              5) Sorting return users by Created and LastActive, by default use LastActive Desc
            */

            var users = _context.Users.Include(p => p.Photos).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender.ToLower());

            var minDOB = DateTime.Now.AddYears(-userParams.MaxAge - 1);
            var maxDOB = DateTime.Now.AddYears(-userParams.MinAge);
            users = users.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);


            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            switch (userParams.OrderBy)
            {
                case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }


        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool Likers)
        {
            var users = await _context.Users.Include(x => x.Likers)
                                            .Include(x => x.Likees)
                                            .FirstOrDefaultAsync(u => u.Id == userId);


            if (Likers)
            {/* Pull out userIds that the user likes */
                return users.Likers.Where(u => u.LikeeId == userId).Select(i => i.LikerId);
            }
            else
            {/* pull out userIds that people who like this user */
                return users.Likees.Where(u => u.LikerId == userId).Select(i => i.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {

            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            /* Include sender photo in the message result */
            var messages = _context.Messages
            .Include(m => m.Sender).ThenInclude(m => m.Photos)
            .Include(m => m.Recipient).ThenInclude(m => m.Photos)
            .AsQueryable();



            if (messageParams.MessageContainer.Equals("Inbox", StringComparison.InvariantCultureIgnoreCase))
            {
                messages = messages.Where(m => m.RecipientId == messageParams.UserId && m.RecipientDeleted == false);
            }
            else if (messageParams.MessageContainer.Equals("Outbox", StringComparison.InvariantCultureIgnoreCase))
            {
                messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
            }
            else
            {
                messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted && m.IsRead == false);
            }


            messages = messages.OrderByDescending(m => m.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                           .Include(m => m.Sender).ThenInclude(p => p.Photos)
                           .Include(m => m.Recipient).ThenInclude(p => p.Photos)
                           .Where(m => m.RecipientId == userId && m.SenderId == recipientId && !m.RecipientDeleted
                                     || m.RecipientId == recipientId && m.SenderId == userId && !m.SenderDeleted)
                           .OrderByDescending(m => m.MessageSent)
                           .ToListAsync();

            return messages;
        }
    }
}