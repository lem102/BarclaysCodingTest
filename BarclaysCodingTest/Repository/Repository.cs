using BarclaysCodingTest.Database;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BarclaysCodingTest.Repository;

public class Repository<T>(ApplicationDbContext applicationDbContext) : IRepository<T> where T : BaseEntity
{
    private DbSet<T> _dbSet = applicationDbContext.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        var addedEntity = await _dbSet.AddAsync(entity);

        return addedEntity.Entity;
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<Result> SaveChangesAsync()
    {
        await applicationDbContext.SaveChangesAsync();
        return Result.Success();
    }
}
