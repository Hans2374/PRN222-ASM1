using PaymentCVSTS.Services;
using Microsoft.AspNetCore.Authentication.Cookies; // Import Cookie Authentication


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<SP25_PRN222_NET1704_PRJ_G5_CVSTSContext>();


//Dependency Injection 
builder.Services.AddScoped<IPayment, PaymentService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = new PathString("/LoginAccount/Login");
        options.AccessDeniedPath = new PathString("/LoginAccount/Forbidden");
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthorization();  // Kích hoạt Authorization Middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
