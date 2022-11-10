using ChessAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ChessAPI.Dtos;

namespace ChessAPI.Data
{
    public class ChessAPIRepo : IChessAPIRepo
    {
        private readonly ChessAPIDBContext _dbContext;

        public ChessAPIRepo(ChessAPIDBContext A1dbContext)
        {
            _dbContext = A1dbContext;
        }

        public IEnumerable<Product> AllItems()
        {
            IEnumerable<Product> products = _dbContext.Products.ToList<Product>();
            return products;
        }

        public IEnumerable<Product> GetItems()
        {
            IEnumerable<Product> products = _dbContext.Products.ToList<Product>();
            return products;
        }

        public IEnumerable<Comment> GetComments()
        {
            IEnumerable<Comment> comments = _dbContext.Comments.ToList<Comment>();
            return comments;
        } 
        public Comment WriteComment(Comment comment)
        {
            EntityEntry<Comment> e = _dbContext.Comments.Add(comment);
            Comment c = e.Entity;
            _dbContext.SaveChanges();
            return c;
        }

        public IEnumerable<User> AllUsers()
        {
            IEnumerable<User> users = _dbContext.Users.ToList<User>();
            return users;
        }
        public bool ValidLogin(string userName, string password)
        {
            User user = _dbContext.Users.FirstOrDefault(e => e.userName == userName && e.password == password);

            if (user == null)
                return false;
            return true;
        }
        public User AddUser(User user)
        {
            EntityEntry<User> u = _dbContext.Users.Add(user);
            User user1 = u.Entity;
            _dbContext.SaveChanges();
            return user1;
        }
        public IEnumerable<GameRecord> GetRecords()
        {
            IEnumerable<GameRecord> records = _dbContext.GameRecords.ToList<GameRecord>();
            return records;
        }
        public GameRecord AddRecord(GameRecord r)
        {
            EntityEntry<GameRecord> u = _dbContext.GameRecords.Add(r);
            GameRecord record = u.Entity;
            _dbContext.SaveChanges();
            return record;
        }
        public GameRecord RemoveRecord(GameRecord r)
        {
            EntityEntry<GameRecord> u = _dbContext.GameRecords.Remove(r);
            GameRecord record = u.Entity;
            _dbContext.SaveChanges();
            return record;
        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }


    }
}