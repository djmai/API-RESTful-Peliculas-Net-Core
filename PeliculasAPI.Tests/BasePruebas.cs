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
	}
}
