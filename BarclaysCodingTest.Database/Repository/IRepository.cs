namespace BarclaysCodingTest.Database.Repository;

public interface IRepository<T> 
{
    T Add(T entity);
    IQueryable<T> GetAll();
    T Update(T entity);
    void Delete(T user);
    Task SaveChangesAsync();
}
