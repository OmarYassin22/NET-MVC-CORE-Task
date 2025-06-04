using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeTaskConf : IEntityTypeConfiguration<EmployeeTask>
{
    public void Configure(EntityTypeBuilder<EmployeeTask> builder)
    {
        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(t => t.Description)
               .HasMaxLength(1000);

        builder.Property(t => t.Status)
               .IsRequired();

        builder.Property(t => t.CreatedAt)
               .IsRequired();


        builder.HasOne(t => t.Employee)
               .WithMany(e => e.Tasks)
               .HasForeignKey(t => t.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}