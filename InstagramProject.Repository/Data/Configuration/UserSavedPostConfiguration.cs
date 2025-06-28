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
    public class UserSavedPostConfiguration : IEntityTypeConfiguration<UserSavedPost>
    {
        public void Configure(EntityTypeBuilder<UserSavedPost> builder)
        {
            builder.HasKey(ulm => new { ulm.UserId, ulm.PostId });

            builder.HasOne(ulm => ulm.User)
                .WithMany(u => u.Saved)
                .HasForeignKey(ulm => ulm.UserId);

            builder.HasOne(ulm => ulm.Post)
                .WithMany(m => m.Saved)
                .HasForeignKey(ulm => ulm.PostId);

            builder.Property(ulm => ulm.SavedOn)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
