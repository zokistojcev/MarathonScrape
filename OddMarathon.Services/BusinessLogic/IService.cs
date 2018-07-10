using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Services.BusinessLogic
{
    public interface IService<T> where T: class
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }
}
