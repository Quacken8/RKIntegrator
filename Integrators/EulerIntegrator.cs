using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.Integrators;

public class EulerIntegrator : IIntegrator
{
    /// <summary>
    /// Returns function that steps forward using euler method of first order
    /// </summary>
    /// <param name="stepSize">dt</param>
    /// <param name="dxdt">function to update position</param>
    /// <param name="dpdt">function to update momentum</param>
    public Func<(Vector<double> position, Vector<double> momentum), (Vector<double> position, Vector<double> momentum)> 
        GetNextStepFunction(double stepSize, Func<Vector<double>, Vector<double>> dxdt, Func<Vector<double>, Vector<double>> dpdt)
    {
        return x =>
        {
            var lastMomentum = x.momentum;
            var lastPosition = x.position;

            var newPosition = lastPosition + stepSize * dxdt(lastMomentum);
            var newMomentum = lastMomentum + stepSize * dpdt(lastPosition);

            return (newPosition, newMomentum);
        };
    }
}