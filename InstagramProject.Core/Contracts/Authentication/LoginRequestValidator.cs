using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Authentication
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserNameOrEmail)
                .NotEmpty()
                .Must(userNameOrEmail => (!userNameOrEmail.Contains("xss") && !userNameOrEmail.Contains('<') && !userNameOrEmail.Contains('>')))
                .WithMessage("invalid xss request");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(password => (!password.Contains("xss") && !password.Contains('<') && !password.Contains('>')))
                .WithMessage("invalid xss request");
        }
    }
}
