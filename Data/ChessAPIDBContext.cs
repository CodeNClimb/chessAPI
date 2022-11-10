using Microsoft.EntityFrameworkCore;
using ChessAPI.Models;
using Microsoft.Extensions.Options;
using ChessAPI.Dtos;

namespace ChessAPI.Data
{

    public class ChessAPIDBContext : DbContext
    {
        public ChessAPIDBContext(DbContextOptions<ChessAPIDBContext> options) : base(options) { }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<GameMove> GameMoves { get; set; }
        public DbSet<GameRecord> GameRecords { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

    }
}