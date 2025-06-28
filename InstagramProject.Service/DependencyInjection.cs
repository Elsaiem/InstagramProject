using Hangfire;
using InstagramProject.Core.Service_contract;
using InstagramProject.Service.Services.Authentication;
using InstagramProject.Service.Services.EmailService;
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
