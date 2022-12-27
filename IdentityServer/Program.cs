using IdentityServer.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
         .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
        .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
        .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
        .AddTestUsers(InMemoryConfig.GetUsers())
        .AddInMemoryClients(InMemoryConfig.GetClients())
        .AddDeveloperSigningCredential(); //not something we want to use in a production environment;


builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

//app.MapGet("/", () => "Idenity Server");

app.Run();
