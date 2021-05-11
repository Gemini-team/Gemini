using System.Threading.Tasks;
using Grpc.Core;
using GeminiOSPInterface;

namespace Gemini.Networking.Services {
    
    public interface ISimulationService
    {
        StepResponse DoStep(StepRequest request);

        SetStartTimeResponse SetStartTime(SetStartTimeRequest request);

    }

    public class SimulationServiceImpl : Simulation.SimulationBase
    {
        delegate T Del<T, V>(V request);

        private SimulationController _simulationController;
        public SimulationServiceImpl(SimulationController simulationController)
        {
            _simulationController = simulationController;
        }
    
        public override async Task<StepResponse> DoStep(
            StepRequest request, ServerCallContext context)
        {

            _simulationController.SignalEvent.WaitOne();
            return await Task.FromResult(Executor.Execute<StepResponse, Del<StepResponse, StepRequest>, StepRequest>(_simulationController.DoStep, request));
        }

        public override async Task<SetStartTimeResponse> SetStartTime(
            SetStartTimeRequest request, ServerCallContext context)
        {
            return await Task.FromResult(Executor.Execute<SetStartTimeResponse, Del<SetStartTimeResponse, SetStartTimeRequest>, SetStartTimeRequest>(_simulationController.SetStartTime, request));
        }
    }

}
