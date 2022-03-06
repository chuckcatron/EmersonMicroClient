using System;
using clientProcessing.Models;
using NetMQ.Sockets;

namespace clientProcessing.Interfaces
{
    public interface ISubSocket
    {
        SubscriberSocket OpenConnection();
        void CloseConnection();
        void Subscribe(string channel);
        string ReceiveTopic();
        string ReceiveMessage();
    }
}