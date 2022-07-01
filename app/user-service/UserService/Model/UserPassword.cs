namespace UserService.Model;

using System.ComponentModel.DataAnnotations;

public class UserPassword
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Password { get; set; } = null!;
}