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
    // Simple sample RealProxy-based logging proxy
    // Note that the proxied type must derive from MarshalByRefObject
    public class RealProxyLoggingDecorator<T> : RealProxy where T: MarshalByRefObject
    {
        // A field to store an inner 'real' instance of the proxied type.
        // All API calls will go to this wrapped instance (after the proxy has 
        // done its logging).
        private readonly T _target;

        // The Serilog logger to be used for logging.
        private readonly Logger _logger;

        // When creating a new instance of this proxy type, we invoke RealProxy's
        // constructor with the type to be wrapped and, optionally, allow
        // the caller to provide a 'target' object to be wrapped.
        private RealProxyLoggingDecorator(T target = null) : base(typeof(T))
        {
            // If no target object is supplied, created a new one.
            if (target == null)
            {
                target = Activator.CreateInstance<T>();
            }

            _target = target;

            // Setup the Serilog logger
            _logger = new LoggerConfiguration()
                .WriteTo.Console().CreateLogger();
            _logger.Information("New logging decorator created for object of type {TypeName}", typeof(T).FullName);
        }

        // This convenience method creates an instance of RealProxyLoggingDecorator
        // and calls RealProxy's GetTransparentProxy method to retrieve the proxy
        // object (which looks like an instance of U but calls our Invoke method
        // whenever an API is used).
        public static U Decorate<U>(U target = null) where U: MarshalByRefObject
        {
            return target == null ?
                new RealProxyLoggingDecorator<U>().GetTransparentProxy() as U :
                new RealProxyLoggingDecorator<U>(target).GetTransparentProxy() as U;
        }

        // The invoke method is the heart of a RealProxy implementation. Here, we
        // define what should happen when a member on the proxy object is used.
        public override IMessage Invoke(IMessage msg)
        {
            // The IMessage argument should be an IMethodCallMessage indicating
            // which method is being invoked. Interestingly, RealProxy even translates 
            // field access into method calls so those will show up here, too.
            if (msg is IMethodCallMessage methodCallMsg)
            {
                try
                {
                    // Perform the logging that this proxy is meant to provide
                    _logger.Information("Calling method {TypeName}.{MethodName} with arguments {Arguments}", methodCallMsg.MethodBase.DeclaringType.Name, methodCallMsg.MethodName, methodCallMsg.Args);

                    // Cache the method's arguments locally so that out and ref args can be updated at invoke time.
                    // (methodCallMsg.Args can't be updated directly since they are returned by a property getter and don't refer to a consistent object[])
                    var args = methodCallMsg.Args;

                    // For this proxy implementation, we still want to call the original API 
                    // (after logging has happened), so use reflection to invoke the desired
                    // API on our wrapped target object.
                    var result = methodCallMsg.MethodBase.Invoke(_target, args);

                    // A little more logging.
                    _logger.Information("Method {TypeName}.{MethodName} returned {ReturnValue}", methodCallMsg.MethodBase.DeclaringType.Name, methodCallMsg.MethodName, result);

                    // Finally, Invoke should return a ReturnMessage object indicating the result of the operation
                    return new ReturnMessage(result, args, args.Length, methodCallMsg.LogicalCallContext, methodCallMsg);
                }
                catch (TargetInvocationException exc)
                {
                    // If the MethodBase.Invoke call fails, warn in the logs and then return
                    // a ReturnMessage containing the exception.
                    _logger.Warning(exc.InnerException, "Method {TypeName}.{MethodName} threw exception: {Exception}", methodCallMsg.TypeName, methodCallMsg.MethodName, exc.InnerException);

                    return new ReturnMessage(exc.InnerException, methodCallMsg);
                }
            }
            else
            {
                throw new ArgumentException("Invalid message; expected IMethodCallMessage", nameof(msg));
            }
        }
    }
}
#endif // NET461