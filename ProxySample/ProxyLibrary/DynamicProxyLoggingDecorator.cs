using Castle.DynamicProxy;

namespace ProxyLibrary
{
    public class DynamicProxyLoggingDecorator
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static T DecorateViaInheritance<T>(T target) where T: class
        {
            var proxy = target == null ?
                _generator.CreateClassProxy<T>(new DynamicProxyLoggingInterceptor(typeof(T).Name)) :
                _generator.CreateClassProxyWithTarget(target, new DynamicProxyLoggingInterceptor(typeof(T).Name));
                
            return proxy;
        }

        public static T DecorateViaComposition<T>(T target) where T: class
        {
            var proxy = target == null ?
                _generator.CreateInterfaceProxyWithoutTarget<T>(new DynamicProxyLoggingInterceptor(typeof(T).Name)) :
                _generator.CreateInterfaceProxyWithTarget(target, new DynamicProxyLoggingInterceptor(typeof(T).Name));

            return proxy;
        }
    }
}
