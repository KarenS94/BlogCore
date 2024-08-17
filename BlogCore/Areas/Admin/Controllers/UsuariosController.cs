using BlogCore.AccesoDatos.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        // instanciamos a la unidad de trabajo
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public UsuariosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {

            // opcion 1: Obtener todos los usuarios
          //  return View(_contenedorTrabajo.Usuario.GetAll());


            //opcion 2: obtener todos los usuarios menos el que esta logeado, para no bloquearse el mismo

           var claimsIdentity = (ClaimsIdentity)this.User.Identity; // esta linea se esta obteniendo la identidad actual de usuario que esta autenticado en la aplicacion
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_contenedorTrabajo.Usuario.GetAll(u => u.Id != usuarioActual.Value));
        }
		[HttpGet]
		public IActionResult Bloquear(string id)
		{
            if (id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.BloquearUsuario(id);
            return RedirectToAction(nameof(Index));
		}
		public IActionResult Desbloquear(string id)
		{
			if (id == null)
			{
				return NotFound();
			}
			_contenedorTrabajo.Usuario.DesbloquearUsuario(id);
			return RedirectToAction(nameof(Index));
		}
	}
}
