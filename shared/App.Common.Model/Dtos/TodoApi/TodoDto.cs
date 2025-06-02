namespace App.Common.Domain.Dtos.TodoApi
{
    public record TodoDto(
        int Id,
        string Title,
        string Description,
        DateTime DueDate,
        bool IsCompleted,
        string[] Tags,
        string AssignedTo,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
