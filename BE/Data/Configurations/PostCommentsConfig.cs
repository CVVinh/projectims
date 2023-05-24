using BE.Data.Constand;
using DocumentFormat.OpenXml.Office2021.PowerPoint.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BE.Data.Models;

namespace BE.Data.Configurations
{
    public class PostCommentsConfig : IEntityTypeConfiguration<PostComments>
    {
        public void Configure(EntityTypeBuilder<PostComments> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.PostComments.TABLE_NAME);
            builder.Property(s => s.userCreated).IsRequired();
            builder.HasOne(s => s.postComment)
                .WithMany(s => s.commentPostNavigations)
                .HasForeignKey(s => s.postId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.CommentsParent)
                .WithMany(s => s.PostCommentsNavigations)
                .HasForeignKey(s => s.parentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.userComment)
                .WithMany(s => s.UserCommenNavigations)
                .HasForeignKey(s => s.idUserComment)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
