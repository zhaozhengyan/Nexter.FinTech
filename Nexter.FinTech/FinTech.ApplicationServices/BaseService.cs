using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexter.Domain;

namespace FinTech.ApplicationServices
{
    public abstract class BaseService
    {
        protected ILogger Logger { get; }
        protected IUnitOfWork UnitOfWork { get; }
        protected IRepository Repository { get; }

        protected BaseService(ILogger logger, IUnitOfWork unitOfWork, IRepository repository)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            Repository = repository;
        }

        protected Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return UnitOfWork.CommitAsync(cancellationToken);
        }
    }
}