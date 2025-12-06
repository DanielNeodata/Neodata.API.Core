namespace neodataEcosystem
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
				{
					Version = "v1",
					Title = "Neodata Ecosystem API",
					Description = "<h3>API for Neodata Ecosystem</h3>"
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//if (env.IsProduction() || env.IsDevelopment())
			//{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neodata Ecosystem API V1");
				c.RoutePrefix = "neodataEcosystem.v1";
			});
			//}

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
