using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
	[ApiController]
	[Route("api/actores")]
	public class ActoresController : CustomBaseController
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IAlmacenadorArchivos almacenadorArchivos;
		private readonly string contenedor = "actores";

		public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos) : base(context, mapper)
		{
			this.context = context;
			this.mapper = mapper;
			this.almacenadorArchivos = almacenadorArchivos;
		}

		[HttpGet]
		public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
		{
			return await Get<Actor, ActorDTO>(paginacionDTO);
		}

		[HttpGet("{id:int}", Name = "obtenerActor")]
		public async Task<ActionResult<ActorDTO>> Get(int id)
		{
			return await Get<Actor, ActorDTO>(id);
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

		[HttpPut("{id}")]
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
			return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Detele(int id)
		{
			return await Delete<Actor>(id);
		}
	}
}
