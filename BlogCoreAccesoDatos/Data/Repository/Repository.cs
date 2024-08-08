using BlogCore.AccesoDatos.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class

    {
        // aqui vamos a trabajar con los datos entonces aqui vamos a trabajar con el contexto
        protected readonly DbContext Context;
        internal DbSet<T> dbSet;
        // aqui hacemos el constructor de la clase

        public Repository(DbContext context)
        {
            // aqui hacemos la inyeccion de dependencia, con el contexto podemos acceder a los datos
            Context = context;
            this.dbSet = context.Set<T>();    
        }


        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(int id) // este es para obtener su registro a traves de su id
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
        {
            // aqui se recupera una coleccion de identidades de tipo T
            //Se crea una consulta IQueryable a partir del DbSet del contexto,si consultas categoria te va a traer la lista categoria, etc.
            IQueryable<T> query = dbSet; // AQUI ES UNA TABLA COMPLETA
            // aqui vamos a hacer una validacion de filter porque es un parametro que puede ser nulo, a menos q no sea nulo entonces muestra el filtro
            // Se aplica el filtro si se proporciona
            if (filter != null) // AQUI LA TRAE FILTRADA
            {
                query = query.Where(filter);
            }
            if (includeProperties != null) // AQUI LA TRAE RELACIONADA
            {
                foreach (var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            // para traer ordenado los datos, si no es nulo entonces si se estan tratando de ordenar los registros y se convierte la consulta en una lista
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList(); // sino quiso trae los datos ordenados, sin filtrarlo te devuelve todos los datos.
        }


        // este permite obtener registros de manera individual
        public T GetFirstOrderDefault(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            // primero hacemos la consulta
            IQueryable<T> query = dbSet;
            if (filter != null) // AQUI NECESITAMOS HACER EL FILTRADO
            {
                query = query.Where(filter);
            }

            if (includeProperties != null) // Tambien necesitamos el include property para extraer un registro individual asoc a otra tabla
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.FirstOrDefault(); 
        }

        public void Remove(int id)
        {
            T entityToRemove = dbSet.Find(id);
        }

        public void Remove(T inity)
        {
            dbSet.Remove(inity);
        }
    }
}

