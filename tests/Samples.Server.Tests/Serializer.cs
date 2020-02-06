#if NETSTANDARD2_0
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace Samples.Server.Tests
{
    public static class Serializer
    {
        public static string Serialize(object obj)
#if NETSTANDARD2_0
            => JsonConvert.SerializeObject(obj);
#else
            => JsonSerializer.Serialize(obj);
#endif
    }
}
