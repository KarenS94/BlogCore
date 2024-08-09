using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
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
        {  
            return View(); 
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid) 
            { 
                string rutaPrincipal = _hostingEnvironment.WebRootPath; 
                var archivos = HttpContext.Request.Form.Files; 
                if (archivos.Count() > 0) 
                {
                    //   nuevo slider
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create)) 
					{
                        archivos[0].CopyTo(fileStreams); 
                    }
                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension; 
            

                    //para guardar todo el articulo junto con la imagen y se va a crear en la base de datos

                    _contenedorTrabajo.Slider.Add(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Imagen", "Debes seleccionar una imagen");
                }
            }
            return View(slider);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {

            if (id != null)
            {
                var slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
                return View(slider);
            }

            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Slider slider)
		{
            if (!ModelState.IsValid)
            {

                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var sliderDesdeBd = _contenedorTrabajo.Slider.Get(slider.Id);

                if (archivos.Count() > 0)
                {
                    //   nuevo slider
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    //var nuevaExtension= Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaPrincipal, sliderDesdeBd.UrlImagen.TrimStart('\\'));
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }
                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension;


                    //para guardar todo el articulo junto con la imagen y se va a crear en la base de datos

                    _contenedorTrabajo.Slider.Update(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // cuando la imagen existe y se conserva
                    slider.UrlImagen = sliderDesdeBd.UrlImagen;

                }

                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));

            }
			    
                return View(slider);
		}






		#region Llamadas a la Api
		[HttpGet]
		// creamos un metodo para obtener o consultar datos
		public IActionResult GetAll()
		{
			return Json(new { data = _contenedorTrabajo.Slider.GetAll()});

		}

		public IActionResult Delete(int id)
		{
            var sliderDesdeBd = _contenedorTrabajo.Slider.Get(id);
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, sliderDesdeBd.UrlImagen.TrimStart('\\'));
			if (System.IO.File.Exists(rutaImagen))
			{
				System.IO.File.Delete(rutaImagen);
			}

			if (sliderDesdeBd== null)
			{
				return Json(new { success = false, message = "Error borrando slider" });
			}

			_contenedorTrabajo.Slider.Remove(sliderDesdeBd);
			_contenedorTrabajo.Save();
			return Json(new { success = true, message = "Slider borrado correctamente" });
		}
		#endregion

	}
}

// el GetAll para  obtener del repositorio generico los datos de la tabla articulo.