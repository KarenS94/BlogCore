using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Areas.Admin.Controllers;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogCore.Areas.Cliente.Controllers

{
    [Area("Cliente")]
    public class HomeController : Controller
    {
       private readonly IContenedorTrabajo _contenedorTrabajo;

        public HomeController(IContenedorTrabajo contenedorTrabajo)
        {
          _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Sliders = _contenedorTrabajo.Slider.GetAll(),
                ListArticulo = _contenedorTrabajo.Articulo.GetAll(),
            };

            // esto nos detecta si estamos en la pag de inicio
            ViewBag.IsHome = true;

            return View(homeVM);
        }

        [HttpGet]
        public IActionResult ResultadoBusqueda(string searchString, int page =1, int pageSize=6)
        {
           
            var articulos = _contenedorTrabajo.Articulo.AsQueryable();

            // filtrar un articulo si hay un termino de busqueda

            if (!string.IsNullOrEmpty(searchString))
            {
                articulos = articulos.Where(e => e.Nombre.Contains(searchString));
                
            }
            //paginar los resultados

            var paginatedEntries = articulos.Skip((page -1)*pageSize).Take(pageSize);
            //crear el modelo para la vista
            var model = new ListaPaginada<Articulo>(paginatedEntries.ToList(), articulos.Count(), page, pageSize, searchString);
            return View(model);

        }

       


        [HttpGet]
        public IActionResult Detalle(int id)
        {

            var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(id);
            return View(articuloDesdeBd);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
