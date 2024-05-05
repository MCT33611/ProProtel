using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProPortel.Data;
using ProPortel.Models;
using ProPortel.Repositories.IRepositories;
using ProPortel.Repositories;
using ProPortel.Utility.ISevices;
using ProPortel.Utility;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SD.Role_Admin, policy =>
    {
        policy.RequireRole(SD.Role_Admin);
    });

    options.AddPolicy(SD.Role_Customer, policy =>
    {
        policy.RequireRole(SD.Role_Customer);
    });

    options.AddPolicy(SD.Role_Admin, policy =>
    {
        policy.RequireRole(SD.Role_Customer);
    });

});
builder.Services.AddCors(options =>
{


    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>(provider =>
{
    // Retrieve Cloudinary configuration from appsettings.json
    var cloudinarySettings = builder.Configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
    return new CloudinaryService(cloudinarySettings!.CloudName, cloudinarySettings!.ApiKey, cloudinarySettings!.ApiSecret);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
