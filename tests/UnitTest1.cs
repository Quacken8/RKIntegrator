namespace particle_tests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void TestMethod1()
    {

        Integrator integrator = new Integrator();
        MockupOutputHandler handler = new MockupOutputHandler();
        integrator.integrate(handler, 1, 1e-1);
    }
}

public class MockupOutputHandler : IOutputHandler{
    
    public List<Vector2> positions = new List<Vector2>();
    public List<double> energies = new List<double>();
    public List<double> angularMomenta = new List<double>();
    public List<double> times = new List<double>();
    public void addPositionDatapoint(Vector2 position){
        positions.Add(position);
    }
    public void addEnergyDatapoint(double energy){
        energies.Add(energy);
    }
    public void addAngularMomentumDatapoint(double andgularMomentum){
        angularMomenta.Add(andgularMomentum);
    }
    public void addTimeDatapoint(double time){
        times.Add(time);
    }
    public void write(){
        foreach (double energy in energies){
            Console.WriteLine(energy);
        }

    }
}