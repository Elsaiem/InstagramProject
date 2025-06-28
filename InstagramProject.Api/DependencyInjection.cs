using InstagramProject.Repository.Authentication;
using Microsoft.Extensions.Options;

namespace InstagramProject.Api
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddAPIDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllers();
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
					builder
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
				);
			});
			services.AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleAuth>>().Value);
			services.Configure<GoogleAuth>(configuration.GetSection(GoogleAuth.SectionName));
			services.AddSwaggerServices();
			services.AddHttpContextAccessor();
			return services;
		}
		private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
		{
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			return services;
		}
	}
}
