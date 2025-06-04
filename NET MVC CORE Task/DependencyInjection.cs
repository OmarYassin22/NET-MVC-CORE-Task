namespace NET_MVC_CORE_Task;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ARIBContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services
.AddControllersWithViews()
.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<EmployeeDTOValidator>();
});

        services.AddFluentValidationAutoValidation();
        // Register Repos
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IFileService, FileService>();

        // Register Mapster
        services.AddMapster();
        MapsterConf.Configure();

        // Add session services
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register Identity Framework Core services
        services.AddIdentity<Employee, IdentityRole<int>>(
            opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;

            })
             .AddDefaultTokenProviders()
             .AddEntityFrameworkStores<ARIBContext>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ARIBContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        try
        {
            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();

            await SeedDataAsync(context, userManager, roleManager, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }

    private static async Task SeedDataAsync(ARIBContext context, UserManager<Employee> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger logger)
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            logger.LogError("Pending migrations detected. Please apply them first.");
            return;
        }

        // Seed Roles
        if (!context.Roles.Any())
        {
            var roles = new[]
            {
                new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole<int> { Name = "Employee", NormalizedName = "EMPLOYEE" }
            };
            foreach (var role in roles)
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                    logger.LogInformation("Created role: {Name}", role.Name);
                else
                    logger.LogError("Failed to create role {Name}: {Errors}", role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // seed depts
        Department hr = null, it = null, finance = null;
        if (!context.Departments.Any())
        {
            hr = new Department { Name = "HR" };
            it = new Department { Name = "IT" };
            finance = new Department { Name = "Finance" };

            await context.Departments.AddRangeAsync(hr, it, finance);
            await context.SaveChangesAsync();
            logger.LogWarning("Seeded departments.");
        }
        else
        {
            hr = context.Departments.FirstOrDefault(d => d.Name == "HR")!;
            it = context.Departments.FirstOrDefault(d => d.Name == "IT")!;
            finance = context.Departments.FirstOrDefault(d => d.Name == "Finance")!;
        }

        var adminEmail = "admin@arib.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Employee
            {
                UserName = "admin",
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                DepartmentId = hr.Id,
                Department = hr,
                Salary = 80000m,
                ImagePath = string.Empty
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogWarning("Seeded admin user: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to seed admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        // seed users
        if (!context.Users.Any(e => e.Id != adminUser.Id))
        {
            var hrManager = new Employee
            {
                UserName = "hrmanager",
                Email = "hrmanager@arib.com",
                FirstName = "HR",
                LastName = "Manager",
                Salary = 60000m,
                ManagerId = null,
                DepartmentId = hr.Id,
                Department = hr,
                EmailConfirmed = true,
                ImagePath = string.Empty,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var itManager = new Employee
            {
                UserName = "itmanager",
                Email = "itmanager@arib.com",
                FirstName = "IT",
                LastName = "Manager",
                Salary = 70000m,
                ManagerId = null,
                DepartmentId = it.Id,
                Department = it,
                EmailConfirmed = true,
                ImagePath = string.Empty
            };
            var employee1 = new Employee
            {
                UserName = "emp1",
                Email = "emp1@arib.com",
                FirstName = "emp",
                LastName = "empfather",
                Salary = 50000m,
                ManagerId = null,
                DepartmentId = hr.Id,
                Department = hr,

                EmailConfirmed = true,
                ImagePath = string.Empty
            };
            var employee2 = new Employee
            {
                UserName = "emp2",
                Email = "emp2@arib.com",
                FirstName = "emp2",
                LastName = "emp2father",
                Salary = 55000m,
                ManagerId = null,
                DepartmentId = it.Id,
                Department = it,
                EmailConfirmed = true,
                ImagePath = string.Empty
            };

            // Assign roles
            var result = await userManager.CreateAsync(hrManager, "Hr@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(hrManager, "Manager");
            }
            else
            {
                logger.LogError("Failed to create HR Manager: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            result = await userManager.CreateAsync(itManager, "It@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(itManager, "Manager");
            }
            else
            {
                logger.LogError("Failed to create IT Manager: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            result = await userManager.CreateAsync(employee1, "Emp1@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employee1, "Employee");
            }
            else
            {
                logger.LogError("Failed to create Employee 1: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            result = await userManager.CreateAsync(employee2, "Emp2@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employee2, "Employee");
            }
            else
            {
                logger.LogError("Failed to create Employee 2: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            employee1.ManagerId = hrManager.Id;
            employee2.ManagerId = itManager.Id;
            await context.SaveChangesAsync();

            logger.LogWarning("Seeded {Count} employees.", 4);
        }

        // seed tasks
        if (!context.Tasks.Any())
        {
            var emp1 = context.Users.First(e => e.UserName == "emp1");
            var emp2 = context.Users.First(e => e.UserName == "emp2");

            var tasks = new[]
            {
                new EmployeeTask
                {
                    Title = "HR Report",
                    Description = "Prepare monthly HR report",
                    Status = TaskState.Pending,
                    EmployeeId = emp1.Id,
                    Employee=emp1,
                    CreatedAt = DateTime.UtcNow
                },
                new EmployeeTask
                {
                    Title = "IT Upgrade",
                    Description = "Upgrade server software",
                    Status = TaskState.InProgress,
                    EmployeeId = emp2.Id,
                    Employee=emp2,
                    CreatedAt = DateTime.UtcNow
                },
                new EmployeeTask
                {
                    Title = "Finance Audit",
                    Description = "Conduct annual audit",
                    Status = TaskState.Completed,
                    EmployeeId = emp2.Id,
                    Employee=emp2,
                    CreatedAt = DateTime.UtcNow
                }
            };
            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} tasks.", tasks.Length);
        }
    }
}
