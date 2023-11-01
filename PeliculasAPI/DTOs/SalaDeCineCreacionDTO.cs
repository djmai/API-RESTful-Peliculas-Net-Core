using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
	public class SalaDeCineCreacionDTO
	{
		[Required]
		[StringLength(120)]
		public string Nombre { get; set; }

		[Range(-90, 90)]
		public double Latitud { get; set; }

		[Range(-180, 180)]
		public double Longitud { get; set; }
	}
}
