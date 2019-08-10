using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace FinTech.Infrastructure.Validation
{
	public static class Extensions
	{
		static Extensions()
		{
			Getters = new Dictionary<MemberInfo, Func<object, object>>();
		}
		private static IDictionary<MemberInfo, Func<object, object>> Getters { get; }
		private static Func<object, object> AcquireGetter(MemberInfo member)
		{
			var key = member;

			if (Getters.TryGetValue(key, out var value) == false)
			{
				lock (Getters)
				{
					if (Getters.TryGetValue(key, out value) == false)
					{
						var untypedOwner = Expression.Parameter(typeof(object));
						var owner = Expression.Convert(untypedOwner, member.DeclaringType);
						var returnValue = Expression.MakeMemberAccess(owner, member);
						var untypedReturnValue = Expression.Convert(returnValue, typeof(object));
						var sourceCode = Expression.Lambda<Func<object, object>>(untypedReturnValue, untypedOwner);
						value = sourceCode.Compile();
						Getters.Add(member, value);
					}
				}
			}

			return value;
		}
		private static MemberInfo GetRequiredMember<TOwner, TValue>(this Expression<Func<TOwner, TValue>> me)
		{
			var body = me.Body as MemberExpression;
			if (body == null) throw new ArgumentException("Can not find MemberExpression from expression.Body.");

			return body.Member;
		}
		private static TValue GetValue<TOwner, TValue>(this TOwner me, Expression<Func<TOwner, TValue>> expression)
		{
			var member = expression.GetRequiredMember();
			var value = me.GetValue<TOwner, TValue>(member);

			return value;
		}
		private static TValue GetValue<TOwner, TValue>(this TOwner owner, MemberInfo member)
		{
			var getter = AcquireGetter(member);

			return (TValue)getter.Invoke(owner);
		}
		private static string Localize(this MemberInfo me)
		{
			var attribute = me.GetCustomAttribute<DescriptionAttribute>();

			return attribute?.Description ?? me.Name;
		}
		private static string Localize(this Type me)
		{
			var attribute = me.GetCustomAttribute<DescriptionAttribute>();

			return attribute?.Description ?? me.Name;
		}
		private static string Localize(this Enum me)
		{
			var attribute = me.GetType().GetMember(me.ToString())[0].GetCustomAttribute<DescriptionAttribute>();

			return attribute?.Description ?? me.ToString();
		}
		private static string CodeFor<TOwner,TValue>(Expression<Func<TOwner, TValue>> expression)
		{
			return expression.GetRequiredMember().Name;
		}
		private static string CodeFor<TOwner>()
		{
			return typeof(TOwner).Name;
		}
		public static IValidator<TOwner> FailIf<TOwner>(this IValidator<TOwner> me, Func<TOwner, bool> predicate, string message, string statusCode = null)
		{
			if (predicate.Invoke(me.Owner)) throw new BusinessViolation(message, statusCode);

			return me;
		}
		public static IValidator<TOwner> Null<TOwner>(this IValidator<TOwner> me, string message = null)
		{
			return me.FailIf(el => el == null, message ?? $"{typeof(TOwner).Localize()} can not be null.");
		}
		public static IValidator<TOwner> Null<TOwner, TValue>(this IValidator<TOwner> me, Expression<Func<TOwner, TValue>> expression, string message = null)
		{
			return me.FailIf(el => el.GetValue(expression) == null, message ?? $"{expression.GetRequiredMember().Localize()} can not be null.");
		}
		public static IValidator<TOwner> Null<TOwner>(this IValidator<TOwner> me, Expression<Func<TOwner, string>> expression, string message = null)
		{
			return me.FailIf(el => string.IsNullOrEmpty(el.GetValue(expression)), message ?? $"{expression.GetRequiredMember().Localize()} can not be null.", $"Null");
		}
		public static IValidator<TOwner> Mobile<TOwner>(this IValidator<TOwner> me, Expression<Func<TOwner, string>> expression, string message = null)
		{
			var value = me.Owner.GetValue(expression);

			return me.FailIf(el => value?.Length != 11, message ?? $"{expression.GetRequiredMember().Localize()} is not a mobile format.", $"InvalidMobileNumber");
		}
		public static IValidator<TOwner> NotFound<TOwner, TValue>(this IValidator<TOwner> me, Expression<Func<TOwner, TValue>> expression, string message = null)
		{
			return me.FailIf(el => el.GetValue(expression) == null, message ?? $"{expression.GetRequiredMember().Localize()} can not be found.", $"NotFound");
		}
		public static IValidator<TOwner> NotFound<TOwner>(this IValidator<TOwner> me, string message = null)
		{
			return me.FailIf(el => el == null, message ?? $"{typeof(TOwner).GetDescription()} can not be found.", $"NotFound");
		}
		public static IValidator<TOwner> InUse<TOwner>(this IValidator<TOwner> me, TOwner target, string message = null)
		{
			return me.FailIf(el => target != null, message ?? $"{typeof(TOwner).Localize()} is already in use.", $"InUse");
		}
	}
}
