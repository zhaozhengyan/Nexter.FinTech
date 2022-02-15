using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nexter.Fintech.Core
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static string GetDescription(this Enum value)
        {
            if (value == null) return "";
            var type = value.GetType();
            return type.GetDescription(value.ToString());
        }

        public static string GetDisplayName(this Enum val)
        {
            if (val == null) return "";
            return val.GetType()
                      .GetMember(val.ToString())
                      .FirstOrDefault()
                      ?.GetCustomAttribute<DisplayAttribute>(false)
                      ?.Name
                      ?? val.ToString();
        }

        public static string GetDisplayName(this Type type, string fieldName)
        {
            return type?.GetField(fieldName)?.GetCustomAttribute<DisplayAttribute>()?.Name;
        }
        public static string GetDescription(this Type me)
        {
            if (me == null) throw new ArgumentNullException(nameof(me));
            var type = me;

            var attributes = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length == 0) return me.Name;
            var description = ((DescriptionAttribute)attributes[0]).Description;

            return description;
        }

        public static string GetDescription(this Type type, string fieldName)
        {
            var typeDisplay = typeof(DescriptionAttribute);
            var attributes = new DescriptionAttribute[0];
            try
            {
                var field = type.GetField(fieldName);
                var arr = field.GetCustomAttributes(typeDisplay, true);
                attributes = arr.OfType<DescriptionAttribute>().ToArray();
                var attribute = attributes.FirstOrDefault();
                return attribute.Description;
            }
            catch
            {
                return "";
            }
        }

        public static string GetNestedMessage(this Exception ex)
        {
            if (ex == null)
                return "";
            var sb = new StringBuilder();
            sb.Append(ex.Message);
            while (ex.InnerException != null)
            {
                sb.Append("-->");
                sb.Append(ex.InnerException.Message);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

        public static T ConvertTo<T>(this IConvertible convertibleValue, T val = default(T))
        {
            if (null == convertibleValue)
                return default(T);
            try
            {
                if (!typeof(T).IsGenericType)
                {
                    return (T)Convert.ChangeType(convertibleValue, typeof(T));
                }
                else
                {
                    var genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                    }
                }
            }
            catch
            {
                return val;
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, typeof(T).FullName));
        }

        /// <summary>
        /// 获取字符串枚举的指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetLiteralCustomAttribute<T>(this Type enumType, string value) where T : Attribute
        {
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if ((string)field.GetValue(null) == value)
                {
                    return (T)field.GetCustomAttribute(typeof(T), false);
                }
            }
            return null;
        }

        public static string GetFriendlyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(long))
                return "long";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            return type.Name;
        }

        ///// <summary>
        ///// 对数据源src的decimal类型属性截断保留小数
        ///// </summary>
        ///// <param name="src"></param>
        ///// <param name="scale"></param>
        //public static void SetScaleDecimalProperty(this object src, int scale = 2)
        //{
        //    if (src == null)
        //    {
        //        return;
        //    }
        //    var type = src.GetType();
        //    foreach (var property in type.GetProperties())
        //    {
        //        if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
        //        {
        //            if (property.GetMethod == null)
        //            {
        //                continue;
        //            }
        //            var value = ((decimal?)property.GetValue(src));
        //            if (value == null)
        //            {
        //                continue;
        //            }
        //            if (property.SetMethod != null)
        //            {
        //                property.SetValue(src, value.Value.SetScale(scale));
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// 调用输入类型的无参构造，初始化属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConstuctObject(this Type type)
        {
            var obj = Activator.CreateInstance(type);
            foreach (var fieldInfo in type.GetProperties())
            {
                if (fieldInfo.PropertyType == typeof(int)
                    || fieldInfo.PropertyType == typeof(int?))
                {
                    fieldInfo.SetValue(obj, 0);
                }
                else if (fieldInfo.PropertyType == typeof(decimal)
                    || fieldInfo.PropertyType == typeof(decimal?))
                {
                    fieldInfo.SetValue(obj, (decimal)0);
                }
                else if (fieldInfo.PropertyType == typeof(string))
                {
                    fieldInfo.SetValue(obj, "");
                }
                else if (fieldInfo.PropertyType == typeof(float)
                    || fieldInfo.PropertyType == typeof(float?)
                    || fieldInfo.PropertyType == typeof(double?)
                    || fieldInfo.PropertyType == typeof(double))
                {
                    fieldInfo.SetValue(obj, 0.0);
                }
                else if (fieldInfo.PropertyType == typeof(DateTime)
                    || fieldInfo.PropertyType == typeof(DateTime?))
                {
                    fieldInfo.SetValue(obj, DateTime.Now);
                }
            }
            return obj;
        }
    }
}
