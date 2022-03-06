using System;
using clientProcessing.Interfaces;
using clientProcessing.Models;
using Microsoft.Extensions.Logging;

namespace clientProcessing
{
    public class ClientProcessing
    {
        private readonly ILogger<ClientProcessing> _logger;
        private readonly IScreenHelper _screenHelper;
        private readonly IFlowRouting _flowRouting;
        private readonly ISubSocket _subSocket;

        public ClientProcessing(ILogger<ClientProcessing> logger, IScreenHelper screenHelper, IFlowRouting flowRouting, ISubSocket subSocket)
        {
            _logger = logger;
            _screenHelper = screenHelper;
            _flowRouting = flowRouting;
            _subSocket = subSocket;
        }
        internal void Run()
        {
            _logger.LogInformation("Application Started at {dateTime}", DateTime.UtcNow);
            _screenHelper.Clear();

            var flow = new UserFlow();

            while (flow.NextStep != "bye")
            {
                _subSocket.OpenConnection();
                
                flow = _flowRouting.GetRolling();
            }


            _logger.LogInformation("Application Ended at {dateTime}", DateTime.UtcNow);
            _subSocket.CloseConnection();
        }
    }
}