namespace UserService.Model;

using System.ComponentModel.DataAnnotations;

public class UserUpdate
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
}