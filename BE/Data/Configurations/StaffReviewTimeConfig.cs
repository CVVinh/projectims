using BE.Data.Constand;
using BE.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BE.Data.Configurations
{
    public class StaffReviewTimeConfig : IEntityTypeConfiguration<StaffReviewTime>
    {
        public void Configure(EntityTypeBuilder<StaffReviewTime> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.StaffReviewTime.TABLE_NAME);
            builder.Property(s => s.userCreated).IsRequired();
            builder.Property(s => s.detail).HasColumnType("text").HasDefaultValue("");
            builder.Property(s => s.isDeleted).HasDefaultValue(false);

            builder.HasMany(s => s.staffReviewNavigations).WithOne(s => s.StaffReviewTime)
                .HasForeignKey(s => s.IdstaffReviewDate)
                .HasConstraintName("FK_StaffReviewTime_ListstaffReviews_staffReviewTimeId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
