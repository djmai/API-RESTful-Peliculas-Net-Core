using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
	[ApiController]
	[Route("api/actores")]
	public class ActoresController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IAlmacenadorArchivos almacenadorArchivos;
		private readonly string contenedor = "actores";

		public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
		{
			this.context = context;
			this.mapper = mapper;
			this.almacenadorArchivos = almacenadorArchivos;
		}

		[HttpGet]
		public async Task<ActionResult<List<ActorDTO>>> Get()
		{
			var entidades = await context.Actores.ToListAsync();
			return mapper.Map<List<ActorDTO>>(entidades);
		}

		[HttpGet("{id:int}", Name = "obtenerActor")]
		public async Task<ActionResult<ActorDTO>> Get(int id)
		{
			var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

			if (entidad == null)
				return NotFound();

			var dto = mapper.Map<ActorDTO>(entidad);

			return dto;
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);

			if (actorCreacionDTO.Foto != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await actorCreacionDTO.Foto.CopyToAsync(memoryStream); // Copiando hacia el memoryStream
					var contenido = memoryStream.ToArray(); // Guardando bytes en variable
					var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName); // Extrayendo extension del archivo
					entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType);// Guardamos string de la url
				}
			}

			context.Add(entidad);
			await context.SaveChangesAsync(); // Guardando registro en la DB

			var dto = mapper.Map<ActorDTO>(entidad);
			return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
		}

		[HttpPut]
		public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
		{
			// Actualiza todos los datos cuando el usuario envia todo lo de ActorCreacionDTO
			// var entidad = mapper.Map<Actor>(actorCreacionDTO);
			// entidad.Id = id;
			// context.Entry(entidad).State = EntityState.Modified;

			// Actualizacion de los campos que son diferentes
			var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

			if (actorDB == null) { return NotFound(); }

			actorDB = mapper.Map(actorCreacionDTO, actorDB); // Mapedo de campos de actorCreacionDTO a actorDB - solo los que traigan dato

			if (actorCreacionDTO.Foto != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await actorCreacionDTO.Foto.CopyToAsync(memoryStream); // Copiando hacia el memoryStream
					var contenido = memoryStream.ToArray(); // Guardando bytes en variable
					var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName); // Extrayendo extension del archivo
					actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDB.Foto, actorCreacionDTO.Foto.ContentType);// Guardamos string de la url
				}
			}

			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPatch("{id}")]
		public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
		{
			if (patchDocument == null)
			{
				return BadRequest();
			}

			var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

			if (entidadDB == null)
			{
				return NotFound();
			}

			var entidadDTO = mapper.Map<ActorPatchDTO>(entidadDB);

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

		[HttpDelete("id")]
		public async Task<ActionResult> Detele(int id)
		{
			var existe = await context.Actores.AnyAsync(x => x.Id == id);
			if (!existe)
				return NotFound();

			context.Remove(new Actor() { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}
	}
}
