using System;
using System.Collections.Generic;

namespace Gwen.Net.OpenTk.New.Graphics;

public class UniformDictionary(Int32 program, Func<Int32, String, Int32> uniformLocationResolver)
{
    private readonly Dictionary<String, Int32> data = new();

    public Int32 this[String key]
    {
        get
        {
            if (data.TryGetValue(key, out Int32 loc))
            {
                return loc;
            }

            Int32 uniformLocation = uniformLocationResolver.Invoke(program, key);
            data.Add(key, uniformLocation);

            return uniformLocation;
        }
    }
}
