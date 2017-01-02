using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using WampClient.Core.Extensions;

namespace WampClient.Core.JsonSerialization
{
    public class JsonPayload : Payload<string>
    {
        public static readonly JsonPayload Empty = new JsonPayload(null, null);

        public JsonPayload(string args, string argKws) : base(args, argKws)
        {
            if (!string.IsNullOrEmpty(args) && !args.IsValidJArray())
                throw new InvalidArgumentsException();
            if (!string.IsNullOrEmpty(argKws) && !argKws.IsValidJObject())
                throw new InvalidArgumentKeywordsException();
        }
    }
}
