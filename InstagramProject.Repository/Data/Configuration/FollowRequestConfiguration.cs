using InstagramProject.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Repository.Data.Configuration
{
    public class FollowRequestConfiguration : IEntityTypeConfiguration<FollowRequest>
    {
        public void Configure(EntityTypeBuilder<FollowRequest> builder)
        {
            builder.HasKey(fr => fr.Id);

            builder.HasOne(fr => fr.Requester)
                   .WithMany()
                   .HasForeignKey(fr => fr.RequesterId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fr => fr.TargetUser)
                   .WithMany()
                   .HasForeignKey(fr => fr.TargetUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
