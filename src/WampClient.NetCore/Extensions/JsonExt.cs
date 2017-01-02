using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WampClient.Core.Extensions
{
    public static class JsonExt
    {

        public static bool IsValidJArray(this string input)
        {
            try
            {
                JArray.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public static bool IsValidJObject(this string input)
        {
            try
            {
                JObject.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
