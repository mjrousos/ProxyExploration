using Castle.DynamicProxy;
using Serilog;
using Serilog.Core;
using System.Reflection;

namespace ProxyLibrary
{
    // Simple sample logging interceptor for DynamicProxy
    class DynamicProxyLoggingInterceptor : IInterceptor
    {
        // The Serilog logger to be used for logging.
        private readonly Logger _logger;

        // Constructor that initializes the Logger the interceptor will use
        public DynamicProxyLoggingInterceptor(string typeName = null) 
        {
            // Setup the Serilog logger
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console().CreateLogger();
            _logger.Information($"New logging decorator created{(string.IsNullOrWhiteSpace(typeName) ? string.Empty : " for object of type {TypeName}")}", typeName);
        }

        // The Intercept method is where the interceptor decides how to handle calls to the proxy object
        public void Intercept(IInvocation invocation)
        {
            try
            {
                // Perform the logging that this proxy is meant to provide
                _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", invocation.Method.DeclaringType.Name, invocation.Method.Name, invocation.Arguments);

                // Invocation.Proceeds goes on to the next interceptor or, if there are no more interceptors, invokes the method.
                // The details of how the method are invoked will depend on the proxying model used. The interceptor does not need
                // to know those details.
                invocation.Proceed();

                // A little more logging.
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
