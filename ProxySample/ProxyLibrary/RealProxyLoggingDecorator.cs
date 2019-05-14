#if NET461
using Serilog;
using Serilog.Context;
using Serilog.Core;
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ProxyLibrary
{
    public class RealProxyLoggingDecorator<T> : RealProxy where T: MarshalByRefObject
    {
        private readonly T _target;
        private readonly Logger _logger;

        // Wrap existing object
        private RealProxyLoggingDecorator(T target = null) : base(typeof(T))
        {
            if (target == null)
            {
                target = Activator.CreateInstance<T>();
            }

            _target = target;
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console().CreateLogger();
            _logger.Information("New logging decorator created for object of type {TypeName}", typeof(T).FullName);
        }

        public static U Decorate<U>(U target = null) where U: MarshalByRefObject
        {
            return target == null ?
                new RealProxyLoggingDecorator<U>().GetTransparentProxy() as U :
                new RealProxyLoggingDecorator<U>(target).GetTransparentProxy() as U;
        }

        public override IMessage Invoke(IMessage msg)
        {
            using (LogContext.PushProperty("ActivityId", Guid.NewGuid()))
            {
                if (msg is IMethodCallMessage methodCallMsg)
                {
                    try
                    {
                        _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", methodCallMsg.MethodBase.DeclaringType.Name, methodCallMsg.MethodName, methodCallMsg.Args);

                        // Cache the method's arguments locally so that out and ref args can be updated at invoke time.
                        // (methodCallMsg.Args can't be updated since they are returned by a property getter and don't refer to a consistent object[])
                        var args = methodCallMsg.Args;
                        var result = methodCallMsg.MethodBase.Invoke(_target, args);
                        _logger.Information("Method {TypeName}.{MethodName} returned {ReturnValue}", methodCallMsg.MethodBase.DeclaringType.Name, methodCallMsg.MethodName, result);
                        return new ReturnMessage(result, args, args.Length, methodCallMsg.LogicalCallContext, methodCallMsg);
                    }
                    catch (TargetInvocationException exc)
                    {
                        _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", methodCallMsg.TypeName, methodCallMsg.MethodName, exc.InnerException);

                        return new ReturnMessage(exc.InnerException, methodCallMsg);
                    }
                }
            }

            throw new ArgumentException("Invalid message; expected IMethodCallMessage", nameof(msg));
        }
    }
}
#endif // NET461