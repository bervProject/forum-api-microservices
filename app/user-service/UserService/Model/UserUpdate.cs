using System.ComponentModel.DataAnnotations;

namespace UserService.Model;

public class UserUpdate
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
}