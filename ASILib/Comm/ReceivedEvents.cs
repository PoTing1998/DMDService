using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm
{
    /// <summary>
    /// Event handlers for data received.
    /// </summary>
    public class ReceivedEvents
    {
        /// <summary>
        /// Declare delegation for data received.
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="source"></param>
        public delegate void ReceivedEventHandler(byte[] dataBytes, string source);

        /// <summary>
        /// Declare delegation for client connected.
        /// </summary>
        /// <param name="source"></param>
        public delegate void ConnectedEventHandler(string source);

        /// <summary>
        /// Declare delegation while error occurred.
        /// </summary>
        /// <param name="exception"></param>
        public delegate void ErrorEventHandlers(System.Exception exception);

        /// <summary>
        /// Server Is Open
        /// </summary>
        /// <param name="source"></param>
        public delegate void OpenEventHandlers(string source);

        /// <summary>
        /// Server Is Close
        /// </summary>
        /// <param name="source"></param>
        public delegate void CloseEventHandlers(string source);

        /// <summary>
        /// Declare delegation while client disconnected.
        /// </summary>
        /// <param name="source"></param>
        public delegate void DisconnectedEventHandler(string source);

        /// <summary>
        /// Declare delegation while other side disconnect
        /// </summary>
        /// <param name="source"></param>
        public delegate void OtherSideDisconnectEventHandler(string source);
    }
}
