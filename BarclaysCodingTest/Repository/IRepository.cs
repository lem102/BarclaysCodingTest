using BarclaysCodingTest.Utilities;

namespace BarclaysCodingTest.Repository;

public interface IRepository<T> 
{
    Task<T> AddAsync(T entity);
    IQueryable<T> GetAll();
    Task<Result> SaveChangesAsync();
}
