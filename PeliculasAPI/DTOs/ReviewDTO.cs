using Microsoft.AspNetCore.Identity;
using PeliculasAPI.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
	public class ReviewDTO
	{
		public int Id { get; set; }

		public string Comentario { get; set; }

		public int Puntuacion { get; set; }

		public int PeliculaId { get; set; }

		public string UsuarioId { get; set; }

		public string NombreUsuario { get; set; }
	}
}
