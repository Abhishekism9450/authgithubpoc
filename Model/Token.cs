using System.ComponentModel.DataAnnotations;
namespace dotnet.Models
{
    public class Token
    {
        public string Email { get; set; }
        [Key]
        public string access_Token { get; set; }
    }
}