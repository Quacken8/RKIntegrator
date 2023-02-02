using System.Numerics;
using Simul1.Integrators;
using Simul1.OutputHandlers;

namespace Simul1.ParticleSimulation;

public class Simulation {
    private readonly Vector<double> _initialPosition;
    private readonly Vector<double> _initialMomentum;
    public Simulation(Vector<double> initialPosition, Vector<double> initialMomentum){
        _initialPosition = initialPosition;
        _initialMomentum = initialMomentum;
    }

    /// <summary>
    /// simulate the whole movement
    /// </summary>
    /// <param name="outputHandler">class that can remember energies, angular momenta, times and positions of the particle at each step</param>
    /// <param name="finalTime">desired final time of the simulation</param>
    /// <param name="timeStep">timestep of the simulation</param>
    /// <param name="method">method with which to integrate. Viable options are "euler", "RK2" and "RK4" (not case sensitive)</param>
    /// <exception cref="ArgumentException">thrown if you dont choose viable method for integration</exception>
    public string Integrate(IOutputHandler outputHandler, double finalTime, double timeStep, EIntegrationMethod method = EIntegrationMethod.RK2, bool write = true)
    {

        double t = 0;
        // const double timeStep = 1e-3; // dt
        // int totalNumOfSteps = (int)((finalTime - t) / timeStep) + 1; // could be later added into the output handler so it can create the correct sized array for results without having to constantly update List<> lengths; TBD tho, first need to actually find the slow places of this program and it probably won't matter in the end
        // also for a long enough simulation the double result of that division will lose accuracy so this number isnt rly good anyway; should do a bit of bit magic to it first

        var earth = new Particle(_initialPosition, _initialMomentum);
        var initialEnergy = AssortedFunctions.TotalEnergy(_initialPosition, _initialMomentum);
        var integrationStepFn = method.GetIntegrator().GetNextStepFunction(timeStep, AssortedFunctions.dxdt, AssortedFunctions.dpdt);
        
        while (t < finalTime)
        {
            outputHandler.addTimeDatapoint(t);
            outputHandler.addEnergyDatapoint(earth.Energy);
            outputHandler.addAngularMomentumDatapoint(earth.AngularMomentum);
            outputHandler.addPositionDatapoint(earth.Position);
            var oldV = (earth.Position, earth.Momentum);
            var newV = integrationStepFn(oldV);
            earth.Position = newV.position;
            earth.Momentum = newV.momentum;

            t += timeStep;
        }
        if (write){
            outputHandler.write();
        }

        return $"{t}\t{timeStep}\t{earth.Position[0]}\t{earth.Position[1]}\t{Math.Abs(AssortedFunctions.TotalEnergy(earth.Position, earth.Momentum)-initialEnergy)}";
    }
}