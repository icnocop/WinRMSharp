﻿using System.Net;
using WinRMSharp.Tests.Sessions;
using Xunit;

namespace WinRMSharp.Tests
{
    [Trait("Category", "InMemory")]
    public class InMemoryClientTests : BaseClientTests, IDisposable
    {
        private readonly SessionManager _sessionManager = new SessionManager();

        [Fact]
        public void ConstructClient()
        {
            string baseUrl = "https://127.0.0.1/wsman";
            string userName = "exampleUser";
            string password = "examplePassword";

            ICredentials credentials = new NetworkCredential(userName, password);

            WinRMClientOptions clientOptions = new WinRMClientOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5),
                MaxEnvelopeSize = 100,
                ReadTimeout = TimeSpan.FromSeconds(10)
            };

            WinRMClient client = new WinRMClient(baseUrl, credentials, clientOptions);

            Assert.NotNull(client.Transport);
            Assert.NotNull(client.Protocol);

            Assert.Equal(clientOptions.OperationTimeout, client.Protocol.OperationTimeout);
            Assert.Equal(clientOptions.MaxEnvelopeSize, client.Protocol.MaxEnvelopeSize);
            Assert.Equal(clientOptions.ReadTimeout, client.Transport.ReadTimeout);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _sessionManager.Dispose();
        }

        /// <inheritdoc/>
        public override WinRMClient GenerateClient(string sessionName)
        {
            Uri baseUrl = new Uri("https://127.0.0.1:5986/wsman");
            string userName = "exampleUser";
            string password = "examplePassword";

            Tuple<string, string, bool>[] replacements = new Tuple<string, string, bool>[]
            {
                Tuple.Create(baseUrl.ToString(), "https://127.0.0.1:5986/wsman", false),
                Tuple.Create(baseUrl.Host, "127.0.0.1", false),
                Tuple.Create(userName, "exampleUser", false),
            };

            DelegatingHandler handler = _sessionManager.GenerateSessionHandler(
                SessionState.Playback,
                sessionName,
                replacements);

            ICredentials credentials = new NetworkCredential(userName, password);
            ITransport transport = new Transport(baseUrl, handler, credentials);

            ProtocolOptions protocolOptions = new ProtocolOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5)
            };

            Protocol protocol = new Protocol(transport, new IncrementingGuidProvider(), protocolOptions);

            return new WinRMClient(protocol);
        }
    }
}
