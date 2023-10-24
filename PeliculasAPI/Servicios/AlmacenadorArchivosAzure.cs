using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;

namespace PeliculasAPI.Servicios
{
	public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
	{
		private readonly string connectionString;
		public AlmacenadorArchivosAzure(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("AzureStorage");
		}

		public async Task BorrarArchivo(string ruta, string contenedor)
		{
			if (string.IsNullOrEmpty(ruta))
			{
				return;
			}

			var cliente = new BlobContainerClient(connectionString, contenedor); // Conectamos con Azure
			await cliente.CreateIfNotExistsAsync(); // Creamos la carpeta si no existe
			var archivo = Path.GetFileName(ruta);
			var blob = cliente.GetBlobClient(archivo);
			await blob.DeleteIfExistsAsync(); // Borramos el archivo del servidor
		}

		public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
		{
			await BorrarArchivo(ruta, contenedor); // Eliminamos el archivo
			return await GuardarArchivo(contenido, extension, contenedor, contentType); // Guardamos el archivo nuevo
		}

		public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
		{
			var cliente = new BlobContainerClient(connectionString, contenedor); // Conectamos con Azure
			await cliente.CreateIfNotExistsAsync(); // Creamos la carpeta si no existe
			cliente.SetAccessPolicy(PublicAccessType.Blob); // Agregamos politicas de seguridad

			var archivoNombre = $"{Guid.NewGuid()}{extension}"; // Generando nombre de archivo
			var blob = cliente.GetBlobClient(archivoNombre);

			var blobUploadOptions = new BlobUploadOptions(); // Configurando opciones de carga
			var blobHttpHeader = new BlobHttpHeaders();
			blobHttpHeader.ContentType = contentType;  // Pasamos el tipo
			blobUploadOptions.HttpHeaders = blobHttpHeader;

			await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions); // Empujando imagen al servidor

			return blob.Uri.ToString(); // Obtenemos URL del archivo

		}
	}
}
