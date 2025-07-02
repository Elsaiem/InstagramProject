using FluentValidation;
using InstagramProject.Core.Contracts.Authentication;
using InstagramProject.Core.Contracts.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	public class CreatePostRequestValidation : AbstractValidator<CreatePostRequest>
	{
		public CreatePostRequestValidation()
		{
			RuleForEach(x => x.PostMedia)
			   .SetValidator(new BlockedSignaturesValidator())
			   .SetValidator(new FileSizeValidator())
			   .SetValidator(new FileTypeValidator())
			   .When(x => x.PostMedia != null && x.PostMedia.Any());
		}
	}
}
