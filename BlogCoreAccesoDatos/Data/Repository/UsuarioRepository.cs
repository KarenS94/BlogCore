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
    public class UsuarioRepository : Repository<AplicationUser>, IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;
        public UsuarioRepository(ApplicationDbContext db) : base(db)
        {

            _db = db;

        }
        public void BloquearUsuario(string IdUsuario)
        {

            var usuarioDesdeBd = _db.AplicationUsers.FirstOrDefault(u => u.Id == IdUsuario);
            usuarioDesdeBd.LockoutEnd = DateTime.Now.AddDays(1000);
            _db.SaveChanges();
            
        }

        public void DesbloquearUsuario(string IdUsuario)
        {
            var usuarioDesdeBd = _db.AplicationUsers.FirstOrDefault(u => u.Id == IdUsuario);
            usuarioDesdeBd.LockoutEnd = DateTime.Now;
            _db.SaveChanges();
        }
    }
}
