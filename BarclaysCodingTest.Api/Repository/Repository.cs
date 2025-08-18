using BarclaysCodingTest.Api.Database;
using BarclaysCodingTest.Api.Entities;
using BarclaysCodingTest.Api.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Api.Repository;

public class Repository<T>(ApplicationDbContext applicationDbContext) : IRepository<T> where T : BaseEntity
{
    private DbSet<T> _dbSet = applicationDbContext.Set<T>();

    public T Add(T entity)
    {
        var addedEntity = _dbSet.Add(entity);
        return addedEntity.Entity;
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public T Update(T entity)
    {
        var updatedEntity = _dbSet.Update(entity);
        return updatedEntity.Entity;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await applicationDbContext.SaveChangesAsync();
    }
}
