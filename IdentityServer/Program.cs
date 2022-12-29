using IdentityServer.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddIdentityServer()
//         .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
//        .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
//        .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
//        .AddTestUsers(InMemoryConfig.GetUsers())
//        .AddInMemoryClients(InMemoryConfig.GetClients())
//        .AddDeveloperSigningCredential(); //not something we want to use in a production environment;

builder.Services.AddRazorPages();

var migrationAssembly = "IdentityServer";

builder.Services.AddIdentityServer()
        .AddTestUsers(InMemoryConfig.GetUsers())
        //.AddDeveloperSigningCredential() //not something we want to use in a production environment
        .AddConfigurationStore(opt =>
        {
            opt.ConfigureDbContext = c => c.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"),
                sql => sql.MigrationsAssembly(migrationAssembly));
        })
        .AddOperationalStore(opt =>
        {
            opt.ConfigureDbContext = o => o.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"),
                sql => sql.MigrationsAssembly(migrationAssembly));
        });

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
app.MapRazorPages()
    .RequireAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapDefaultControllerRoute();
//});

//app.MapGet("/", () => "Idenity Server");
//app.MigrateDatabase();
app.Run();
