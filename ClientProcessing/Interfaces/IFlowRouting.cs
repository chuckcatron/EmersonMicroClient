using clientProcessing.Models;
using NetMQ.Sockets;

namespace clientProcessing.Interfaces
{
    public interface IFlowRouting
    {
        string NextStep(string nextStep);
        UserFlow GetRolling();
    }
}