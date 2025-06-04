using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeConf : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        {

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Salary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.ImagePath)
                .HasMaxLength(512);

            builder.HasOne(e => e.Department)
             .WithMany(d => d.Employees)
             .HasForeignKey(e => e.DepartmentId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.FullName);


            builder.HasIndex(e => e.ManagerId);
            builder.HasIndex(e => e.DepartmentId);
        }
    }
}
