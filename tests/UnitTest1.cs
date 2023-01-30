namespace particle_tests;


[TestClass]
public class UnitTest1
{

    double wholePeriod = 8.0 * Math.Atan(1.0);

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

        double timestep = 1e-4;
        double finaltime = wholePeriod+2*timestep;
        integrator.integrate(handler, finaltime, timestep, method: "euler");
    }

    [TestMethod]
    public void TestStepsizeDependency(){ // not advised to use becuase for some reason vscode's unit tests dont show console output until the end of the test so you dont see what part of the simulation you are at and since these take a lot of time it's rather impractical; use the TestStepsizeDependency() function in main instead

        double[] stepSizes = new double[] { 1e-1, 5e-2, 1e-2, 5e-3, 1e-3, 5e-4, 1e-4, 5e-5, 1e-5, 1e-6, 1e-7, 1e-8, 1e-9 };
        string[] methods = new string[] { "Euler", "RK2", "RK4" };
        const string filename = "stepsizeTestOutput.txt";

        OutputHandlerForMethodOrderTesting stepsizeHandler = new OutputHandlerForMethodOrderTesting(filename);

        foreach (string method in methods)
        {
            stepsizeHandler.addMethodDatapoint(method);
            Console.WriteLine($"Currently trying the {method} method");
            foreach (double stepSize in stepSizes)
            {
                Console.WriteLine($"Currently running stepsize {stepSize}");
                stepsizeHandler.addStepsizeDatapoint(stepSize);
                // setup new simnulation
                double wholePeriod = 8.0 * Math.Atan(1.0);
                const double eccentricity = 0.3; // unitless eccentricity of an orbit
                Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
                Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
                Integrator integrator = new Integrator(initialPosition, initialMomentum);

                integrator.integrate(stepsizeHandler, wholePeriod + 5 * stepSize, stepSize, write: false, method: method);
            }
        }

        stepsizeHandler.write();
    }
}

/// <summary>
/// This handler's only purpose is to write down position after one period
/// </summary>
public class OutputHandlerForMethodOrderTesting : IOutputHandler
{
    string outputFilename;
    public OutputHandlerForMethodOrderTesting(string outputFilename)
    {
        this.outputFilename = outputFilename;
        outputString += "stepsize\tposX\tposY\n";
    }

    public double period { set; get; }
    bool afterAPeriod = false;
    public Queue<Vector2> positions = new Queue<Vector2>();
    public Queue<double> stepSizes = new Queue<double>();
    public List<string> methods = new List<string>();

    public string outputString { set; get; } // dumb way of doing this in general but nobody expects more than ~20 datapoints anyway

    /// <summary>
    /// this is expected to run only once per method
    /// </summary>
    /// <param name="method"></param>
    public void addMethodDatapoint(string method)
    {
        methods.Add(method);
        outputString += method + "\n";
    }

    /// <summary>
    /// is expected to be run before position gets recorded
    /// </summary>
    /// <param name="stepSize"></param>
    public void addStepsizeDatapoint(double stepSize)
    {
        stepSizes.Enqueue(stepSize);
        outputString += stepSize + "\t";
        afterAPeriod = false;
    }

    Vector2 lastPosition = new Vector2(0, 0);
    /// <summary>
    /// is expected to run every timestep during the integration, but will only record once per simulation and after the period
    /// </summary>
    /// <param name="position"></param>
    public void addPositionDatapoint(Vector2 position)
    {
        if (afterAPeriod && positions.Count < stepSizes.Count)
        {
            positions.Enqueue(lastPosition);

            afterAPeriod = false;
            outputString += position.X + "\t" + position.Y + "\n";
        }
        lastPosition = position;
    }
    public void addTimeDatapoint(double time)
    {
        if (time > period)
        {
            afterAPeriod = true;
        }
    }

    /// <summary>
    /// it is expected youll call this after multiple runs with different dts
    /// </summary>
    public void write()
    {
        using (StreamWriter sw = new StreamWriter(outputFilename))
        {
            sw.Write(outputString);
        }
    }


    public void addAngularMomentumDatapoint(double andgularMomentum) { }//not used lol
    public void addEnergyDatapoint(double energy) { }// not used lol
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