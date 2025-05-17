using SeaQuailDiagramTool.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Domain.Services
{
    public interface IPersistenceService<T> where T : IHaveID
    {
        Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> criteria);
        Task<T> GetById(Guid id);
        Task<T> Save(T item);
        Task Delete(Guid id);
    }
    public static class IPersistenceServiceExtensions
    {
        public static async Task<T> GetOne<T>(this IPersistenceService<T> persistence, Expression<Func<T, bool>> criteria) where T : IHaveID
        {
            var result = await persistence.Filter(criteria);
            return result.FirstOrDefault();
        }
        public static async Task<List<T>> GetList<T>(this IPersistenceService<T> persistence, Expression<Func<T, bool>> criteria) where T : IHaveID
        {
            var result = await persistence.Filter(criteria);
            return result.ToList();
        }
    }
}
