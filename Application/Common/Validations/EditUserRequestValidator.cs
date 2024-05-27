using Application.Common.Interfaces.Entities.Users.DTOs;
using FluentValidation;

namespace Application.Common.Validations;

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
    public EditUserRequestValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty()
            .WithMessage("Campo de nome completo é obrigatório.")
            .MaximumLength(100)
            .WithMessage("Máximo de 100 caracteres permitidos.");
    }
}