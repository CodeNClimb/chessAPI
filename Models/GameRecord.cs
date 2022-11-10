using System.ComponentModel.DataAnnotations;

namespace ChessAPI.Models
{
    public class GameRecord
    {
        [Key]
        public Int32 id{ get; set; }
        public string? gameId { get; set; }
        public string? state { get; set; }
        public string? player1 { get; set; }
        public string? player2 { get; set; }
        public string? lastMovePlayer1 { get; set; }
        public string? lastMovePlayer2 { get; set; }

    }
}