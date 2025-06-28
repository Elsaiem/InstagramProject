
using InstagramProject.Core;
using InstagramProject.Repository.Data.Contexts;
using InstagramProject.Repository;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using InstagramProject.Service;

namespace InstagramProject.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAPIDependencies(builder.Configuration);
            builder.Services.AddRepositoryDependencyInjection(builder.Configuration);
            builder.Services.AddCoreDependencyInjection(builder.Configuration);
            builder.Services.AddServicesDependencyInjection(builder.Configuration);
            builder.Services.AddSignalR();

            var app = builder.Build();

            using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            // await ApplicationContextSeed.SeedAsync(context);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter{
                        User=app.Configuration.GetValue<string>("HangfireSettings:Username"),
                        Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                ],
                DashboardTitle = "Instagram Dashboard",
            });
            app.UseHangfireDashboard("/jobs");
            app.UseAuthorization();


            app.MapControllers();
           // app.MapHub<NotificationHub>("/notificationHub").RequireCors("SignalRCors");
            app.UseStaticFiles();
            app.Run();

        }
    }
}
