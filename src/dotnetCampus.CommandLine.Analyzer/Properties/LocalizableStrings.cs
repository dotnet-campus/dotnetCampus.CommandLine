using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace dotnetCampus.CommandLine.Properties
{
    public static class LocalizableStrings
    {
        public static LocalizableString Get(string key) => new LocalizableResourceString(key, Resources.ResourceManager, typeof(Resources));
    }
}
