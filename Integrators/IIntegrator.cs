using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.Integrators;

public interface IIntegrator
{
    public Func<(Vector<double> position, Vector<double> momentum), (Vector<double> position, Vector<double> momentum)> 
        GetNextStepFunction(double stepSize, Func<Vector<double>, Vector<double>> dxdt, Func<Vector<double>, Vector<double>> dpdt);
}