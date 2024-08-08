using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]// especificar el area donde estara ese controlador
    public class SliderController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment; //accede a las carpetas de la raiz y guarda la imagen  en una ubicacion de imagen

        public SliderController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create() 
        {  //creamos el formulario y mandamos a llamar a dos tablas

            ArticuloMV artiMV = new ArticuloMV()
            {
                Articulo = new BlogCore.Models.Articulo(),// pasamos una instancia al modelo articulo para poder que los campos se adapten al nombra, la descripcion, etc
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(artiMV); 
        }

        // accion editar, lo primero que necesitamos es mostrar el formulario
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloMV artiMV)
        {
            if (ModelState.IsValid) 
            { 
                string rutaPrincipal = _hostingEnvironment.WebRootPath; // accedemos a la ruta principal q es wwwroot
                var archivos = HttpContext.Request.Form.Files; // aqui accedemos a la carpeta. accedemos al formulario y a los archivos de ese form
                if (artiMV.Articulo.Id == 0 && archivos.Count()> 0) // quiere decir q se esta validando si se va a crear un articulo nuevo
                {
                    //   nuevo articulo
                    string nombreArchivo = Guid.NewGuid().ToString();//cada imagen tenga un nombre unico
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);// obtenemos del filename la extension del archivo

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create)) // para crear un archivo nuevo
					{
                        archivos[0].CopyTo(fileStreams); // para copiarlo en memoria
                    }
                    artiMV.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension; // aqui se guarda la ruta real que es donde se guarda el archivo
                    artiMV.Articulo.FechaCreacion = DateTime.Now.ToString();

                    //para guardar todo el articulo junto con la imagen y se va a crear en la base de datos

                    _contenedorTrabajo.Articulo.Add(artiMV.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Imagen", "Debes seleccionar una imagen");
                }
            }


            artiMV.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            return View(artiMV);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {

			ArticuloMV artiMV = new ArticuloMV()
			{
				Articulo = new BlogCore.Models.Articulo(),
				ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
			};

            if (id != null)//si id no es igual a nulo entonces se envia el articulo
            {
                artiMV.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());// aqui obtenemos el id del articulo
            }

            return View(artiMV);// si existe pasamos la vista
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ArticuloMV artiMV)
		{
			if (!ModelState.IsValid)
			{
				string rutaPrincipal = _hostingEnvironment.WebRootPath; 
				var archivos = HttpContext.Request.Form.Files;


                var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(artiMV.Articulo.Id);

				if (archivos.Count() > 0) 
				{
					//   nueva imagen para el articulo
					string nombreArchivo = Guid.NewGuid().ToString();
					var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
					var extension = Path.GetExtension(archivos[0].FileName);
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeBd.UrlImagen.TrimStart('\\'));
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }
                    // SUBIMOS EL ARCHIVO DE NUEVO

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create)) 
					{
						archivos[0].CopyTo(fileStreams); 
					}
					artiMV.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension; 
					artiMV.Articulo.FechaCreacion = DateTime.Now.ToString();

					_contenedorTrabajo.Articulo.Update(artiMV.Articulo);
					_contenedorTrabajo.Save();

					return RedirectToAction(nameof(Index));
				}
				else
				{
					// mantener la imagen y cambiar los cambios
                    artiMV.Articulo.UrlImagen = articuloDesdeBd.UrlImagen;
				}
				_contenedorTrabajo.Articulo.Update(artiMV.Articulo);
				_contenedorTrabajo.Save();

				return RedirectToAction(nameof(Index));
			}

			artiMV.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
			return View(artiMV);
		}


		#region Llamadas a la Api
		[HttpGet]
		// creamos un metodo para obtener o consultar datos
		public IActionResult GetAll()
		{
			return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties:"Categoria")});// pone nombre con la tabla relacionada a articulos

		}

		public IActionResult Delete(int id)
		{
			var articuloDesdeBd= _contenedorTrabajo.Articulo.Get(id); // accedemos al articulo
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;//para borrar la imagen de la carpeta
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, articuloDesdeBd.UrlImagen.TrimStart('\\'));// ubicacion de la imagen para borrarla
			if (System.IO.File.Exists(rutaImagen))
			{
				System.IO.File.Delete(rutaImagen);
			}

			if (articuloDesdeBd== null)
			{
				return Json(new { success = false, message = "Error borrando articulo" });
			}

			_contenedorTrabajo.Articulo.Remove(articuloDesdeBd);
			_contenedorTrabajo.Save();
			return Json(new { success = true, message = "Articulo borrado correctamente" });
		}
		#endregion

	}
}

// el GetAll para  obtener del repositorio generico los datos de la tabla articulo.