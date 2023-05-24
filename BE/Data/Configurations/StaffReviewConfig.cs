using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE.Data.Configurations
{
    public class StaffReviewConfig : IEntityTypeConfiguration<StaffReview>
    {
        public void Configure(EntityTypeBuilder<StaffReview> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id)
                .UseIdentityColumn();
            builder.Property(x => x.achievements).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.knowledge).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.skill).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.forwardMindset).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.positiveMindset).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.steadfastMindset).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.perfectionistMindset).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.fromTalkToResult).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.connectToAction).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.hobbies).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.personality).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.commitmentsForCompany).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.colleagueOpinion).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.HROpinion).HasColumnType("text").HasDefaultValue("");
            builder.Property(x => x.isDeleted).HasDefaultValue(false);
            builder.Property(x => x.Approver).HasDefaultValue(0);
        }
    }
}
