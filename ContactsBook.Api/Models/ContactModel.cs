using System.ComponentModel.DataAnnotations;

namespace ContactsBook.Api.Models;

public class ContactModel
{
    public string? Id { get; set;}

    [Required]
    public string FirstName { get; set; } = default!;
    [Required]
    public string LastName { get; set; }= default!;
    [Required]
    public string Email { get; set; }= default!;
}