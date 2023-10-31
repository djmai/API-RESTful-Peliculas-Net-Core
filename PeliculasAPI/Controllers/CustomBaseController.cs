using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
	public class CustomBaseController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public CustomBaseController(ApplicationDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}

		protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
		{
			var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
			var dtos = mapper.Map<List<TDTO>>(entidades);
			return dtos;
		}

		protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
		{
			var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

			if (entidad == null)
			{
				return NotFound();
			}

			return mapper.Map<TDTO>(entidad);
		}

		protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>(TCreacion creacionDTO, string nombreRuta) where TEntidad: class, IId
		{
			var entidad = mapper.Map<TEntidad>(creacionDTO);
			context.Add(entidad);
			await context.SaveChangesAsync();

			var dtoLectura = mapper.Map<TLectura>(entidad);

			return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
		}

		protected async Task<ActionResult> Put<TCreacion, TEntidad>(int id, TCreacion creacionDTO) where TEntidad : class, IId
		{
			var entidad = mapper.Map<TEntidad>(creacionDTO);
			entidad.Id = id;
			context.Entry(entidad).State = EntityState.Modified;
			await context.SaveChangesAsync();
			return NoContent();
		}

		protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new ()
		{
			var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
			if (!existe)
				return NotFound();

			context.Remove(new TEntidad() { Id = id });
			await context.SaveChangesAsync();

			return NoContent();
		}
	}
}
