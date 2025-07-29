using App.Common.Domain.Dtos.Todo;
using FluentValidation;

namespace App.Api.Todo.Dtos.Validators;

public class TodolistDtoValidator : AbstractValidator<TodolistDto>
{
    public TodolistDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
