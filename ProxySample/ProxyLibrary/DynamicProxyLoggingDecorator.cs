using Castle.DynamicProxy;
using System;

namespace ProxyLibrary
{
    public class DynamicProxyLoggingDecorator
    {
        // ProxyGenerator is used to create DynamicProxy proxy objects
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        // CreateClassWithProxy uses inheritance-based proxying to create a new object that actually
        // derives from the provided class and applies interceptors to all APIs. In this model, the proxy
        // object and target object are the same.
        public static T DecorateViaInheritance<T>() where T: class =>
            _generator.CreateClassProxy<T>(new DynamicProxyLoggingInterceptor(typeof(T).Name));

        // CreateInterfaceProxyWithTarget uses composition-based proxying to wrap a target object with
        // a proxy object implementing the desired interface. Calls are passed to the target object
        // after running interceptors. This model is similar to DispatchProxy.
        public static T DecorateViaComposition<T>(T target = null) where T: class
        {
            var proxy = target != null ?
                _generator.CreateInterfaceProxyWithTarget(target, new DynamicProxyLoggingInterceptor(typeof(T).Name)) :

                // There is also a CreateInterfaceProxyWithoutTarget method but that API assumes there is no wrapped object at all,
                // and only the interceptors are called. That model can be useful but doesn't match the logging example used in this sample.
                _generator.CreateInterfaceProxyWithTarget<T>(Activator.CreateInstance(typeof(T)) as T, new DynamicProxyLoggingInterceptor(typeof(T).Name));

            return proxy;
        }
    }
}
