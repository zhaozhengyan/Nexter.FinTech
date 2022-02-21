namespace Nexter.Fintech.Core
{
	public static class Rule
	{
		public static IValidator<TOwner> For<TOwner>(TOwner owner = default(TOwner))
		{
			return new Validator<TOwner>(owner);
		}
	}
}
