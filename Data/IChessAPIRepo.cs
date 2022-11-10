using ChessAPI.Models;
using ChessAPI.Dtos;

namespace ChessAPI.Data { 
public interface IChessAPIRepo
{
        IEnumerable<Product> AllItems();
        IEnumerable<Product> GetItems();
        IEnumerable<Comment> GetComments();
        Comment WriteComment(Comment comment);
        void SaveChanges();
        IEnumerable<User> AllUsers();
        User AddUser(User user);
        public bool ValidLogin(string userName, string password);
        IEnumerable<GameRecord> GetRecords();
        GameRecord AddRecord(GameRecord record);
        GameRecord RemoveRecord(GameRecord r);
    }
}