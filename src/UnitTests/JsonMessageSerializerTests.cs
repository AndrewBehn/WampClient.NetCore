using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WampClient.Core;
using WampClient.Core.Exceptions;
using WampClient.Core.JsonSerialization;
using WampClient.Core.Messages;
using Xunit;

namespace UnitTests
{
    public class StringArgs : Payload<string>
    {
        public StringArgs(string args, string argKws) : base(args, argKws)
        {
        }
    }

    public class JsonMessageSerializerWritingTests
    {
        [Fact]
        public void WritingHello()
        {
            var serializer = new JsonMessageSerializer();
            var result = serializer.Write(new Hello("realm", new HelloDetails()));
            Assert.Equal(@"[1,""realm"",{""roles"":{""publisher"":{},""subscriber"":{},""caller"":{},""callee"":{}}}]",
                result);
        }

        [Fact]
        public void WritingWelcome()
        {
            var serializer = new JsonMessageSerializer();
            var result = serializer.Write(new Welcome(1, new WelcomeDetails()));
            Assert.Equal(@"[2,1,{}]", result);
        }

        [Fact]
        public void WritingAbort()
        {
            var serializer = new JsonMessageSerializer();
            var result = serializer.Write(new Abort(null, "abort"));
            Assert.Equal(@"[3,{},""abort""]", result);
        }

        [Fact]
        public void WritingGoodbye()
        {
            var serializer = new JsonMessageSerializer();
            var result = serializer.Write(new Goodbye(null, "goodbye"));
            Assert.Equal(@"[6,{},""goodbye""]", result);
        }

        [Fact]
        public void WritingPublish()
        {
            var serializer = new JsonMessageSerializer();
            var result = serializer.Write(new Publish<string>(1, null, "topic", null));
            Assert.Equal(@"[16,1,{},""topic""]", result);
        }

        [Fact]
        public void WritingPublished()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Published(1, 1));
            Assert.Equal(@"[17,1,1]", r);
        }

        [Fact]
        public void WritingSubscribe()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Subscribe(1, null, "topic"));
            Assert.Equal(@"[32,1,{},""topic""]", r);
        }

        [Fact]
        public void WritingSubscribed()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Subscribed(1, 1));
            Assert.Equal(@"[33,1,1]", r);
        }

        [Fact]
        public void WritingUnsubscribe()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Unsubscribe(1, 1));
            Assert.Equal(@"[34,1,1]", r);
        }

        [Fact]
        public void WriringUnsubscribed()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Unsubscribed(1));
            Assert.Equal(@"[35,1]", r);
        }

        [Fact]
        public void WritingEvent()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Event<string>(1, 1, null, null));
            Assert.Equal(@"[36,1,1,{}]", r);
        }

        [Fact]
        public void WritingCall()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Call<string>(1, null, "procedure", null));
            Assert.Equal(@"[48,1,{},""procedure""]", r);
        }

        [Fact]
        public void WritingResult()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Result<string>(1, null, null));
            Assert.Equal(@"[50,1,{}]", r);
        }

        [Fact]
        public void WritingRegister()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Register(1, null, "procedure"));
            Assert.Equal(@"[64,1,{},""procedure""]", r);
        }

        [Fact]
        public void WritingRegistered()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Registered(1, 1));
            Assert.Equal(@"[65,1,1]", r);
        }

        [Fact]
        public void WritingUnregister()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Unregister(1, 1));
            Assert.Equal(@"[66,1,1]", r);
        }

        [Fact]
        public void WritingUnregistered()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Unregistered(1));
            Assert.Equal(@"[67,1]", r);
        }


        [Fact]
        public void WritingInvocation()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Invocation<string>(1, 1, null, null));
            Assert.Equal(@"[68,1,1,{}]", r);
        }

        [Fact]
        public void WritingYield()
        {
            var serializer = new JsonMessageSerializer();
            var r = serializer.Write(new Yield<string>(1, null, null));
            Assert.Equal(@"[70,1,{}]", r);
        }
        
        public static readonly string DummyArgs = "[\"dummyArg\"]";
        public static readonly string DummyKwArgs = "{\"dummyKw\":\"dummyValue\"}";
        public static readonly string EmptyArray = "[]";
        public static readonly string EmptyObject = "{}";
        public static readonly string DummyPayloadString = $"{DummyArgs},{DummyKwArgs}";
        public static readonly string ArgumentOnlyPayloadString = $"{DummyArgs}";
        public static readonly string ArgKwOnlyPayloadString = $"{EmptyArray},{DummyKwArgs}";

        public static readonly Payload<string> DummyPayload = new JsonPayload(DummyArgs, DummyKwArgs);
        public static readonly Payload<string> ArgumentOnlyPayload = new JsonPayload(DummyArgs, null);
        public static readonly Payload<string> ArgKwOnlyPayload = new JsonPayload(null, DummyKwArgs);

        [Fact]
        public void WritingEventWithPayload()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Event<string>(1, 1, null, DummyPayload));
            Assert.Equal($"[36,1,1,{EmptyObject},{DummyPayloadString}]", r);
        }

        [Fact]
        public void WritingCallWithPayload()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Call<string>(1, null, "procedure", DummyPayload));
            Assert.Equal($"[48,1,{EmptyObject},\"procedure\",{DummyPayloadString}]", r);
        }

        [Fact]
        public void WritingResultWithPayload()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Result<string>(1, null, DummyPayload));
            Assert.Equal($"[50,1,{EmptyObject},{DummyPayloadString}]", r);
        }

        [Fact]
        public void WritingInvocationWithPayload()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Invocation<string>(1, 1, null, DummyPayload));
            Assert.Equal($"[68,1,1,{EmptyObject},{DummyPayloadString}]", r);
        }

        [Fact]
        public void WritingYieldWithPayload()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Yield<string>(1, null, DummyPayload));
            Assert.Equal($"[70,1,{EmptyObject},{DummyPayloadString}]", r);
        }

        ////////

        [Fact]
        public void WritingEventWithArgs()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Event<string>(1, 1, null, ArgumentOnlyPayload));
            Assert.Equal($"[36,1,1,{EmptyObject},{ArgumentOnlyPayloadString}]", r);

        }

        [Fact]
        public void WritingCallWithArgs()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Call<string>(1, null, "procedure", ArgumentOnlyPayload));
            Assert.Equal($"[48,1,{EmptyObject},\"procedure\",{ArgumentOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingResultWithArgs()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Result<string>(1, null, ArgumentOnlyPayload));
            Assert.Equal($"[50,1,{EmptyObject},{ArgumentOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingInvocationWithArgs()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Invocation<string>(1, 1, null, ArgumentOnlyPayload));
            Assert.Equal($"[68,1,1,{EmptyObject},{ArgumentOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingYieldWithArgs()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Yield<string>(1, null, ArgumentOnlyPayload));
            Assert.Equal($"[70,1,{EmptyObject},{ArgumentOnlyPayloadString}]", r);
        }

        /// 

        [Fact]
        public void WritingEventWithArgsKws()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Event<string>(1, 1, null, ArgKwOnlyPayload));
            Assert.Equal($"[36,1,1,{EmptyObject},{ArgKwOnlyPayloadString}]", r);

        }

        [Fact]
        public void WritingCallWithArgsKws()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Call<string>(1, null, "procedure", ArgKwOnlyPayload));
            Assert.Equal($"[48,1,{EmptyObject},\"procedure\",{ArgKwOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingResultWithArgsKws()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Result<string>(1, null, ArgKwOnlyPayload));
            Assert.Equal($"[50,1,{EmptyObject},{ArgKwOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingInvocationWithArgsKws()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Invocation<string>(1, 1, null, ArgKwOnlyPayload));
            Assert.Equal($"[68,1,1,{EmptyObject},{ArgKwOnlyPayloadString}]", r);
        }

        [Fact]
        public void WritingYieldWithArgsKws()
        {
            var s = new JsonMessageSerializer();
            var r = s.Write(new Yield<string>(1, null, ArgKwOnlyPayload));
            Assert.Equal($"[70,1,{EmptyObject},{ArgKwOnlyPayloadString}]", r);
        }




        ///////
        public static readonly string Garbage = "thisIsNotValidJson";
        public static readonly Payload<string> InvalidArgPaylod = new StringArgs(Garbage, EmptyObject);
        ////////

        [Fact]
        public void WritingEventWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Event<string>(1, 1, null, InvalidArgPaylod);

            Assert.Throws<InvalidArgumentsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingCallWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Call<string>(1, null, "procedure", InvalidArgPaylod);
            Assert.Throws<InvalidArgumentsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingResultWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Result<string>(1, null, InvalidArgPaylod);
            Assert.Throws<InvalidArgumentsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingInvocationWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Invocation<string>(1, 1, null, InvalidArgPaylod);
            Assert.Throws<InvalidArgumentsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingYieldWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Yield<string>(1, null, InvalidArgPaylod);
            Assert.Throws<InvalidArgumentsException>(() => serializer.Write(s));
        }

        public static readonly Payload<string> InvalidArgKwsPayload = new StringArgs(EmptyArray, Garbage);

        [Fact]
        public void WritingEventWithInvalidArgKws()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Event<string>(1, 1, null, InvalidArgKwsPayload);

            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingCallWithInvalidArgKws()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Call<string>(1, null, "procedure", InvalidArgKwsPayload);
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingResultWithInvalidArgKws()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Result<string>(1, null, InvalidArgKwsPayload);
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingInvocationWithInvalidArgKws()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Invocation<string>(1, 1, null, InvalidArgKwsPayload);
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Write(s));
        }

        [Fact]
        public void WritingYieldWithInvalidArgKws()
        {
            var serializer = new JsonMessageSerializer();
            var s = new Yield<string>(1, null, InvalidArgKwsPayload);
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Write(s));
        }
    }

    public class JsonMessageSerializerReadingTests
    {
        [Fact]
        public void ReadingHello()
        {
            var serializer = new JsonMessageSerializer();
            string s =
                @"[1,""realm"",{""roles"":{""publisher"":{},""subscriber"":{},""caller"":{},""callee"":{}}}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingWelcome()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[2,1,{}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingAbort()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[3,{},""abort""]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingGoodbye()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[6,{},""goodbye""]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingPublish()
        {
            var serializer = new JsonMessageSerializer();
            string s = (@"[16,1,{},""topic""]");
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingPublished()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[17,1,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingSubscribe()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[32,1,{},""topic""]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingSubscribed()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[33,1,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingUnsubscribe()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[34,1,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void WriringUnsubscribed()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[35,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingEvent()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[36,1,1,{}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingCall()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[48,1,{},""procedure""]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingResult()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[50,1,{}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingRegister()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[64,1,{},""procedure""]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingRegistered()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[65,1,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingUnregister()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[66,1,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingUnregistered()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[67,1]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingInvocation()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[68,1,1,{}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingYield()
        {
            var serializer = new JsonMessageSerializer();
            string s = @"[70,1,{}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        public static readonly string DummyArgs = "[\"dummyArg\"]";
        public static readonly string DummyKwArgs = "{\"dummyKw\":\"dummyValue\"}";

        public static readonly string EmptyArray = "[]";
        public static readonly string EmptyObject = "{}";
        public static readonly string DummyPayloadString = $"{DummyArgs},{DummyKwArgs}";
        public static readonly string ArgumentOnlyPayloadString = $"{DummyArgs}";
        public static readonly string ArgKwOnlyPayloadString = $"{EmptyArray},{DummyKwArgs}";
        
        [Fact]
        public void ReadingEventWithPayload()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[36,1,1,{EmptyObject},{DummyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingCallWithPayload()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[48,1,{EmptyObject},\"procedure\",{DummyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingResultWithPayload()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[50,1,{EmptyObject},{DummyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingInvocationWithPayload()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[68,1,1,{EmptyObject},{DummyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingYieldWithPayload()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[70,1,{EmptyObject},{DummyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        ////////

        [Fact]
        public void ReadingEventWithArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[36,1,1,{EmptyObject},{ArgumentOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingCallWithArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[48,1,{EmptyObject},\"procedure\",{ArgumentOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingResultWithArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[50,1,{EmptyObject},{ArgumentOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingInvocationWithArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[68,1,1,{EmptyObject},{ArgumentOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingYieldWithArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[70,1,{EmptyObject},{ArgumentOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        /// 

        [Fact]
        public void ReadingEventWithArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[36,1,1,{EmptyObject},{ArgKwOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingCallWithArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[48,1,{EmptyObject},\"procedure\",{ArgKwOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingResultWithArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[50,1,{EmptyObject},{ArgKwOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingInvocationWithArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[68,1,1,{EmptyObject},{ArgKwOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        [Fact]
        public void ReadingYieldWithArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[70,1,{EmptyObject},{ArgKwOnlyPayloadString}]";
            var r = serializer.Read(s);
            var w = serializer.Write(r);

            Assert.Equal(s, w);
        }

        public static readonly string InvalidArgPaylodString = $"{EmptyObject},{EmptyObject}";
        ////////

        [Fact]
        public void ReadingEventWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[36,1,1,{EmptyObject},{InvalidArgPaylodString}]";

            Assert.Throws<InvalidArgumentsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingCallWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[48,1,{EmptyObject},\"procedure\",{InvalidArgPaylodString}]";
            Assert.Throws<InvalidArgumentsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingResultWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[50,1,{EmptyObject},{InvalidArgPaylodString}]";
            Assert.Throws<InvalidArgumentsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingInvocationWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[68,1,1,{EmptyObject},{InvalidArgPaylodString}]";
            Assert.Throws<InvalidArgumentsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingYieldWithInvalidArgs()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[70,1,{EmptyObject},{InvalidArgPaylodString}]";
            Assert.Throws<InvalidArgumentsException>(() => serializer.Read(s));
        }

        public static readonly string InvalidArgKwsPayloadString = $"{EmptyArray},{EmptyArray}";

        [Fact]
        public void ReadingEventWithInvalidArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[36,1,1,{EmptyObject},{InvalidArgKwsPayloadString}]";

            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingCallWithInvalidArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[48,1,{EmptyObject},\"procedure\",{InvalidArgKwsPayloadString}]";
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingResultWithInvalidArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[50,1,{EmptyObject},{InvalidArgKwsPayloadString}]";
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingInvocationWithInvalidArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[68,1,1,{EmptyObject},{InvalidArgKwsPayloadString}]";
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Read(s));
        }

        [Fact]
        public void ReadingYieldWithInvalidArgsKws()
        {
            var serializer = new JsonMessageSerializer();
            string s = $"[70,1,{EmptyObject},{InvalidArgKwsPayloadString}]";
            Assert.Throws<InvalidArgumentKeywordsException>(() => serializer.Read(s));
        }
    }
}