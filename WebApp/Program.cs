using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Data;
using WebApp.Services;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Threading.Tasks;
using CorePush.Google;
using CorePush.Apple;
using System.Configuration;
using WebApp.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppContext") ?? throw new InvalidOperationException("Connection string 'WebAppContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSingleton<IDictionary<UserConnect, string>>(opts => new Dictionary<UserConnect, string>());
builder.Services.AddSingleton<IDictionary<string, string>>(opts => new Dictionary<string, string>());
builder.Services.AddSingleton<MessagesHub>();
builder.Services.AddSingleton<ContactHub>();

builder.Services.AddControllers();
builder.Services.AddTransient<INotificationService, NotificationService>();
var defaultApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWTParams:Audience"],
        ValidIssuer = builder.Configuration["JWTParams:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTParams:SecretKey"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("http://localhost:3000");
        });

});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ContactHub>("/api/contactHub");
    endpoints.MapHub<MessagesHub>("/api/messageHub");
});

app.Run();
