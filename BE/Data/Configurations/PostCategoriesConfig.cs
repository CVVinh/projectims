using BE.Data.Constand;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BE.Data.Models;

namespace BE.Data.Configurations
{
    public class PostCategoriesConfig: IEntityTypeConfiguration<PostCategories>
    {
        public void Configure(EntityTypeBuilder<PostCategories> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.PostCategories.TABLE_NAME);
        }
    }
}
