using System.Security.Claims;
using System.Text;
using AuthorizationService.Api.Mapping;
using AuthorizationService.Application;
using AuthorizationService.Application.Interfaces;
using AuthorizationService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// для токенов, мемори конечно такое себе, но подкручивать что-то типа редиса это оверхед для тестового задания наверное)
// но естественно есть и другие варианты как сделать так, чтобы бд не доставляла проблем от кучи запросов и сервис был отказоустойчивее
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:3000")
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },

            // не думаю что стоит для тестового стенда добавлять refresh token, хотя и так наверно много функционала избыточно :)
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    context.HttpContext.Response.Cookies.Delete("token");

                    return context.Response.WriteAsync("{\"message\":\"Token expired\"}");
                }

                return Task.CompletedTask;
            },

            OnTokenValidated = async context =>
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var allowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

                if (allowAnonymous)
                {
                    return;
                }

                var token = context.SecurityToken as JsonWebToken;
                if (token != null)
                {
                    var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    var authManager = context.HttpContext.RequestServices.GetRequiredService<IAuthManager>();
                    var isTokenValid = await authManager.ValidateToken(int.Parse(userId), token.EncodedToken);
                    if (!isTokenValid)
                    {
                        context.Fail("Invalid token.");
                    }
                }
            }
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();