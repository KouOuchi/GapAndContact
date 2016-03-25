using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Common
{

    [Serializable]
    public class CommsInfo
    {
        public string Message { get; set; }
        public DTLocation Location { get; set; }
        public DTLocationItem LocationItem { get; set; }

        public CommsInfo(string mesg)
        {
            Message = mesg;
        }
        public CommsInfo(string mesg, DTLocation location, DTLocationItem location_item)
        {
            Message = mesg;
            Location = location;
            LocationItem = location_item;
        }
    }

    /// <SUMMARY>
    /// This CallbackSink object will be 'anchored'
    /// on the client and is used as the target for a callback
    /// given to the server.
    /// </SUMMARY>
    public class CallbackSink : MarshalByRefObject
    {
        public event delCommsInfo OnHostToClient;

        public CallbackSink()
        { }

        [OneWay]
        public void HandleToClient(CommsInfo info)
        {
            if (OnHostToClient != null)
                OnHostToClient(info);
        }
    }

    //public delegate void delUserInfo(string UserID);
    public delegate void delCommsInfo(CommsInfo info);

    // This class is created on the server and allows
    // for clients to register their existance and
    // a call-back that the server uses to communicate back.
    public class ServerTalk : MarshalByRefObject
    {
        //    private static delUserInfo _NewUser;
        private static delCommsInfo _ClientToHost;
        private static List<ClientWrap> _list =
                        new List<ClientWrap>();

        public void RegisterHostToClient(delCommsInfo htc)
        {
            _list.Add(new ClientWrap(htc));
        }

        /// <SUMMARY>
        /// The host should register a function
        /// pointer to which it wants a signal
        /// send when a User Registers
        /// </SUMMARY>
        //    public static delUserInfo NewUser
        //    {
        //        get { return _NewUser; }
        //        set { _NewUser = value; }
        //    }

        /// <SUMMARY>
        /// The host should register a function pointer
        /// to which it wants the CommsInfo object
        /// send when the client wants
        /// to communicate to the server
        /// </SUMMARY>
        public static delCommsInfo ClientToHost
        {
            get { return _ClientToHost; }
            set { _ClientToHost = value; }
        }

        // The static method that will be invoked
        // by the server when it wants to send a message
        // to a specific user or all of them.
        public static void RaiseHostToClient(
                                             string Message, DTLocation loc, DTLocationItem loc_item)
        {
            foreach (ClientWrap client in _list)
            {
                if (client.HostToClient != null)
                    client.HostToClient(new CommsInfo(Message, loc, loc_item));
            }
        }

        // This instance method allows a client
        // to send a message to the server
        public void SendMessageToServer(CommsInfo Message)
        {
            if (_ClientToHost != null)
                // make sure there is a delegate to call!
                _ClientToHost(Message);
        }

        // Small private class to wrap
        // the User and the callback together.
        private class ClientWrap
        {
            private delCommsInfo _HostToClient = null;

            public ClientWrap(
                   delCommsInfo HostToClient)
            {
                _HostToClient = HostToClient;
            }

            public delCommsInfo HostToClient
            {
                get { return _HostToClient; }
            }
        }
    }
}



