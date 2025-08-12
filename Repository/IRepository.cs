namespace BarclaysCodingTest.Repository;

// TODO: add where t is entity or similar
public interface IRepository<T> 
{
    T Add(T entity);
}
