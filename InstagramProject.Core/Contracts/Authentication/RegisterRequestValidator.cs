using FluentValidation;
using InstagramProject.Core.Abstractions.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 6 characters and should contain a lowercase, uppercase, number, and a special character.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(3, 100)
                .WithMessage("Full Name must be at least 3 characters and at most 100 characters");


            RuleFor(x => x.UserName)
                    .Must(username => (!username.Contains('.') && !username.Contains('_') && !username.Contains('@') && !username.Contains("xss") && !username.Contains('<') && !username.Contains('>')))
                    .WithMessage("Username must not contain (.) or (_) or (@)");

            RuleFor(x => x.BirthDay)
                .NotEmpty();
        }
    }
}
