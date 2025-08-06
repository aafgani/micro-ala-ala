using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Api.Todo.Models;

[Table("Todos")]
public partial class MyTodo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [Column("title")]
    public string Title { get; set; }

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [StringLength(255)]
    [Column("assign_to")]
    public string? AssignTo { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(255)]
    [Column("created_by")]
    public string CreatedBy { get; set; }

}
