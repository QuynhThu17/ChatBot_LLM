using ChatBot_LLM.Data;
using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Services;
using ChatBot_LLM.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IChatbotService, GeminiChatbotService>();

// Sử dụng DbContextFactory thay vì AddDbContext
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<FAQService>();
builder.Services.AddScoped<ChatHistoryService>();
builder.Services.AddHttpContextAccessor();

// Thêm ChatManager và ChatUIHandler
builder.Services.AddScoped<ChatManager>();
builder.Services.AddScoped<ChatUIHandler>();

builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();