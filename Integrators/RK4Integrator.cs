using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.Integrators;

public class RK4Integrator : IIntegrator
{
    /// <summary>
    /// Returns function that steps forward using runge kutta method of order 4 for a function func that's independent of time
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

            Vector<double> k2 = lastPosition + 0.5 * stepSize * dxdt(l1);
            Vector<double> l2 = lastMomentum + 0.5 * stepSize * dpdt(k1);

            Vector<double> k3 = lastPosition + 0.5 * stepSize * dxdt(l2);
            Vector<double> l3 = lastMomentum + 0.5 * stepSize * dpdt(k2);

            Vector<double> k4 = lastPosition + stepSize * dxdt(l3);
            Vector<double> l4 = lastMomentum + stepSize * dpdt(k3);

            var newPosition = lastPosition + stepSize / 6 * (dxdt(l1) + 2 * dxdt(l2) + 2 * dxdt(l3) + dxdt(l4));

            var newMomentum = lastMomentum + stepSize / 6 * (dpdt(k1) + 2 * dpdt(k2) + 2 * dpdt(k3) + dpdt(k4));

            return (newPosition, newMomentum);
        };
    }
}