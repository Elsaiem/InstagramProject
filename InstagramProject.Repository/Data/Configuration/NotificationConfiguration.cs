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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message)
                .IsRequired();

            builder.Property(n => n.NotificationType)
                .IsRequired();



            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(ulm => ulm.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(ulm => ulm.UserId);

            // Add relationships

        }
    }
}
