namespace Nexter.Fintech.Core
{
	public interface IValidator<TOwner>
	{
		TOwner Owner { get; }
		IValidator<TDestination> SetOwner<TDestination>(TDestination owner);
	}
}
