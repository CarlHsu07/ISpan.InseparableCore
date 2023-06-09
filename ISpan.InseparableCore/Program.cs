using ISpan.InseparableCore.Controllers;
using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using NuGet.Configuration;
using System.Configuration;
using ISpan.InseparableCore.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromDays(1) ); // 加入Session，過期時間設為1天
builder.Services.AddSignalR(); //加入 SignalR
builder.Services.AddHttpContextAccessor();

// InseparableContext
builder.Services.AddDbContext<InseparableContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("InseparableConnection")));

//api key
builder.Services.Configure<ApiKeys>(builder.Configuration.GetSection("ApiKeys"));

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    //pattern: "{controller=Member}/{action=EditProfile}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
