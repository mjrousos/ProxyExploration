using Serilog;
using Serilog.Context;
using Serilog.Core;
using System;
using System.Reflection;

namespace ProxyLibrary
{
    // Simple sample DispatchProxy-based logging proxy
    public class DispatchProxyLoggingDecorator<T> : DispatchProxy 
        where T: class // T must be an interface
    {
        // The Serilog logger to be used for logging.
        private readonly Logger _logger;

        // Expose the target object as a read-only property so that users can access
        // fields or other implementation-specific details not available through the interface
        public T Target { get; private set; }

        // A DispatchProxy's parameterless ctor is called when a 
        // new proxy instance is Created
        public DispatchProxyLoggingDecorator() : base()
        {
            // Setup the Serilog logger
            _logger = new LoggerConfiguration()
                .WriteTo.Console().CreateLogger();
            _logger.Information("New logging decorator created for object of type {TypeName}", typeof(T).FullName);
        }

        // This convenience method creates an instance of DispatchProxyLoggingDecorator,
        // sets the target object, and calls DispatchProxy's Create method to retrieve the 
        // proxy implementation of the target interface (which looks like an instance of T 
        // but calls our Invoke method whenever an API is used).
        public static T Decorate(T target = null)
        {
            // DispatchProxy.Create creates proxy objects
            var proxy = Create<T, DispatchProxyLoggingDecorator<T>>()
                as DispatchProxyLoggingDecorator<T>;

            // If the proxy wraps an underlying object, it must be supplied after creating
            // the proxy.
            proxy.Target = target ?? Activator.CreateInstance<T>();

            return proxy as T;
        }

        // The invoke method is the heart of a DispatchProxy implementation. Here, we
        // define what should happen when a method on the proxied object is used. The
        // signature is a little simpler than RealProxy's since a MethodInfo and args
        // are passed in directly.
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                // Perform the logging that this proxy is meant to provide
                _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", targetMethod.DeclaringType.Name, targetMethod.Name, args);

                // For this proxy implementation, we still want to call the original API 
                // (after logging has happened), so use reflection to invoke the desired
                // API on our wrapped target object.
                var result = targetMethod.Invoke(Target, args);

                // A little more logging.
                _logger.Information("Method {TypeName}.{MethodName} returned {ReturnValue}", targetMethod.DeclaringType.Name, targetMethod.Name, result);

                return result;
            }
            catch (TargetInvocationException exc)
            {
                // If the MethodInvoke.Invoke call fails, log a warning and then rethrow the exception
                _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", targetMethod.DeclaringType.Name, targetMethod.Name, exc.InnerException);

                throw exc.InnerException;
            }
        }

        // Provide a workaround for the decorated object's fields not being exposed
        public void SetFieldValue(string fieldName, object fieldValue)
        {
            _logger.Information("Setting field value {TypeName}.{FieldName} to {FieldValue}", Target.GetType().FullName, fieldName, fieldValue);
            var fieldInfo = Target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                _logger.Warning("Field not found: {TypeName}.{FieldName}", Target.GetType().FullName, fieldName);
            }
            else
            {
                fieldInfo?.SetValue(Target, fieldValue);
                _logger.Information("Field value {TypeName}.{FieldName} set", Target.GetType().FullName, fieldName, fieldValue);
            }
        }

        public object GetFieldValue(string fieldName)
        {
            _logger.Information("Getting field value for {TypeName}.{FieldName}", Target.GetType().FullName, fieldName);
            var fieldInfo = Target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                _logger.Warning("Field not found: {TypeName}.{FieldName}", Target.GetType().FullName, fieldName);
                return null;
            }
            else
            {
                var ret = fieldInfo.GetValue(Target);
                _logger.Information("Retrieved value {fieldValue} from {TypeName}.{FieldName}", ret, Target.GetType().FullName, fieldName);
                return ret;
            }
        }
    }
}