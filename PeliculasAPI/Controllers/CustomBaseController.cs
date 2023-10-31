using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
	}
}
