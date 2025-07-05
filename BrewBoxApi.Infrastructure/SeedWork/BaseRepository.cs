using System.Linq.Expressions;
using BrewBoxApi.Application.Common.Exceptions;
using BrewBoxApi.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace BrewBoxApi.Infrastructure.SeedWork;

public abstract class BaseRepository<TModel>(ApplicationDbContext dbContext) where TModel : BaseModel
{
    public IQueryable<TModel> FindAll(params Expression<Func<TModel, object>>[] includes)
    {
        var query = dbContext.Set<TModel>().AsNoTracking();
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public IQueryable<TModel> Where(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
    {
        var query = dbContext.Set<TModel>()
            .Where(expression)
            .AsNoTracking();
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public async ValueTask<TModel> FindAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        var query = dbContext.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var model = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(TModel), id);

        return model;
    }

    public virtual async ValueTask<TModel?> FindOneByFilterAsync(Expression<Func<TModel, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        var query = dbContext.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var model = await query.FirstOrDefaultAsync(expression, cancellationToken)
            ?? throw new NotFoundException(nameof(TModel), expression);

        return model;
    }

    public async ValueTask<IEnumerable<TModel>> FindManyByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        var query = dbContext.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var resultsList = await query.Where(e => ids.Contains(EF.Property<string>(e, "Id")))
            .ToListAsync(cancellationToken);

        return resultsList ?? [];
    }

    public async ValueTask<TModel> AddAsync(TModel entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.Set<TModel>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async ValueTask DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var model = await dbContext.Set<TModel>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(TModel), id);

        dbContext.Set<TModel>().Remove(model);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(TModel entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Set<TModel>().Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}