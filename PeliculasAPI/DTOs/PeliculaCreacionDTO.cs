using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
	public class PeliculaCreacionDTO
	{
		[Required]
		[StringLength(300)]
		public string Titulo { get; set; }

		public bool EnCines { get; set; }

		public DateTime FechaEstreno { get; set; }

		[PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
		[TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
		public IFormFile Poster { get; set; }
	}
}
