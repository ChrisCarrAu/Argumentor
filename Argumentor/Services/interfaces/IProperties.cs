using System.Collections.Generic;
using System.Reflection;

namespace ArgumentRes.Services.interfaces
{
    public interface IProperties<in T>
    {
        PropertyInfo Switch(string key);
        bool IsBooleanSwitch(string param);
        IEnumerable<string> MissingArguments { get; }
        void SetFlagValue(T obj, string param, object arg);
        void SetPropertyValue(T obj, object arg);
    }
}
