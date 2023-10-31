using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
	[ApiController]
	[Route("api/peliculas")]
	public class PeliculasController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IAlmacenadorArchivos almacenadorArchivos;
		private readonly string contenedor = "peliculas";

		public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
		{
			this.context = context;
			this.mapper = mapper;
			this.almacenadorArchivos = almacenadorArchivos;
		}

		[HttpGet]
		public async Task<ActionResult<PeliculasIndexDTO>> Get()
		{
			var top = 5;
			var hoy = DateTime.Today;

			var proximosEstrenos = await context.Peliculas
				.Where(x => x.FechaEstreno > hoy)
				.OrderBy(x => x.FechaEstreno)
				.Take(top)
				.ToListAsync();

			var enCines = await context.Peliculas
				.Where(x => x.EnCines)
				.Take(top)
				.ToListAsync();

			var resultado = new PeliculasIndexDTO();

			resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
			resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

			return resultado;

			//var peliculas = await context.Peliculas.ToListAsync();
			//return mapper.Map<List<PeliculaDTO>>(peliculas);
		}

		[HttpGet("filtro")]
		public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
		{
			var peliculasQueryable = context.Peliculas.AsQueryable();

			if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
			{
				peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
			}

			if (filtroPeliculasDTO.EnCines)
			{
				peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
			}

			if (filtroPeliculasDTO.ProximosEstrenos)
			{
				var hoy = DateTime.Today;
				peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
			}

			if (filtroPeliculasDTO.GeneroId != 0)
			{
				peliculasQueryable = peliculasQueryable
					.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
					.Contains(filtroPeliculasDTO.GeneroId));
			}

			await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina);

			var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

			return mapper.Map<List<PeliculaDTO>>(peliculas);
		}

		[HttpGet("{id:int}", Name = "obtenerPelicula")]
		public async Task<ActionResult<PeliculaDetallesDTO>> Get(int id)
		{
			var pelicula = await context.Peliculas
				.Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
				.Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (pelicula == null)
				return NotFound();

			pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

			return mapper.Map<PeliculaDetallesDTO>(pelicula);
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
		{
			var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

			if (peliculaCreacionDTO.Poster != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream); // Copiando hacia el memoryStream
					var contenido = memoryStream.ToArray(); // Guardando bytes en variable
					var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName); // Extrayendo extension del archivo
					pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType);// Guardamos string de la url
				}
			}

			AsignarOrdenActores(pelicula); // Asignación de orden
			context.Add(pelicula);
			await context.SaveChangesAsync(); // Guardando registro en la DB

			var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
			return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);
		}

		private void AsignarOrdenActores(Pelicula pelicula)
		{
			if (pelicula.PeliculasActores != null)
			{
				for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
				{
					pelicula.PeliculasActores[i].Orden = i;
				}
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
		{
			var peliculaDB = await context.Peliculas
				.Include(x => x.PeliculasActores)
				.Include(x => x.PeliculasGeneros)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (peliculaDB == null) { return NotFound(); }

			peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

			if (peliculaCreacionDTO.Poster != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream); // Copiando hacia el memoryStream
					var contenido = memoryStream.ToArray(); // Guardando bytes en variable
					var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName); // Extrayendo extension del archivo
					peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);// Guardamos string de la url
				}
			}

			AsignarOrdenActores(peliculaDB); // Asignación de orden
			await context.SaveChangesAsync(); // Guardando registro en la DB
			return NoContent();
		}

		[HttpPatch("{id}")]
		public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
		{
			if (patchDocument == null)
			{
				return BadRequest();
			}

			var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

			if (entidadDB == null)
			{
				return NotFound();
			}

			var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);

			patchDocument.ApplyTo(entidadDTO, ModelState);

			var esValido = TryValidateModel(entidadDTO);
			if (!esValido)
			{
				return BadRequest(ModelState);
			}

			mapper.Map(entidadDTO, entidadDB);
			await context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Detele(int id)
		{
			var existe = await context.Peliculas.AnyAsync(x => x.Id == id);
			if (!existe)
				return NotFound();

			context.Remove(new Pelicula() { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}
	}
}
