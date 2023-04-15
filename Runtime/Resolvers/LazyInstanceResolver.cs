using System;

namespace Mirzipan.Infusion.Resolvers
{
    public class LazyInstanceResolver : IResolver
    {
        private object _instance;
        private readonly Type _targetType;
        
        public LazyInstanceResolver(Type targetType)
        {
            _targetType = targetType;
        }
        
        public object Resolve(InjectionContainer container)
        {
            return _instance ??= container.Instantiate(_targetType);
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}