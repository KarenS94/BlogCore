using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models.ViewModels
{
	public class ArticuloMV
	{
		//hacemos un llamado a la entidad principal
		public  Articulo Articulo { get; set; }

		public IEnumerable<SelectListItem> ListaCategorias { get; set; }

	}
}
 