using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.Micro
{
    internal class DictionaryRequest : Dictionary<string, object>, IDynamicRequest
    {
        public DictionaryRequest(IReadOnlyDictionary<string, object> fields)
        {
            foreach(var kvp in fields) Add(kvp.Key, kvp.Value);
        }

        public DynamicValueResult<object> LookupValue(string key)
        {
            var lookup = this.ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase);

            if (lookup.TryGetValue(key, out var value))
            {
                return new DynamicValueResult<object>(value);
            }

            return new DynamicValueResult<object>();
        }
    }

    public interface IDynamicRequest : IRequest
    {
        DynamicValueResult<object> LookupValue(string key);
    }

    public class DynamicValueResult<T>
    {
        public DynamicValueResult()
        {
            Found = false;
        }

        public DynamicValueResult(T value)
        {
            Found = true;
            Value = value;
        }

        public bool Found { get; }
        public T Value { get; }
    }
}
