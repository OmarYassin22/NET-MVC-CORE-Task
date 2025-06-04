using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DepartmentConf : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(d => d.Name)
               .IsRequired()
               .HasMaxLength(100);


        builder.HasMany(d => d.Employees)
               .WithOne(e => e.Department)
               .HasForeignKey(e => e.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.Name);
    }
}