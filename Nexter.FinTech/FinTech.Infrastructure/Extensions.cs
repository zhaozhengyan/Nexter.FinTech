using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FinTech.Infrastructure
{
    public static partial class Extensions
    {
        public static T Clone<T>(this T obj) where T : ValueObject
        {
            return obj.GetCopy() as T;
        }
        /// <summary>
        /// 安全获取值，当值为null时，不会抛出异常
        /// </summary>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>(this T? value) where T : struct
        {
            return value ?? default(T);
        }

        public static IPAddress GetLocalIpv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        public static T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
        {
            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                var rawValues = values.ToString(); // writes out as Csv when there are multiple.

                if (!string.IsNullOrWhiteSpace(rawValues))
                {
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
                }
            }
            return default(T);
        }

        public static async Task<string> GetRequestBodyStringAsync(this HttpRequest request)
        {
            string bodyString = null;
            if (request.Body.CanSeek)
            {
                var position = request.Body.Position;
                request.Body.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(request.Body);
                request.HttpContext.Response.RegisterForDispose(streamReader);
                bodyString = await streamReader.ReadToEndAsync();
                request.Body.Seek(position, SeekOrigin.Begin);
            }
            return bodyString;
        }

        public static string GetRequestBodyString(this HttpRequest request)
        {
            long position = request.Body.Position;
            string str = null;
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0L, SeekOrigin.Begin);
                StreamReader streamReader = new StreamReader(request.Body);
                request.HttpContext.Response.RegisterForDispose((IDisposable)streamReader);
                str = streamReader.ReadToEnd();
                request.Body.Seek(position, SeekOrigin.Begin);
            }
            return str;
        }

        public static async Task<string> GetResponseBodyStringAsync(this HttpResponse response)
        {
            string bodyString = null;
            if (response.Body.CanSeek)
            {
                var position = response.Body.Position;
                response.Body.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(response.Body);
                response.RegisterForDispose(streamReader);
                bodyString = await streamReader.ReadToEndAsync();
                response.Body.Seek(position, SeekOrigin.Begin);
            }
            return bodyString;
        }

        //public static HttpResponse EnableRewind(this HttpResponse response)
        //{
        //    if (response == null)
        //    {
        //        throw new ArgumentNullException(nameof(response));
        //    }
        //    var body = response.Body;
        //    if (!body.CanSeek)
        //    {
        //        var responseBodyStream = new WriteSyncMemoryStream(body);
        //        response.Body = responseBodyStream;
        //        response.HttpContext.Response.RegisterForDispose(responseBodyStream);
        //    }
        //    return response;
        //}


        /// <summary>
        /// 格式化手机号（脱敏）
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string FormatMobile(this string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }
            if (mobile.Length >= 3)
            {
                return mobile.Substring(0, 3) + "******" + mobile.Substring(mobile.Length - 2, 2);
            }
            else
            {
                return mobile + "*******";
            }
        }

        public static string FormatEmail(this string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;
            var atIndex = email.IndexOf("@");
            string account;
            var host = "";
            if (atIndex == -1)
            {
                //not found
                account = email;
            }
            else
            {
                account = email.Substring(0, atIndex);
                host = email.Substring(atIndex, email.Length - atIndex);
            }

            return $"{Substring(account, 3)}****{host}";
        }

        /// <summary>
        /// 姓名脱敏 只保留姓
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static string FormatPracticeLicenseNr(this string no)
        {
            if (string.IsNullOrWhiteSpace(no))
                return no;

            if (no.Length <= 5)
                return no;

            var host = no.Substring(no.Length - 2, 2);
            return $"{Substring(no, 3)}*********{host}";
        }

        /// <summary>
        /// 返回指定长度的子串
        /// 如果src为空，返回空字符串，如果src长度不足，返回src的拷贝
        /// </summary>
        /// <param name="src"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Substring(string src, int length)
        {
            if (src == null)
            {
                return string.Empty;
            }
            var strLen = src.Length;
            var endPos = Math.Min(length, strLen);
            return src.Substring(0, endPos);
        }

        /// <summary>
        /// 姓名脱敏 只保留姓
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            if (name.Length == 1)
                return name;

            return name[0] + new string('*', name.Length - 1);
        }


        /// <summary>
        /// 批量注入抽象类实现
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedServices<TService>(this IServiceCollection services)
        {
            var assemblies = GetAssemblies(a => a.GetName().Name.StartsWith("YLQ"));
            var serviceInterfaceTypes = assemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => t != typeof(TService) && typeof(TService).IsAssignableFrom(t) &&
                             t.IsInterface);
            var serviceTypes = assemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => t != typeof(TService) && typeof(TService).IsAssignableFrom(t) &&
                             !t.IsAbstract && t.IsClass).ToArray();
            foreach (var interfaceType in serviceInterfaceTypes)
            {
                var serviceType = serviceTypes.FirstOrDefault(t => interfaceType.IsAssignableFrom(t));
                if (serviceType != null)
                    services.AddScoped(interfaceType, serviceType);
            }
            return services;
        }

        /// <summary>
        /// 批量注入当前对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedConcreteServices<TService>(this IServiceCollection services)
        {
            var assemblies = GetAssemblies(a => a.GetName().Name.StartsWith("YLQ"));
            var serviceTypes = assemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => t != typeof(TService) && typeof(TService).IsAssignableFrom(t) &&
                            !t.IsAbstract && t.IsClass);
            foreach (var interfaceType in serviceTypes)
            {
                services.AddScoped(interfaceType);
            }
            return services;
        }

        public static Assembly[] GetAssemblies(Func<Assembly, bool> assemblyNameExpression)
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
                .Select(f => new FileInfo(f))
                .Where(f => f.Name.EndsWith(".dll")
                            && !f.Name.EndsWith(".PrecompiledViews.dll")
                            && !f.Name.EndsWith(".Views.dll"))
                .Select(f => Assembly.Load(f.Name.Remove(f.Name.Length - 4)))
                .Where(assemblyNameExpression)
                .ToArray();

            var loadedAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.GetName().Name.EndsWith(".PrecompiledViews")
                                   && !assembly.GetName().Name.EndsWith(".Views"))
                .Where(assemblyNameExpression);

            return loadedAssemblies.Union(assemblies)
                .ToArray();
        }

    }
}

