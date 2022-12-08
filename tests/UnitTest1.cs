namespace particle_tests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void TestMethodTOConsole()
    {
        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
        Integrator integrator = new Integrator(initialPosition, initialMomentum);
        MockupOutputHandler handler = new MockupOutputHandler();

        double timestep = 5e-2;
        double finaltime = 1;
        integrator.integrate(handler, finaltime, timestep);
    }

    [TestMethod]
    public void TestMethodTOFile()
    {
        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
        Integrator integrator = new Integrator(initialPosition, initialMomentum);

        string outputFile = "output.txt";
        FileWriter handler = new FileWriter(outputFile);

        double timestep = 5e-2;
        double finaltime = 1;
        integrator.integrate(handler, finaltime, timestep);
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