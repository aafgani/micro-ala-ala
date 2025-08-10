using System.ComponentModel.DataAnnotations;

namespace App.Web.UI.Models;

public class PersonViewModel
{
    public int Id { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    public string Lastname { get; set; }
}
