using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WampClient.Core.Messages;

namespace WampClient.Core.MsgPackSerialization
{
    public class MsgpackPayload : Payload<byte[]>
    {
        public MsgpackPayload(byte[] args, byte[] argKws) : base(args, argKws)
        {
            //TODO: validate arguments
        }

        public static bool IsValidMsgpackArray(byte[] input)
        {
            try
            {
                throw new Exception();
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static bool IsValidMsgpackObject(byte[] input)
        {
            try
            {
                throw new Exception();
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
