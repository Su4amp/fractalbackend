using Application.Common.Interfaces.Entities.Users.DTOs;
using FluentValidation;

namespace Application.Common.Validations;

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty()
            .WithMessage("Campo de username é obrigatório.");

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Campo de senha é obrigatório.");
    }
}