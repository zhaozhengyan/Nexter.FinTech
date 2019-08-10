//using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

namespace Nexter.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));

        void DetachAll();

        IRepository Repository { get; }

        //Task<int> ExecuteSqlCommandAsync(RawSqlString sql, params object[] parameters);

    }
}
 