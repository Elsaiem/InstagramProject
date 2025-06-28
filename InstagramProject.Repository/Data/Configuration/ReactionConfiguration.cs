using InstagramProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Repository.Data.Configuration
{
	public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
	{
		public void Configure(EntityTypeBuilder<Reaction> builder)
		{
			builder.HasOne<Post>()
				.WithMany(p => p.Reactions)
				.HasForeignKey(r => r.PostId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne<Comment>()
				.WithMany(c => c.Reactions)
				.HasForeignKey(r => r.CommentId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
