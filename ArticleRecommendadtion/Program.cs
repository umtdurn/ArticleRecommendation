using ArticleRecommendadtion.AbstractServices.ApiServiceAbstract;
using ArticleRecommendadtion.AbstractServices.BusinessServiceAbstract;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using ArticleRecommendadtion.ConcreteServices.ApiServiceConcrete;
using ArticleRecommendadtion.ConcreteServices.BusinessServiceConcrete;
using ArticleRecommendadtion.ConcreteServices.MongoDbConcrete;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ArticleRecommendadtion;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpClient();
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IMongoDbService, MongoDbService>();
        builder.Services.AddScoped<IBusinessService, BusinessService>();
        builder.Services.AddScoped<IApiService, ApiService>();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.Name = "ArticleRecomm.Auth";
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Login";
        });

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

