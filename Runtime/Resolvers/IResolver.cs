using System;

namespace Mirzipan.Infusion.Resolvers
{
    public interface IResolver : IDisposable
    {
        object Resolve(InjectionContainer container);
    }
}