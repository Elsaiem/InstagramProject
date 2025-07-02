﻿using FluentValidation;
using InstagramProject.Core.Abstractions.Consts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Files
{
	public class FileSizeValidator : AbstractValidator<IFormFile>
	{
		public FileSizeValidator()
		{
			RuleFor(x => x)
				.Must((request, context) => request.Length <= FileSettings.MaxFileSizeInBytes)
				.WithMessage($"Max file size is {FileSettings.MaxFileSizeInMB} MB.")
				.When(x => x is not null);
		}
	}
}
