namespace particle_tests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void TestMethod1()
    {
        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
        Integrator integrator = new Integrator(initialPosition, initialMomentum);
        MockupOutputHandler handler = new MockupOutputHandler();
        integrator.integrate(handler, 1, 1e-2);
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
        foreach (Vector2 position in positions){
            Console.WriteLine(position);
        }
    }
}