using Microsoft.EntityFrameworkCore;

namespace ChessAPI.Models
{

    [Keyless]
    public class GameMove
    {
        public string gameId { get; set; }
        public string? move { get; set; }

    }
}