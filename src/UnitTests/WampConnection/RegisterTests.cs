using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.WampConnection;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using Xunit;

namespace UnitTests.WampConnection
{
    public class RegisterTests
    {
        [Fact]
        public async Task RegisterWithImmediateReturn()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, Registered>(_ => true, r => new Registered(r.Request, 1));
            var registration = await config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null));

            Assert.NotNull(registration);
        }

        [Fact]
        public async Task RegisterWithDelayedReturn()
        {
            var config = new WampConnectTestConfig();
            var registerTask = config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null));
            var registerMsg = config.SentMessages.OfType<Register>().Single();
            config.ReceivedMessages.OnNext(new Registered(registerMsg.Request, 1));

            var registered = await registerTask;
            Assert.NotNull(registered);
        }

        [Fact]
        public async Task RegisterWithImmediateError()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, RegisterError>(_ => true,
                r => new RegisterError(r.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<RegisterException>(() => config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null)));
        }

        [Fact]
        public async Task RegisterWithDelayedError()
        {
            var config = new WampConnectTestConfig();
            var registerTask = config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null));
            var registerMsg = config.SentMessages.OfType<Register>().Single();
            config.ReceivedMessages.OnNext(new RegisterError(registerMsg.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<RegisterException>(() => registerTask);
        }

        [Fact]
        public async Task RegisterDiscriminatesIncomingRegsiterErrorMessages()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, RegisterError>(_ => true, r => new RegisterError(r.Request + 1, null, "ruhroh"));
            config.SetupMessageResponse<Register, Registered>(_ => true, r => new Registered(r.Request, 1));

            var registered = await config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null));
            Assert.NotNull(registered);
        }

        [Fact]
        public async Task RegisterDiscriminatesIncomingReigsteredMessages()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, Registered>(r => new Registered(r.Request + 1, 1));
            config.SetupMessageResponse<Register, RegisterError>(r => new RegisterError(r.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<RegisterException>(() => config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null)));
        }

        [Fact]
        public async Task ThrowingInvocationExceptionInHandlerReturnsSendsInvocationError()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, Registered>(_ => true, r => new Registered(r.Request, 1));
            var registration = await config.Subject.Register("procedure", async i =>
            {
                await Task.Delay(0);
                throw new InvocationException("ruhroh");
            });

            config.ReceivedMessages.OnNext(new Invocation<string>(1, registration.Registered.Registration, null, null));
            Assert.NotNull(config.SentMessages.OfType<InvocationError<string>>().Single());
        }

        [Fact]
        public async Task InvocationsAreDiscrimiatedByRegistration()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Register, Registered>(_ => true, r => new Registered(r.Request, 1));
            var registration = await config.Subject.Register("procedure", _ => Task.FromResult<Payload<string>>(null));
            config.ReceivedMessages.OnNext(new Invocation<string>(1, registration.Registered.Registration, null, null));
            config.ReceivedMessages.OnNext(new Invocation<string>(2, registration.Registered.Registration + 1, null,
                null));
            Assert.Equal(1, config.SentMessages.OfType<Yield<string>>().Count());
        }


    }
}