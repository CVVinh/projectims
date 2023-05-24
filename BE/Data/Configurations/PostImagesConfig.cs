using BE.Data.Constand;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BE.Data.Models;

namespace BE.Data.Configurations
{
    public class PostImagesConfig: IEntityTypeConfiguration<PostImages>
    {
        public void Configure(EntityTypeBuilder<PostImages> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.PostImages.TABLE_NAME);

            builder.HasOne(s => s.postImages)
                .WithMany(s => s.imagePostsNavigations)
                .HasForeignKey(s => s.postId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
