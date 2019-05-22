using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using System;
using System.Reflection;

namespace ProxyLibrary
{
    class DynamicProxyLoggingInterceptor : IInterceptor
    {
        private readonly Logger _logger;

        public DynamicProxyLoggingInterceptor(string typeName = null) 
        {
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console().CreateLogger();
            _logger.Information($"New logging decorator created{(string.IsNullOrWhiteSpace(typeName) ? string.Empty : " for object of type {TypeName}")}", typeName);
        }

        public void Intercept(IInvocation invocation)
        {
            using (LogContext.PushProperty("ActivityId", Guid.NewGuid()))
            {
                try
                {
                    _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", invocation.Method.DeclaringType.Name, invocation.Method.Name, invocation.Arguments);

                    invocation.Proceed();
                    _logger.Information("Finished calling method {TypeName}.{MethodName}", invocation.Method.DeclaringType.Name, invocation.Method.Name);
                }
                catch (TargetInvocationException exc)
                {
                    _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", invocation.Method.DeclaringType.Name, invocation.Method.Name, exc.InnerException);

                    throw exc.InnerException;
                }
            }
        }
    }
}
