namespace Mirzipan.Infusion.Resolvers
{
    public class InstanceResolver: IResolver
    {
        private object _instance;
        
        public InstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Resolve(InjectionContainer container)
        {
            return _instance;
        }

        public void Dispose()
        {
        }
    }
}