using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dotnetCampus.Cli.Parsers
{
    internal class ImmutableRuntimeOptionParser<T> : RuntimeCommandLineOptionParser<T>
    {
        private readonly ConstructorInfo _constructor;
        private readonly object[] _values;
        private readonly Dictionary<PropertyInfo, int> _indexDictionary = new Dictionary<PropertyInfo, int>();
        private readonly Dictionary<int, PropertyInfo> _indexedValueDictionary = new Dictionary<int, PropertyInfo>();
        private readonly Dictionary<char, PropertyInfo> _shortNameDictionary = new Dictionary<char, PropertyInfo>();
        private readonly Dictionary<string, PropertyInfo> _longNameDictionary = new Dictionary<string, PropertyInfo>();

        public ImmutableRuntimeOptionParser(string verb, IReadOnlyList<PropertyInfo> attributedProperties) : base(verb)
        {
            var properties = attributedProperties;

            _values = new object[properties.Count];
            _constructor = typeof(T).GetConstructor(properties.Select(x => x.PropertyType).ToArray());
            for (var i = 0; i < properties.Count; i++)
            {
                var propertyInfo = properties[i];
                _indexDictionary[propertyInfo] = i;

                if (propertyInfo.IsDefined(typeof(OptionAttribute)))
                {
                    var attribute = propertyInfo.GetCustomAttribute<OptionAttribute>();
                    if (attribute.ShortName != null)
                    {
                        _shortNameDictionary[attribute.ShortName.Value] = propertyInfo;
                    }

                    if (!string.IsNullOrWhiteSpace(attribute.LongName))
                    {
                        _longNameDictionary[attribute.LongName] = propertyInfo;
                    }
                }

                if (propertyInfo.IsDefined(typeof(ValueAttribute)))
                {
                    var attribute = propertyInfo.GetCustomAttribute<ValueAttribute>();
                    _indexedValueDictionary[attribute.Index] = propertyInfo;
                }
            }
        }

        public override void SetValue(int index, string value)
        {
            _values[_indexDictionary[_indexedValueDictionary[index]]] = value;
        }

        public override void SetValue(char shortName, bool value)
        {
            _values[_indexDictionary[_shortNameDictionary[shortName]]] = value;
        }

        public override void SetValue(char shortName, string value)
        {
            _values[_indexDictionary[_shortNameDictionary[shortName]]] = value;
        }

        public override void SetValue(char shortName, IReadOnlyList<string> values)
        {
            _values[_indexDictionary[_shortNameDictionary[shortName]]] = values;
        }

        public override void SetValue(string longName, bool value)
        {
            _values[_indexDictionary[_longNameDictionary[longName]]] = value;
        }

        public override void SetValue(string longName, string value)
        {
            _values[_indexDictionary[_longNameDictionary[longName]]] = value;
        }

        public override void SetValue(string longName, IReadOnlyList<string> values)
        {
            _values[_indexDictionary[_longNameDictionary[longName]]] = values;
        }

        public override T Commit()
        {
            return (T) _constructor.Invoke(_values);
        }
    }
}