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

        double timestep = 1e-5;
        double finaltime = 10;
        integrator.integrate(handler, finaltime, timestep);
    }
}

/// <summary>
/// this output handler's duty is to test the order of used method; it will only record position of the planet after exactly one period. 
/// </summary>
public class OutputHandlerForMethodOrderTesting : IOutputHandler {

    string outputFilename;
    public OutputHandlerForMethodOrderTesting(string outputFilename){
        this.outputFilename = outputFilename;
    }

    public int order{ set; get; }
    public Queue<Vector2> positions = new Queue<Vector2>();
    public Queue<double> stepSizes = new Queue<double>();

    public void addStepsizeDatapoint(double stepSize){
        stepSizes.Enqueue(stepSize);
    }
    public void addPositionDatapoint(Vector2 position){
        positions.Enqueue(position);
    }

    /// <summary>
    /// it is expected youll call this after multiple runs with different dts
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void write()
    {
        if (positions.Count != stepSizes.Count){
            throw new Exception("You have a diferent number of dts and positions recorded");
        }

        using (StreamWriter sw = new StreamWriter(outputFilename))
        {
            string toWrite = $"order {order}\ndt\tpositionX\tpositionY"; // header of the file; unitless (ew)
            sw.WriteLine(toWrite);
            while (stepSizes.Count > 0)
            {
                toWrite = stepSizes.Dequeue() + "\t" + positions.Dequeue().ToString();
                sw.WriteLine(toWrite);
            }
        }
    }

    public void addAngularMomentumDatapoint(double andgularMomentum){}//not used lol
    public void addEnergyDatapoint(double energy) { }// not used lol
    public void addTimeDatapoint(double time) { }// not used lol

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