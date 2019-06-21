# Proxy Exploration

This repository is meant to be a small playground for implementing 
[proxy design patterns][proxy-pattern] with different APIs. First, I use 
[`System.Runtime.Remoting.Proxies.RealProxy`][real-proxy] to implement the 
proxy pattern. Then, I port the sample to .NET Core and use 
[`System.Reflection.DispatchProxy`][dispatch-proxy] and [`Castle.DynamicProxy`][dynamic-proxy] to implement the same 
pattern.

This repo is in response to frequent use of `RealProxy` in customer 
applications that complicates migrating to .NET Core.

[proxy-pattern]:https://en.wikipedia.org/wiki/Proxy_pattern
[real-proxy]:https://docs.microsoft.com/dotnet/api/system.runtime.remoting.proxies.realproxy
[dispatch-proxy]:https://docs.microsoft.com/dotnet/api/system.reflection.dispatchproxy
[dynamic-proxy]: http://www.castleproject.org/projects/dynamicproxy/