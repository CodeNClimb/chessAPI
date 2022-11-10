using ChessAPI.Data;
using ChessAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace ChessAPI.Dtos
{

    public class CommentDto
    {
        [Required]
        public string UserComment { set; get; }
        public string? Name { set; get; }

    }
}