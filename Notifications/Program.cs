using Notifications.Application;
using Notifications.Core;
using Notifications.Domain;
using Notifications.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load settings
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

// CORS setup (before authentication but after HTTPS redirection)
var app = builder.Build();

// Ensure HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors(cors => cors.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin());

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
        c.RoutePrefix = string.Empty; // Set Swagger UI at the root
    });
}

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
