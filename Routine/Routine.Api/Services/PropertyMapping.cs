using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappigDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappigDictionary)
        {
            MappigDictionary = mappigDictionary ?? throw new ArgumentNullException(nameof(mappigDictionary));
        }
    }
}
