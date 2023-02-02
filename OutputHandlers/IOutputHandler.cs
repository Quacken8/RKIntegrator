using System.Numerics;
namespace Simul1.OutputHandlers;

public interface IOutputHandler{

    public void addPositionDatapoint(Vector<double> position);
    public void addEnergyDatapoint(double energy);
    public void addAngularMomentumDatapoint(double angularMomentum);
    public void addTimeDatapoint(double t);
    public void write();
}