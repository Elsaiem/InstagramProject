using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Comment
{
	public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
	{
		public CreateCommentRequestValidator()
		{
			RuleFor(x => x.PostId)
				.NotEmpty()
				.GreaterThan(0)
				.WithMessage("PostId is required and must be greater than 0");

			RuleFor(x => x.Content)
				.NotEmpty()
				.WithMessage("Content is required")
				.Length(1, 2000)
				.WithMessage("Content must be between 1 and 2000 characters");

			RuleFor(x => x.ParentCommentId)
				.GreaterThan(0)
				.WithMessage("ParentCommentId must be greater than 0 when provided")
				.When(x => x.ParentCommentId.HasValue);
		}
	}
}
