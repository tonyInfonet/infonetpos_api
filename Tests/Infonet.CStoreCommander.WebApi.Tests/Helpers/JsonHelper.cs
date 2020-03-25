using Newtonsoft.Json;

namespace Infonet.CStoreCommander.WebApi.Tests.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Method to convert a json result to object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="json">Json</param>
        /// <returns>Object</returns>
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Method to convert an object to json
        /// </summary>
        /// <param name="instance">Object</param>
        /// <returns>Json</returns>
        public static string ToJson(object instance)
        {
           return JsonConvert.SerializeObject(instance.GetType());
        }
    }
}
