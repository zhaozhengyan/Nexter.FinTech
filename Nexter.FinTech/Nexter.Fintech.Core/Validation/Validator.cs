namespace Nexter.Fintech.Core
{
	public class Validator<T> : IValidator<T>
	{
		protected virtual T Owner { get; }

		T IValidator<T>.Owner => Owner;

		public Validator(T owner)
		{
			Owner = owner;
		}

		IValidator<TDestination> IValidator<T>.SetOwner<TDestination>(TDestination owner)
		{
			return new Validator<TDestination>(owner);
		}
	}
}
