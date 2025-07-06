using FluentValidation;
using InstagramProject.Core.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	internal class DeletePostRequestValidation : AbstractValidator<DeletePostRequest>
	{
		public DeletePostRequestValidation()
		{
			RuleFor(x => x.PostId)
				.NotEmpty()
				.NotNull();
		}
	}
}
