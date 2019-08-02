using System;
using System.Collections.Generic;
using System.Reflection;

namespace dotnetCampus.Cli.Parsers
{
    internal class RuntimeOptionParser<T> : RuntimeCommandLineOptionParser<T>
    {
        private readonly T _options;
        private readonly Dictionary<int, PropertyInfo> _indexedValueDictionary = new Dictionary<int, PropertyInfo>();
        private readonly Dictionary<char, PropertyInfo> _shortNameDictionary = new Dictionary<char, PropertyInfo>();
        private readonly Dictionary<string, PropertyInfo> _longNameDictionary = new Dictionary<string, PropertyInfo>();

        public RuntimeOptionParser(string verb, IReadOnlyList<PropertyInfo> attributedProperties) : base(verb)
        {
            _options = Activator.CreateInstance<T>();
            foreach (var propertyInfo in attributedProperties)
            {
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
            if (_indexedValueDictionary.TryGetValue(index, out var property))
            {
                property.SetValue(_options, value);
            }
        }

        public override void SetValue(char shortName, bool value)
        {
            if (_shortNameDictionary.TryGetValue(shortName, out var property))
            {
                property.SetValue(_options, value);
            }
        }

        public override void SetValue(char shortName, string value)
        {
            if (_shortNameDictionary.TryGetValue(shortName, out var property))
            {
                property.SetValue(_options, value);
            }
        }

        public override void SetValue(char shortName, IReadOnlyList<string> values)
        {
            if (_shortNameDictionary.TryGetValue(shortName, out var property))
            {
                property.SetValue(_options, values);
            }
        }

        public override void SetValue(string longName, bool value)
        {
            if (_longNameDictionary.TryGetValue(longName, out var property))
            {
                property.SetValue(_options, value);
            }
        }

        public override void SetValue(string longName, string value)
        {
            if (_longNameDictionary.TryGetValue(longName, out var property))
            {
                property.SetValue(_options, value);
            }
        }

        public override void SetValue(string longName, IReadOnlyList<string> values)
        {
            if (_longNameDictionary.TryGetValue(longName, out var property))
            {
                property.SetValue(_options, values);
            }
        }

        public override T Commit()
        {
            return _options;
        }
    }
}