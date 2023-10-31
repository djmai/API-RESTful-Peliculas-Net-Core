using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<Genero, GeneroDTO>().ReverseMap();

			CreateMap<GeneroCreacionDTO, Genero>();

			CreateMap<Actor, ActorDTO>().ReverseMap();

			CreateMap<ActorCreacionDTO, Actor>()
				.ForMember(x => x.Foto, options => options.Ignore());

			CreateMap<ActorPatchDTO, Actor>().ReverseMap();

			CreateMap<Pelicula, PeliculaDTO>().ReverseMap();

			CreateMap<PeliculaCreacionDTO, Pelicula>()
				.ForMember(x => x.Poster, options => options.Ignore())
				.ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
				.ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

			CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
		}

		private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
		{
			var resultado = new List<PeliculasGeneros>();
			if (peliculaCreacionDTO.GenerosIDs == null)
				return resultado;

			foreach (var id in peliculaCreacionDTO.GenerosIDs)
			{
				resultado.Add(new PeliculasGeneros() { GeneroId = id });
			}

			return resultado;
		}

		private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
		{
			var resultado = new List<PeliculasActores>();
			if (peliculaCreacionDTO.GenerosIDs == null)
				return resultado;

			foreach (var actor in peliculaCreacionDTO.Actores)
			{
				resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
			}

			return resultado;
		}
	}
}
