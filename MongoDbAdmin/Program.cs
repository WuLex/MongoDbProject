using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;

namespace MongoDbAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var Configuration = builder.Configuration;
            
            //Configuration.SetBasePath(Directory.GetCurrentDirectory());

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<MongoSettings>(Configuration.GetSection("MongoSettings"));
         
            var connectionString = Configuration.GetConnectionString("MongoDBConnection");
            builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
            builder.Services.AddScoped<IMongoDatabase>(provider => provider.GetService<IMongoClient>().GetDatabase("CenterDb"));
            //builder.Services.AddSingleton(new MongoClient(connectionString));
            //builder.Services.AddScoped<IMongoDatabase>(provider =>
            //{
            //    var client = provider.GetService<MongoClient>();
            //    return client.GetDatabase("YourDatabaseName");
            //});
            builder.Services.AddTransient<VideoService>();
            builder.Services.AddTransient<GridFSService>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}