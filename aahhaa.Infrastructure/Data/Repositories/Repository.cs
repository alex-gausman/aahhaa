using aahhaa.Shared.Data.Interfaces;
using LiteDB;
using LiteDB.Async;
using System.Linq.Expressions;

namespace aahhaa.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ILiteDatabaseAsync _database;
    private readonly ILiteCollectionAsync<T> _collection;

    public Repository(ILiteDatabaseAsync database)
    {
        _database = database;
        _collection = _database.GetCollection<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await _collection.InsertAsync(entity);
    }

    public async Task DeleteAsync(object key)
    {
        await _collection.DeleteAsync(new BsonValue(key));
    }

    public async Task<T> GetAsync(object key)
    {
        return await _collection.FindByIdAsync(new BsonValue(key));
    }

    public async Task UpdateAsync(T entity)
    {
        await _collection.UpdateAsync(entity);
    }

    public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.FindOneAsync(predicate);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.FindAsync(predicate);
    }
}