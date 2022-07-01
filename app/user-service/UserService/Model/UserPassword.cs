using System.ComponentModel.DataAnnotations;

namespace UserService.Model;

public class UserPassword
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Password { get; set; }
}