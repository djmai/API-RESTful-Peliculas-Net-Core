namespace PeliculasAPI.DTOs
{
	public class PaginacionDTO
	{
		public int Pagina { get; set; } = 1;

		private int cantidadRegistrosPorPagina { get; set; } = 10;

		private readonly int cantidadMaximaRegistrosPorPagina = 50;

		public int CantidadRegistrosPorPagina
		{
			get => cantidadRegistrosPorPagina;

			set
			{
				cantidadRegistrosPorPagina = (value > cantidadMaximaRegistrosPorPagina) ? cantidadMaximaRegistrosPorPagina : value;
			}
		}
	}
}
