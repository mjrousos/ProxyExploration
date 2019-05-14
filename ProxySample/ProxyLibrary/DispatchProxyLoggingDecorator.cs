using Serilog;
using Serilog.Context;
using Serilog.Core;
using System;
using System.Reflection;

namespace ProxyLibrary
{
    public class DispatchProxyLoggingDecorator<T> : DispatchProxy where T: class
    {
        private readonly Logger _logger;

        // Expose the target object as a read-only property so that users can access
        // fields or other implementation-specific details not available through the interface
        public T Target { get; private set; }

        // A DispatchProxy's parameterless ctor is called when a 
        // new proxy instance is Created
        public DispatchProxyLoggingDecorator() : base()
        {
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console().CreateLogger();
            _logger.Information("New logging decorator created for object of type {TypeName}", typeof(T).FullName);
        }

        public static T Decorate(T target)
        {
            var proxy = Create<T, DispatchProxyLoggingDecorator<T>>()
                as DispatchProxyLoggingDecorator<T>;

            proxy.Target = target ?? throw new ArgumentNullException(nameof(target));

            return proxy as T;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            using (LogContext.PushProperty("ActivityId", Guid.NewGuid()))
            {
                try
                {
                    _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", targetMethod.DeclaringType.Name, targetMethod.Name, args);

                    var result = targetMethod.Invoke(Target, args);
                    _logger.Information("Method {TypeName}.{MethodName} returned {ReturnValue}", targetMethod.DeclaringType.Name, targetMethod.Name, result);
                    return result;
                }
                catch (TargetInvocationException exc)
                {
                    _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", targetMethod.DeclaringType.Name, targetMethod.Name, exc.InnerException);

                    throw exc.InnerException;
                }
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