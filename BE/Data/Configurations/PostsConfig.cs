using BE.Data.Constand;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BE.Data.Models;

namespace BE.Data.Configurations
{
    public class PostsConfig: IEntityTypeConfiguration<Posts>
    {
        public void Configure(EntityTypeBuilder<Posts> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.Posts.TABLE_NAME);
            builder.Property(s => s.userCreated).IsRequired();
            builder.HasOne(s => s.postCate)
                .WithMany(s => s.postNavigations)
                .HasForeignKey(s => s.categoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
