using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddAuthentication("Bearer")
   .AddJwtBearer("Bearer", opt =>
   {
       opt.RequireHttpsMetadata = false;
       opt.Authority = "https://localhost:5005";
       opt.Audience = "companyApi";
   });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RepositoryContext>(opts =>
                opts.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly("APIClient")));
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
