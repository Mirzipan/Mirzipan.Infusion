using System;

namespace Mirzipan.Infusion.Resolvers
{
    public class FactoryResolver: IResolver
    {
        private readonly Func<object> _factory;

        public FactoryResolver(Func<object> factoryFunction)
        {
            _factory = factoryFunction;
        }

        public object Resolve(InjectionContainer container)
        {
            return _factory.Invoke();
        }

        public void Dispose()
        {
        }
    }
}