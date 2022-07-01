
namespace UserService.Model;

using System.ComponentModel.DataAnnotations;

class ById
{
    [Required]
    public Guid Id { get; set; }
}