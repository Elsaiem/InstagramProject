using Hangfire;
using InstagramProject.Core.Helpers;
using InstagramProject.Core.Service_contract;
using InstagramProject.Core.ServiceContract;
using InstagramProject.Service.Comment;
using InstagramProject.Service.Services.Authentication;
using InstagramProject.Service.Services.EmailService;
using InstagramProject.Service.Services.Files;
using InstagramProject.Service.Services.Home;
using InstagramProject.Service.Services.Post;
using InstagramProject.Service.Services.Profile;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Service
{
    public static class DependencyInjection
    {
		public static IServiceCollection AddServicesDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IEmailSender, EmailService>();
			services.AddScoped<IHomeService, HomeService>();
			services.AddScoped<IFileService, CloudinaryService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IProfileService, ProfileService>();
			services.AddScoped<ICommentService, CommentService>();
			services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
			
			services.AddBackgroundJobsConfig(configuration);
			return services;
		}
		private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHangfire(config => config
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

			services.AddHangfireServer();

			return services;
		}
	}
}
