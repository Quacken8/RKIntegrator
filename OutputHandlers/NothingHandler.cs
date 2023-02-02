using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.OutputHandlers;

public class NoActionOutputHandler : IOutputHandler{
    public void write() { }
    public void addAngularMomentumDatapoint(double x) {}
    public void addEnergyDatapoint(double x) {}
    public void addPositionDatapoint(Vector<double> x) {}
    public void addTimeDatapoint(double x) {}
}