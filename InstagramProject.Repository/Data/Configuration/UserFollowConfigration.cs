﻿using InstagramProject.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Repository.Data.Configuration
{
    class UserFollowConfigration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasKey(uf => new { uf.UserId, uf.FollowId });

            builder.HasOne(uf => uf.Follower)
                   .WithMany(u => u.Following)
                   .HasForeignKey(uf => uf.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid circular issues

            builder.HasOne(uf => uf.FollowedUser)
                   .WithMany(u => u.Followers)
                   .HasForeignKey(uf => uf.FollowId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid circular issues

            builder.Property(uf => uf.FollowedOn)
                   .IsRequired().
                   HasDefaultValueSql("GETDATE()");
        }
    }
}
