using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using Xunit;

namespace UnitTests.WampConnection
{
    public class CallTests
    {
        [Fact]
        public async Task CallWithImmediateReturn()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Call<string>, Result<string>>(_ => true,
                call => new Result<string>(call.Request, null, null));

            var result = await config.Subject.Call("procedure", null);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CallWithImmediateError()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Call<string>, CallError>(_ => true, call => new CallError(call.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<CallException>(() => config.Subject.Call("procedure", null));
        }

        [Fact]
        public async Task CallWithDelayedReturn()
        {
            var config = new WampConnectTestConfig();
            var callTask = config.Subject.Call("procedure", null);

            var callMessage = config.SentMessages.OfType<Call<string>>().Single();
            config.ReceivedMessages.OnNext(new Result<string>(callMessage.Request, null, null));
            var result = await callTask;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CallWithDelayedError()
        {
            var config = new WampConnectTestConfig();
            var callTask = config.Subject.Call("procedure", null);

            var callMsg = config.SentMessages.OfType<Call<string>>().Single();
            config.ReceivedMessages.OnNext(new CallError(callMsg.Request, null, "ruhroh"));
            await Assert.ThrowsAsync<CallException>(() => callTask);
        }

        [Fact]
        public async Task CallDiscriminatesIncomingResultMessages()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Call<string>, Result<string>>(c => new Result<string>(c.Request + 1, null, null));
            config.SetupMessageResponse<Call<string>, CallError>(c => new CallError(c.Request, null, "ruhroh"));

            var result = await config.Subject.Call("procedure", null);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CallDiscriminatesIncomingCallErrorMessages()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Call<string>, CallError>(c => new CallError(c.Request +1, null, "ruhroh"));
            config.SetupMessageResponse<Call<string>, Result<string>>(c => new Result<string>(c.Request, null, null));

            await Assert.ThrowsAsync<CallException>(() => config.Subject.Call("procedure", null));
        }
    }
}
