using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friflo.Engine.ECS.Extensions;

public static class HashSetExtensions
{
    public static bool TryGetValue<T>(this HashSet<T> set, T key, out T value)
    {
        T? tempValue = set.FirstOrDefault(s => key.Equals(s));
        if (tempValue == null)
        {
            value = default;
            return false;
        }
        else
        {
            value = tempValue;
            return true;
        }
    }
}
