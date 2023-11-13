using Microsoft.AspNetCore.Authorization;
using WebApp_UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(5000);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("HRManagerOnly", policy => policy
    .RequireClaim("Department", "HR")
    .RequireClaim("Manager")
    .Requirements.Add(new HRManagermentProbationRequirement(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagermentProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7170/");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
