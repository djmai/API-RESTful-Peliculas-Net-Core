using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PeliculasActores>()
				.HasKey(x => new { x.ActorId, x.PeliculaId });

			modelBuilder.Entity<PeliculasGeneros>()
				.HasKey(x => new { x.GeneroId, x.PeliculaId });

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Genero> Generos { get; set; }

		public DbSet<Actor> Actores { get; set; }

		public DbSet<Pelicula> Peliculas { get; set; }

		public DbSet<PeliculasActores> PeliculasActores { get; set; }
		public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }

	}
}
