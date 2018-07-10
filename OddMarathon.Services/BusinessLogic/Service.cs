using OddMarathon.Dal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Services.BusinessLogic
{
    public class Service<T> : IService<T> where T : class
    {
        private IRepository<T> _repo;
        public Service(IRepository<T> repo)
        {
            _repo = repo;
        }
        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await _repo.Count(predicate);
        }

        public void Create(T entity)
        {
            _repo.Create(entity);
        }

        public void Delete(T entity)
        {
            _repo.Delete(entity);

        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await _repo.Find(predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<T> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        public void Update(T entity)
        {
            _repo.Update(entity);
        }
    }
}
