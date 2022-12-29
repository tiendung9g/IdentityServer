using CompanyEmployees.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICompanyHttpClient, CompanyHttpClient>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "Cookies";
    opt.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies", (opt) =>
{
    opt.AccessDeniedPath = "/ControllerName/AccessDenied";
})
.AddOpenIdConnect("oidc", opt =>
 {
     opt.SignInScheme = "Cookies";
     opt.Authority = "https://localhost:5005";
     opt.ClientId = "mvc-client";
     opt.ResponseType = "code id_token";
     opt.SaveTokens = true;
     opt.ClientSecret = "MVCSecret";
     opt.GetClaimsFromUserInfoEndpoint = true;
     opt.ClaimActions.DeleteClaim("sid");
     opt.ClaimActions.DeleteClaim("idp");
     opt.Scope.Add("address");
     opt.ClaimActions.MapUniqueJsonKey("address", "address");
     opt.Scope.Add("roles");
     opt.ClaimActions.MapUniqueJsonKey("role", "role");
     opt.TokenValidationParameters = new TokenValidationParameters
     {
         RoleClaimType = "role"
     };
     opt.Scope.Add("companyApi");
     opt.Scope.Add("position");
     opt.Scope.Add("country");
     opt.ClaimActions.MapUniqueJsonKey("position", "position");
     opt.ClaimActions.MapUniqueJsonKey("country", "country");
 });
builder.Services.AddAuthorization(authOpt =>
{
    authOpt.AddPolicy("CanCreateAndModifyData", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("position", "Administrator");
        policyBuilder.RequireClaim("country", "USA");
    });
});
builder.Services.AddControllersWithViews();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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
