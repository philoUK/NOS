using Newtonsoft.Json;

namespace NewOrbit.Messaging.Shared
{
    public static class JsonExtensions
    {
        public static string ToJson(this object target)
        {
            return JsonConvert.SerializeObject(target);
        }
    }
}
