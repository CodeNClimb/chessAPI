using System.ComponentModel.DataAnnotations;

namespace ChessAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string UserComment { get; set; }
        public string Name { get; set; }
        public string? IP { get; set; }
        public string? Time { get; set; }

    }
}