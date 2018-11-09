using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CitadelCore.Websockets
{
    public static class SystemClientWebSocket
    {
        /// <summary>
        /// False if CitadelCore.Websockets.ClientWebSocket is available on this platform, true if
        /// CitadelCore.Websockets.Managed.ClientWebSocket is required.
        /// </summary>
        public static bool ManagedWebSocketRequired => _managedWebSocketRequired.Value;

        static Lazy<bool> _managedWebSocketRequired => new Lazy<bool>(CheckManagedWebSocketRequired);

        private static bool CheckManagedWebSocketRequired()
        {
            try
            {
                using (var clientWebSocket = new ClientWebSocket())
                {
                    return false;
                }
            }
            catch (PlatformNotSupportedException)
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a ClientWebSocket that works for this platform. Uses
        /// CitadelCore.Websockets.ClientWebSocket if supported or
        /// CitadelCore.Websockets.Managed.ClientWebSocket if not.
        /// </summary>
        public static WebSocket CreateClientWebSocket()
        {
            if (ManagedWebSocketRequired)
            {
                return new Managed.ClientWebSocket();
            }
            else
            {
                return new ClientWebSocket();
            }
        }

        /// <summary>
        /// Creates and connects a ClientWebSocket that works for this platform. Uses
        /// CitadelCore.Websockets.ClientWebSocket if supported or
        /// CitadelCore.Websockets.Managed.ClientWebSocket if not.
        /// </summary>
        public static async Task<WebSocket> ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            var clientWebSocket = CreateClientWebSocket();
            await clientWebSocket.ConnectAsync(uri, cancellationToken);
            return clientWebSocket;
        }

        public static Task ConnectAsync(this WebSocket clientWebSocket, Uri uri, CancellationToken cancellationToken)
        {
            if (clientWebSocket is ClientWebSocket)
            {
                return (clientWebSocket as ClientWebSocket).ConnectAsync(uri, cancellationToken);
            }
            else if (clientWebSocket is Managed.ClientWebSocket)
            {
                return (clientWebSocket as Managed.ClientWebSocket).ConnectAsync(uri, cancellationToken);
            }

            throw new ArgumentException("WebSocket must be an instance of CitadelCore.Websockets.ClientWebSocket or CitadelCore.Websockets.Managed.ClientWebSocket", nameof(clientWebSocket));
        }
    }
}