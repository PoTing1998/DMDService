using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm
{
    /// <summary>
    /// An Interface for communications.
    /// </summary>
    public interface ICommunication
    {
        #region Properties
        /// <summary>
        /// Get or Set connection string for establish a connection.
        /// </summary>
        string ConnectionString
        {
            get;
            set;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Open a configured communication.
        /// </summary>
        /// <returns>0: Connection established successful.</returns>
        /// <returns>-1: Connection established fail.</returns>
        int Open();

        /// <summary>
        /// Close an exists communication.
        /// </summary>
        /// <returns>0: Connection closed successful.</returns>
        /// <returns>-1: Connection closed fail.</returns>
        int Close();

        /// <summary>
        /// Send data using established connection.
        /// </summary>
        /// <param name="dataText">Text for sending.</param>
        /// <returns>0: Data sent successful.</returns>
        /// <returns>-1: Data sent fail.</returns>
        int Send(string dataText);

        /// <summary>
        /// Send data to specific target using established connection.
        /// </summary>
        /// <param name="dataText"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        int Send(string dataText, string target);

        /// <summary>
        /// Send data using established connection.
        /// </summary>
        /// <param name="dataBytes">Bytes for sending.</param>
        /// <returns>0: Data sent successful.</returns>
        /// <returns>-1: Data sent fail.</returns>
        int Send(byte[] dataBytes);

        /// <summary>
        /// Send data to specific target using established connection.
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        int Send(byte[] dataBytes, string target);
        #endregion


        #region Events
        /// <summary>
        /// Event fired while data received from established connection.
        /// </summary>
        event ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler ReceivedEvent;

        /// <summary>
        /// Event fired while client connected from established connection.
        /// </summary>
        event ASI.Lib.Comm.ReceivedEvents.ConnectedEventHandler ConnectedEvent;

        /// <summary>
        /// Event fired while communication error occurred.
        /// </summary>
        event ASI.Lib.Comm.ReceivedEvents.ErrorEventHandlers ErrorEvent;

        /// <summary>
        /// Event fired while client disconnected from established connection.
        /// </summary>
        event ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler DisconnectedEvent;
        #endregion
    }
}
