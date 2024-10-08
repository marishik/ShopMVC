using Microsoft.AspNetCore.Authentication.Cookies;
using ShopMVC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Home/Authorization/");

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();


var practiceClientConfig = builder.Configuration.GetSection("PracticeClient").Get<PracticeClientFactoryConfigOptions>();
if(practiceClientConfig == null) {
    throw new ArgumentNullException(nameof(practiceClientConfig), $"{nameof(PracticeClientFactoryConfigOptions): cannot be null}");
}
builder.Services.AddPracticeClient(options => {
    options.ApiUrl = practiceClientConfig.ApiUrl;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "authorization",
    pattern: "{controller=Home}/{action=Authorization}/{id?}");

app.Run();