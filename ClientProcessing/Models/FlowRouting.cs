using System;
using System.Threading;
using clientProcessing.Interfaces;
using NetMQ;
using NetMQ.Sockets;

namespace clientProcessing.Models
{
    class FlowRouting : IFlowRouting
    {
        private readonly IScreenHelper _screenHelper;
        private readonly IChannelRepository _channelRepository;
        private readonly ISubSocket _subSocket;

        public bool HasSubscribed { get; set; }
        public FlowRouting(IScreenHelper screenHelper, IChannelRepository channelRepository, ISubSocket subSocket)
        {
            _screenHelper = screenHelper;
            _channelRepository = channelRepository;
            _subSocket = subSocket;
            HasSubscribed = false;
        }

        public UserFlow GetRolling()
        {
            return StartFlow();
        }

        protected UserFlow StartFlow()
        {
            var flow = new UserFlow();
            var next = GetNext();

            if (next.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                flow.NextStep = "bye";
                return flow;
            }

            DoMainMenu(flow);
            flow = StartFlow();
            if (flow.NextStep == "bye") return flow;

            return flow;
        }

        private string GetNext()
        {
            _screenHelper.Clear();
            _screenHelper.Print("Welcome to Emerson Client Processing");
            _screenHelper.Print("Press <Enter> to continue or \"bye\" to quit ");
            var next = Console.ReadLine();
            return next;
        }

        public string NextStep(string nextStep)
        {
            var input = "";

            Console.WriteLine($"{nextStep} ");
            input = Console.ReadLine();
            return input;
        }

        private string DoMainMenu(UserFlow flow)
        {
            var input = "";
            while (input != "3")
            {
                _screenHelper.Clear();
                _screenHelper.Print("Would you like to do next? ");
                _screenHelper.Print("1 = Subscribe to Channels");
                _screenHelper.Print("2 = Listen for Messages");
                _screenHelper.Print("3 = Back");
                input = Console.ReadLine();


                if (input == "1")
                {
                    DoSubscribeChannels();
                }
                else if (input == "2")
                {
                    if (HasSubscribed)
                    {
                        DoListen();
                    }
                    else
                    {
                        HandleInvalidResponse("You have subscribed to any channels");
                    }
                }
            }

            return input;
        }

        private void DoListen()
        {
            var input = "";
            while (input != "Exit")
            {
                _screenHelper.Clear();
                _screenHelper.Print("Listen for incoming messages");
                _screenHelper.Print("'x' to exit");
                _screenHelper.Print("Press <Enter> to listen");
                var keepLoop = true;
                ConsoleKeyInfo cki;
                while (keepLoop)
                {
                    while (Console.KeyAvailable == false)
                        Thread.Sleep(250); // Loop until input is entered.

                    cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.X)
                    {
                        keepLoop = true;
                        input = "Exit";
                        break;
                    }

                    _subSocket.ReceiveTopic();
                    string messageReceived = _subSocket.ReceiveMessage();
                    Console.WriteLine(messageReceived);
                }
            }
        }

        private void DoSubscribeChannels()
        {
            var input = "";
            var channelCount = _channelRepository.GetChannels().Count;
            var loopTermination = channelCount > 0 ? "3" : "2";
            while (input != loopTermination)
            {
                _screenHelper.Clear();
                _screenHelper.Print("Would you like to do next? ");
                _screenHelper.Print("1 = List Channels");
                
                if (channelCount > 0)
                {
                    _screenHelper.Print("2 = Subscribe");
                    _screenHelper.Print("3 = Back");
                }
                else
                {
                    _screenHelper.Print("2 = Back");
                }
                input = Console.ReadLine();
                
                if (input == "1")
                {
                    DoListChannels();
                }
                else if (input == "2" && input != loopTermination)
                {
                    DoSubscribeChannel();
                }
            }
        }

        private void DoListChannels()
        {
            _screenHelper.Clear();
            _screenHelper.Print("Channel list");
            _screenHelper.Print("------------");
            var channels = _channelRepository.GetChannels();
            if (channels.Count > 0)
            {
                foreach (var channel in channels)
                {
                    _screenHelper.Print(channel.Name);
                }
            }
            else
            {
                _screenHelper.Print("No channels found");
                _screenHelper.Print("Press <Enter> to go back");
            }

            _screenHelper.Print("------------");
            _screenHelper.Print("");
            _screenHelper.Print("Press <Enter> to continue");
            _screenHelper.GetResponse();
        }

        private void DoSubscribeChannel()
        {
            _screenHelper.Clear();

            _screenHelper.Print("Channel list");
            _screenHelper.Print("------------");
            var channels = _channelRepository.GetChannels();
            if (channels.Count > 0)
            {
                foreach (var channel in channels)
                {
                    _screenHelper.Print(channel.Name);
                }
                _screenHelper.Print("All");
            }
            else
            {
                _screenHelper.Print("No channels found");
                _screenHelper.Print("Press <Enter> to go back");
            }

            _screenHelper.Print("------------");
            _screenHelper.Print("");
            _screenHelper.Print("Channel name? ");
            _screenHelper.Print("Enter channel name or <Enter> to go back");
            var name = Console.ReadLine();
            if (name != "")
            {
                name = name == "All" ? "" : name;
                _subSocket.Subscribe(name);
                HasSubscribed = true;
            }
        }

        public void HandleInvalidResponse(string message)
        {
            Console.WriteLine("---Try Again---");
            Console.WriteLine(message);
            Console.WriteLine("<Return to continue>");
            Console.ReadLine();
        }
    }
}
