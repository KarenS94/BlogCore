using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db; // este se va a encargar de poder hacer el acceso a los datos

        public ContenedorTrabajo(ApplicationDbContext db)

        {
           _db = db;
           Categoria = new CategoriaRepository(_db); // aqui ya queda encapsulado
           Articulo = new ArticuloRepository(_db);
           Slider = new SliderRepository(_db);
        }

        public ICategoriaRepository Categoria {  get; private set; } // para usar esta categoria mandamos a llamar al constructor
        public IArticuloRepository Articulo { get; private set; }

		public ISliderRepository Slider { get; private set; }

		public void Dispose()
        {
           _db.Dispose(); // libera los recursos una vez que se abra una conexion a la bd y la oper q haga el guardado etc.
        }

        public void Save() // es para guardar los cambios
        {
            _db.SaveChanges();
        }
    }
}
