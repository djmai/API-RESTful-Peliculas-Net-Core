using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using PeliculasAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Tests
{
	public class BasePruebas
	{
		protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
		protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

		protected ApplicationDbContext ConstruirContext(string nombreDB)
		{
			var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(nombreDB).Options;

			var dbContext = new ApplicationDbContext(opciones);
			return dbContext;
		}

		protected IMapper ConfigurarAutoMapper()
		{
			var config = new MapperConfiguration(options =>
			{
				var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
				options.AddProfile(new AutoMapperProfiles(geometryFactory));
			});

			return config.CreateMapper();
		}

		protected ControllerContext ConstruirControllerContext()
		{
			var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
				new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
				new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
			}));

			return new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = usuario }
			};
		}
	}
}
