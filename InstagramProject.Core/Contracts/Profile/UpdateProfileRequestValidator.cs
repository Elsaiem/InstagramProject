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
				.NotEmpty()
				.Length(3, 100)
				.WithMessage("Full Name must be at least 3 characters and at most 100 characters");

			RuleFor(x => x.UserName)
					.Must(username => (!username.Contains('.') && !username.Contains('_') && !username.Contains('@') && !username.Contains("xss") && !username.Contains('<') && !username.Contains('>')))
					.WithMessage("Username must not contain (.) or (_) or (@)");

			RuleFor(x => x.Bio)
				.Cascade(CascadeMode.Stop)
				.Must(bio => bio == null || (!bio.Contains("xss") && !bio.Contains('<') && !bio.Contains('>')))
				.WithMessage("Invalid XSS content in Bio.")
				.When(x => x.Bio != null);

			RuleFor(x => x.Password)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.WithMessage("Password should be at least 6 characters and should contain a lowercase, uppercase, number, and a special character.");


			RuleFor(x => x.ProfilePic)
				.SetValidator(new BlockedSignaturesValidator())
				.SetValidator(new FileSizeValidator())
				.SetValidator(new FileTypeValidator())
				.When(x => x.ProfilePic != null);
		}
	}

}
