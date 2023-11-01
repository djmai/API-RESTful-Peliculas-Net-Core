using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Servicios;

namespace PeliculasAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			// Configuración de AutoMapper
			services.AddAutoMapper(typeof(Startup));

			// Agregando servicio para almacenar archivos en Azure Storage
			// services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

			// Agregando servicio para almacenar archivos en local
			services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
			services.AddHttpContextAccessor();

			// Configuración de conexión con la base de datos
			services.AddDbContext<ApplicationDbContext>(
				options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"),
				sqlServerOptions => sqlServerOptions.UseNetTopologySuite()
				));

			services.AddControllers()
				.AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles(); // Sirviendo contenido estatico

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
