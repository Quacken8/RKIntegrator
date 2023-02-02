using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.Integrators;

public class RK2Integrator : IIntegrator
{
    /// <summary>
    /// Returns function that calculates the next, (n+1)th value of integrated function func using runge kutta method of order 2 for func that's independent of time
    /// </summary>
    /// <param name="stepSize">desired time step</param>
    /// <param name="lastResult">nth value</param>
    /// <param name="func">function that we're trying to integrate</param>
    /// <returns></returns>
    public Func<(Vector<double> position, Vector<double> momentum), (Vector<double> position, Vector<double> momentum)> 
        GetNextStepFunction(double stepSize, Func<Vector<double>, Vector<double>> dxdt, Func<Vector<double>, Vector<double>> dpdt)
    {
        return x =>
        {
            var lastMomentum = x.momentum;
            var lastPosition = x.position;

            Vector<double> k1 = lastPosition;
            Vector<double> l1 = lastMomentum;

            Vector<double> k2 = lastPosition + stepSize * dxdt(l1);
            Vector<double> l2 = lastMomentum + stepSize * dpdt(k1);

            var newPosition = lastPosition + stepSize * 0.5 * (dxdt(l1) + dxdt(l2));
            var newMomentum = lastMomentum + stepSize * 0.5 * (dpdt(k1) + dpdt(k2));

            return (newPosition, newMomentum);
        };
    }
}