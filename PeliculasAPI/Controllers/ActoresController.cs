using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
	[ApiController]
	[Route("api/actores")]
	public class ActoresController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public ActoresController(ApplicationDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
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
		public async Task<ActionResult> Post([FromBody] ActorCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);
			context.Add(entidad);
			await context.SaveChangesAsync();

			var dto = mapper.Map<ActorDTO>(entidad);
			return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
		}

		[HttpPut]
		public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
		{
			var entidad = mapper.Map<Genero>(generoCreacionDTO);
			entidad.Id = id;
			context.Entry(entidad).State = EntityState.Modified;
			await context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPut]
		public async Task<ActionResult> Put(int id, [FromBody] ActorCreacionDTO actorCreacionDTO)
		{
			var entidad = mapper.Map<Actor>(actorCreacionDTO);
			entidad.Id = id;
			context.Entry(entidad).State = EntityState.Modified;
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
