using FluentValidation;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Contracts.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .Must(fullname => fullname == null || (!fullname.Contains("xss") && !fullname.Contains('<') && !fullname.Contains('>')))
                .WithMessage("Invalid XSS content in FullName.")
                .When(x => x.FullName != null)
                .Length(3, 100).When(x => x.FullName != null);

            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .Must(username => username == null || (!username.Contains('.') && !username.Contains('_') && !username.Contains('@') && !username.Contains("xss") && !username.Contains('<') && !username.Contains('>')))
                .WithMessage("Username must not contain (.) or (_) or (@), or invalid characters.")
                .When(x => x.UserName != null);

            RuleFor(x => x.Bio)
                .Cascade(CascadeMode.Stop)
                .Must(bio => bio == null || (!bio.Contains("xss") && !bio.Contains('<') && !bio.Contains('>')))
                .WithMessage("Invalid XSS content in Bio.")
                .When(x => x.Bio != null);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .Must(email => email == null || (!email.Contains("xss") && !email.Contains('<') && !email.Contains('>')))
                .WithMessage("Invalid XSS content in Email.")
                .EmailAddress().When(x => x.Email != null);

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .Must(password => password == null || (!password.Contains("xss") && !password.Contains('<') && !password.Contains('>')))
                .WithMessage("Invalid XSS content in Password.")
                .Matches(RegexPatterns.Password)
                .When(x => x.Password != null);

            RuleForEach(x => x.Profile_Image)
                .SetValidator(new BlockedSignaturesValidator())
                .SetValidator(new FileSizeValidator())
                .SetValidator(new FileTypeValidator())
                .When(x => x.Profile_Image != null && x.Profile_Image.Any());
        }
    }

}
