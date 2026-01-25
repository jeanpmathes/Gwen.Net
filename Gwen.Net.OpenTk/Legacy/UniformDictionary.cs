using System;
using System.Collections.Generic;

namespace Gwen.Net.OpenTk.Legacy
{
    public class UniformDictionary
    {
        private readonly Dictionary<String, Int32> data;
        private readonly Int32 program;
        private readonly Func<Int32, String, Int32> uniformLocationResolver;

        public UniformDictionary(Int32 program, Func<Int32, String, Int32> uniformLocationResolver)
        {
            data = new Dictionary<String, Int32>();
            this.program = program;
            this.uniformLocationResolver = uniformLocationResolver;
        }

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
}