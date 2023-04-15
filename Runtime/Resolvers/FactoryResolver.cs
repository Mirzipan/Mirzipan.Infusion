using System;
using System.Collections.Generic;

namespace Mirzipan.Infusion.Resolvers
{
    public class FactoryResolver: IResolver
    {
        private readonly Stack<object> _instances = new();
        private readonly Type _targetType;

        public FactoryResolver(Type targetType)
        {
            _targetType = targetType;
        }

        public object Resolve(InjectionContainer container)
        {
            var result = container.Instantiate(_targetType);
            _instances.Push(result);
            return result;
        }

        public void Dispose()
        {
            while (_instances.Count > 0)
            {
                var instance = _instances.Pop();
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _instances.Clear();
        }
    }
}