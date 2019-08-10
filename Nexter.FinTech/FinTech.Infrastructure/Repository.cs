using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace Nexter.Infrastructure
{
    public class Repository : IRepository, IUnitOfWork
    {

        public Repository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected DbContext Context { get; }
        IRepository IUnitOfWork.Repository => throw new NotImplementedException();

        Task<bool> IRepository.ExistAsync<T>(Expression<Func<T, bool>> expression)
        {
            return Context.Set<T>()
                          .AnyAsync(expression);
        }

        async Task<T> IRepository.GetByIdAsync<T>(object id, bool throwExceptionIfNotExists)
        {
            var entity = await Context.Set<T>()
                                      .FindAsync(id)
                                      .ConfigureAwait(false);
            if (entity == null && throwExceptionIfNotExists)
            {
                var name = typeof(T).GetCustomAttribute<DisplayAttribute>()?.Name ?? typeof(T).Name;
                throw new Exception($"{name}({id})不存在");
            }

            return entity;
        }

        void IRepository.Add<T>(T item)
        {
            if (item is Entity model)
            {
                var now = DateTime.Now;
                model.CreatedAt = model.CreatedAt ?? now;
                model.LastModifiedAt = model.LastModifiedAt ?? now;
            }
            Context.Add(item);
        }
        async Task IRepository.AddAsync<T>(T item)
        {
            if (item is Entity model)
            {
                var now = DateTime.Now;
                model.CreatedAt = model.CreatedAt ?? now;
                model.LastModifiedAt = model.LastModifiedAt ?? now;
            }
            await Context.AddAsync(item);
        }

        IQueryable<T> IRepository.AsQueryable<T>()
        {
            return Context.Set<T>();
        }

        void IRepository.Remove<T>(T item)
        {
            Context.Remove(item);
        }

        Task IRepository.RemoveAsync<T>(T item)
        {
            Context.Remove(item);

            return Task.CompletedTask;
        }

        async Task IUnitOfWork.CommitAsync(CancellationToken cancellationToken)
        {
            #region Save events

            //Context.ChangeTracker
            //       .Entries<Entity>()
            //       .ForEach(e =>
            //       {
            //           _eventBus.Publish(e.Entity.DomainEvents.ToArray());
            //           e.Entity.ClearDomainEvents();
            //       });
            //var messageContexts = _eventBus.GetEvents()
            //                               .Select(e => new MessageContext(e))
            //                               .ToArray();
            //_eventBus.ClearMessages();

            //var messages = messageContexts.Select(messageContext => new Message(messageContext))
            //                              .ToArray();
            //var unSentMessages = messageContexts.Select(messageContext => new UnSentMessage(messageContext))
            //                                    .ToArray();

            //if (!(_messageStore is InMemoryMessageStore))
            //{
            //    await Context.AddRangeAsync(messages, cancellationToken)
            //                 .ConfigureAwait(false);
            //    await Context.AddRangeAsync(unSentMessages, cancellationToken)
            //                 .ConfigureAwait(false);
            //}

            #endregion

            await Context.SaveChangesAsync(cancellationToken)
                         .ConfigureAwait(false);

            //// 本地测试时使用InMemoryMessageStore
            //if (_messageStore is InMemoryMessageStore)
            //{
            //    await _messageStore.SaveUnSentMessagesAsync(unSentMessages);
            //}
        }

        void IUnitOfWork.DetachAll()
        {
            var changeEntriesCopy = Context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted)
                .ToList();
            foreach (var entry in changeEntriesCopy)
            {
                entry.State = EntityState.Detached;
            }
        }

        void IDisposable.Dispose()
        {
            Context.Dispose();
        }
    }
}