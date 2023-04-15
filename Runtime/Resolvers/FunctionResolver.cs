using System;

namespace Mirzipan.Infusion.Resolvers
{
    public class FunctionResolver: IResolver
    {
        private readonly Func<object> _factory;

        public FunctionResolver(Func<object> factoryFunction)
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