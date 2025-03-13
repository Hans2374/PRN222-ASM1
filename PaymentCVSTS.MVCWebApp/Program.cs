using PaymentCVSTS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Dependency Injection 
builder.Services.AddScoped<IPayment, PaymentService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<UserAccountService>();

// Configure authentication with more secure settings
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/UserAccounts/Login");
        options.AccessDeniedPath = new PathString("/LoginAccount/Forbidden");
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use CookieSecurePolicy.Always in production with HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add authorization services
builder.Services.AddAuthorization();

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

// These middleware components must be in this specific order
app.UseAuthentication();  // First authenticate the user
app.UseAuthorization();   // Then authorize the user

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();