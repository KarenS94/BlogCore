using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace BlogCore.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CategoriasController : Controller
    {
        //con el uso de los repositorio en este controlador llamamos al contenedor de trabajo
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public IActionResult Create()// aqui se crea el campo formulario
        {
            return View();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]// prevenir ataques xxs
		public IActionResult Create(Categoria categoria)// aqui se crea el campo formulario
		{
            // aqui validamos el formulario

            if (ModelState.IsValid)
            { 
                // guardar en Bd
                _contenedorTrabajo.Categoria.Add(categoria);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            
            }

			return View(categoria);// si no es valido retorna al mismo formulario
		}


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Categoria categoria = new Categoria();// instanciamos
            categoria = _contenedorTrabajo.Categoria.Get(id); //accedemos por el contenedor de trab a categoria y al metodo get
            if (categoria == null)// VALIDAMOS SI NO EXISTE CATEGORIA RETORNAMOS NOT FUND Y SI EXISTE LE PASAMOS EL PARAMETRO CATEGORIA
            {
                return NotFound();  
            }
            return View(categoria);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]// 
		public IActionResult Edit(Categoria categoria)
		{
			// aqui validamos el formulario

			if (ModelState.IsValid)
			{
				// guardar en Bd
				_contenedorTrabajo.Categoria.Update(categoria);
				_contenedorTrabajo.Save();
				return RedirectToAction(nameof(Index));

			}

			return View(categoria);// si no es valido retorna al mismo formulario
		}



		// aqui usamos el repositorio creando y cerrando region xq usamos datatable
		#region llamada a la api
		[HttpGet]
        // creamos un metodo para obtener o consultar datos
        public IActionResult GetAll() 
        {
            return Json(new { Data = _contenedorTrabajo.Categoria.GetAll() });
            
        }

        // usamos aqui para utilizar el metodo sweetalert

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Categoria.Get(id); // si existe el id ahi quedaria guardado
            if (objFromDb  == null)
            {
                // si no existe una categoria va  retornar ese msj en json
                return Json(new { success = false, message = "Error borrando categoria" });
            }
            
            _contenedorTrabajo.Categoria.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Categoria borrada correctamente" });
		}

		#endregion
	}

}

