using System;
using System.Collections.Generic;

namespace Mirzipan.Infusion.Meta
{
    public class InjectionIndexer
    {
        public static readonly InjectionIndexer Instance = new();

        private readonly Dictionary<Type, TypeInjectionInfo> _types = new();

        #region Queries

        public void Index(Type type)
        {
            if (_types.ContainsKey(type))
            {
                return;
            }

            _types[type] = new TypeInjectionInfo(type);
        }

        public bool TryGetInfo(Type type, out TypeInjectionInfo info)
        {
            return _types.TryGetValue(type, out info);
        }

        #endregion Queries

        #region Manipulation

        public void Clear()
        {
            _types.Clear();
        }

        #endregion Manipulation
    }
}