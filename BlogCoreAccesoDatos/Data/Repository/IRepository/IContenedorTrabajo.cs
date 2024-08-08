using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
 public  interface IContenedorTrabajo:  IDisposable
    {
        //Aqui  se deben ir agregando los diferentes repositorios, IRepositorio  no xq ICategoria se implementa de IRepositorio.
        ICategoriaRepository Categoria { get; }
        IArticuloRepository Articulo { get; }
		ISliderRepository Slider { get; }



		// hacemos el metodo save para guardar los cambios realizados en la unidad de trabajo

		void Save();
    }
}
