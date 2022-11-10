using System.ComponentModel.DataAnnotations;
namespace ChessAPI.Models
{
    public class User
    {
        [Key]
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? address { get; set; }
    }
}