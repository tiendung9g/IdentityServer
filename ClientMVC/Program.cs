using CompanyEmployees.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICompanyHttpClient, CompanyHttpClient>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "Cookies";
    opt.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies")
.AddOpenIdConnect("oidc", opt =>
 {
     opt.SignInScheme = "Cookies";
     opt.Authority = "https://localhost:5005";
     opt.ClientId = "mvc-client";
     opt.ResponseType = "code id_token";
     opt.SaveTokens = true;
     opt.ClientSecret = "MVCSecret";
     opt.GetClaimsFromUserInfoEndpoint = true;
 });
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
