using System.Linq.Expressions;
using BrewBoxApi.Application.Common.Exceptions;
using BrewBoxApi.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace BrewBoxApi.Infrastructure.Data;

public abstract class BaseRepository<TModel>(ApplicationDbContext context) : IBaseReadRepository<TModel>, IBaseCommandRepository<TModel> where TModel : BaseModel
{
    protected readonly ApplicationDbContext _context = context;

    public IQueryable<TModel> FindAll(params Expression<Func<TModel, object>>[] includes)
    {
        var query = _context.Set<TModel>().AsNoTracking();
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public async ValueTask<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public IQueryable<TModel> Where(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
    {
        var query = _context.Set<TModel>()
            .Where(expression)
            .AsNoTracking();
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public async ValueTask<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>()
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<TModel?> FindAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var query = _context.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async ValueTask<TModel?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await _context.Set<TModel>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async ValueTask<TModel?> GetByIdIncludingDeletedAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await _context.Set<TModel>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async ValueTask<TModel?> FindOneByFilterAsync(Expression<Func<TModel, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        var query = _context.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async ValueTask<IEnumerable<TModel>> FindManyByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default, params Expression<Func<TModel, object>>[] includes)
    {
        var query = _context.Set<TModel>().AsQueryable();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query
            .Where(e => ids.Contains(EF.Property<string>(e, "Id")))
            .AsNoTracking()
            .ToListAsync(cancellationToken) ?? [];
    }

    public async ValueTask<TModel> AddAsync(TModel entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.Set<TModel>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async ValueTask<TModel> UpdateAsync(TModel entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<TModel>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async ValueTask DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var entity = await GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TModel), id);
        _context.Set<TModel>().Remove(entity); // Triggers soft delete interceptor
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<TModel> RestoreAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var entity = await GetByIdIncludingDeletedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TModel), id);
        entity.IsDeleted = false;
        entity.DeletedOn = null;
        entity.DeletedById = null;
        _context.Set<TModel>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}