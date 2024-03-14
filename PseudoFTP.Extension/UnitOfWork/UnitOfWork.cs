﻿// Copyright (c) Arch team. All rights reserved.

using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Arch.EntityFrameworkCore.UnitOfWork;

/// <summary>
///     Represents the default implementation of the <see cref="IUnitOfWork" /> and <see cref="IUnitOfWork{TContext}" />
///     interface.
/// </summary>
/// <typeparam name="TContext">The type of the db context.</typeparam>
public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>, IUnitOfWork where TContext : DbContext
{
    private bool disposed;
    private Dictionary<Type, object> repositories;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWork{TContext}" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public UnitOfWork(TContext context)
    {
        DbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    ///     Gets the specified repository for the <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="hasCustomRepository"><c>True</c> if providing custom repositry</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}" /> interface.</returns>
    public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
    {
        if (repositories == null)
        {
            repositories = new Dictionary<Type, object>();
        }

        // what's the best way to support custom reposity?
        if (hasCustomRepository)
        {
            var customRepo = DbContext.GetService<IRepository<TEntity>>();
            if (customRepo != null)
            {
                return customRepo;
            }
        }

        Type type = typeof(TEntity);
        if (!repositories.ContainsKey(type))
        {
            repositories[type] = new Repository<TEntity>(DbContext);
        }

        return (IRepository<TEntity>)repositories[type];
    }

    /// <summary>
    ///     Gets the db context.
    /// </summary>
    /// <returns>The instance of type <typeparamref name="TContext" />.</returns>
    public TContext DbContext { get; }

    /// <summary>
    ///     Changes the database name. This require the databases in the same machine. NOTE: This only work for MySQL right
    ///     now.
    /// </summary>
    /// <param name="database">The database name.</param>
    /// <remarks>
    ///     This only been used for supporting multiple databases in the same model. This require the databases in the same
    ///     machine.
    /// </remarks>
    public void ChangeDatabase(string database)
    {
        DbConnection connection = DbContext.Database.GetDbConnection();
        if (connection.State.HasFlag(ConnectionState.Open))
        {
            connection.ChangeDatabase(database);
        }
        else
        {
            string connectionString = Regex.Replace(connection.ConnectionString.Replace(" ", ""),
                @"(?<=[Dd]atabase=)\w+(?=;)", database, RegexOptions.Singleline);
            connection.ConnectionString = connectionString;
        }

        // Following code only working for mysql.
        IEnumerable<IEntityType> items = DbContext.Model.GetEntityTypes();
        foreach (IEntityType item in items)
        {
            if (item is IConventionEntityType entityType)
            {
                entityType.SetSchema(database);
            }
        }
    }

    /// <summary>
    ///     Executes the specified raw SQL command.
    /// </summary>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The number of state entities written to database.</returns>
    public int ExecuteSqlCommand(string sql, params object[] parameters)
    {
        return DbContext.Database.ExecuteSqlRaw(sql, parameters);
    }

    /// <summary>
    ///     Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
    public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class
    {
        return DbContext.Set<TEntity>().FromSqlRaw(sql, parameters);
    }

    /// <summary>
    ///     Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public int SaveChanges(bool ensureAutoHistory = false)
    {
        if (ensureAutoHistory)
        {
            DbContext.EnsureAutoHistory();
        }

        return DbContext.SaveChanges();
    }

    /// <summary>
    ///     Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous save operation. The task result contains the
    ///     number of state entities written to database.
    /// </returns>
    public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false)
    {
        if (ensureAutoHistory)
        {
            DbContext.EnsureAutoHistory();
        }

        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Saves all changes made in this context to the database with distributed transaction.
    /// </summary>
    /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
    /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork" /> array.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous save operation. The task result contains the
    ///     number of state entities written to database.
    /// </returns>
    public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks)
    {
        using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            int count = 0;
            foreach (IUnitOfWork unitOfWork in unitOfWorks)
            {
                count += await unitOfWork.SaveChangesAsync(ensureAutoHistory).ConfigureAwait(false);
            }

            count += await SaveChangesAsync(ensureAutoHistory);

            ts.Complete();

            return count;
        }
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback)
    {
        DbContext.ChangeTracker.TrackGraph(rootEntity, callback);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // clear repositories
                if (repositories != null)
                {
                    repositories.Clear();
                }

                // dispose the db context.
                DbContext.Dispose();
            }
        }

        disposed = true;
    }
}