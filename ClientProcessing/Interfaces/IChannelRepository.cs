using clientProcessing.Models;
using System.Collections.Generic;
using clientProcessing.Models;

namespace clientProcessing.Interfaces
{
    public interface IChannelRepository
    {
        List<Channel> GetChannels();
        bool AddChannel(string name);
        void DeleteChannel(string name);
    }
}