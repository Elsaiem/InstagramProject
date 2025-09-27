using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public class UnFollowRequestValidator : AbstractValidator<UnFollowRequest>
    {
        public UnFollowRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");
            RuleFor(x => x.FollowId)
                .NotEmpty()
                .WithMessage("FollowId is required.");
        }
    }
}
