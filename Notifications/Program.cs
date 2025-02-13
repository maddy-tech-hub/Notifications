using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Application;
using Notifications.Core;
using Notifications.Domain;
using Notifications.Infrastructure;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load SmtpSettings from appsettings.json
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
var emailTemplates = builder.Configuration.GetSection("EmailTemplates").Get<Dictionary<string, EmailTemplate>>();

builder.Services.AddSingleton(smtpSettings);
builder.Services.AddSingleton(emailTemplates);

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailClient, EmailClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(cors => cors.AllowAnyHeader()   // Allow any headers in the request
            .AllowAnyMethod()       // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
            .AllowAnyOrigin());     // Allow requests from any origin

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notifications API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
