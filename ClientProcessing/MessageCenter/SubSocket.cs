using System;
using clientProcessing.Interfaces;
using clientProcessing.Models;
using NetMQ;
using NetMQ.Sockets;

namespace clientProcessing.MessageCenter
{
    public class SubSocket: ISubSocket
    {
        private SubscriberSocket _subscriberSocket;
        public SubSocket( ){}
        public SubscriberSocket OpenConnection()
        {
            _subscriberSocket = new SubscriberSocket();
            _subscriberSocket.Options.ReceiveHighWatermark = 1000;
            _subscriberSocket.Connect("tcp://127.0.0.1:3000");
            return _subscriberSocket;
        }

        public void CloseConnection()
        {
            _subscriberSocket.Close();
        }

        public void Subscribe(string channel)
        {
            _subscriberSocket.Subscribe(channel);
        }

        public string ReceiveTopic()
        {
            return _subscriberSocket.ReceiveFrameString();
        }

        public string ReceiveMessage()
        {
            return _subscriberSocket.ReceiveFrameString();
        }
    }
}