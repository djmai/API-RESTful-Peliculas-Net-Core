namespace PeliculasAPI.DTOs
{
	public class FiltroPeliculasDTO
	{
		public int Pagina { get; set; } = 1;

		public int CantidadRegistrosPorPagina { get; set; } = 10;

		public PaginacionDTO Paginacion
		{
			get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistrosPorPagina = CantidadRegistrosPorPagina }; }
		}

		public string Titulo { get; set; }

		public int GeneroId { get; set; }

		public bool EnCines { get; set; }

		public bool ProximosEstrenos { get; set; }
	}
}
