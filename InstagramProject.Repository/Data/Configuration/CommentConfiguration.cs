using InstagramProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstagramProject.Repository.Data.Configuration
{
	public class CommentConfiguration : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.Property(c => c.Content)
				.IsRequired()
				.HasMaxLength(1000);

			builder.HasOne<Post>()
				.WithMany(p => p.Comments)
				.HasForeignKey(c => c.PostId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(c => c.User)
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(c => c.ParentComment)
				.WithMany(c => c.Replies)
				.HasForeignKey(c => c.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
