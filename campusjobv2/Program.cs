using campusjobv2;
using campusjobv2.Models.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
        )
);


builder.Services.AddSession(options =>
{
    options.Cookie.Name = "CampusJob.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        

        context.Database.EnsureCreated();
        
        if (!context.Users.Any())
        {

            

            var adminUser = new User
            {
                First_Name = "Admin",
                Last_Name = "User",
                Email = "admin@campusjobs.com",
                Password = "test",
                Role = 1
            };
            context.Users.Add(adminUser);
            

            var recruiterUser = new User
            {
                First_Name = "John",
                Last_Name = "Recruiter",
                Email = "recruiter@campusjobs.com",
                Password = "test",
                Role = 2
            };
            context.Users.Add(recruiterUser);
            

            var studentUser = new User
            {
                First_Name = "Alice",
                Last_Name = "Student",
                Email = "student@campusjobs.com",
                Password = "test",
                Role = 3
            };
            context.Users.Add(studentUser);
            
            context.SaveChanges();
            

            context.Admins.Add(new Admin { User_ID = adminUser.User_ID });
            

            var recruiter = new Recruiter { User_ID = recruiterUser.User_ID };
            context.Recruiters.Add(recruiter);
            

            context.Employees.Add(new Employee 
            { 
                Student_ID = 1001, 
                Recruitment_ID = recruiter.Recruitment_ID 
            });
            
            context.SaveChanges();

        }
    }
    catch (Exception ex)
    {
        
    }
}


app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var method = context.Request.Method;
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    

    var excludedPaths = new[] {
        "/Login",
        "/Account",
        "/lib",
        "/css",
        "/js",
        "/_framework",
        "/favicon.ico",
        "/Home",
        "/Homepage",
        "/"
    };

    if (excludedPaths.Any(p => path.StartsWithSegments(p)) ||
        (path == "/Login/Index" && method == "POST"))
    {
        await next();
        return;
    }

    var userId = context.Session.GetInt32("UserId");

    
    if (userId == null)
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homepage}/{action=Index}/{id?}");

app.Run();
