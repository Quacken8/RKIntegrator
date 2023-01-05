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

        double timestep = 1e-7;
        double finaltime = wholePeriod;
        integrator.integrate(handler, finaltime, timestep, method: "Euler");
    }

    [TestMethod]
    public void TestMethodOrderRelationRK2(){

        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));

        string filename = "ordersOutputRK2.txt";
        OutputHandlerForMethodOrderTesting handler = new OutputHandlerForMethodOrderTesting(filename); // saves the final positions of the planet
        handler.period = wholePeriod;

        // create an array of smaller and smaller timesteps that still end up at exactly one period
        const int numberOfDatapoints = 1000;
        double[] stepSizes = new double[numberOfDatapoints];
        for (int i = 0; i < numberOfDatapoints; i++){
            stepSizes[i] = wholePeriod / Math.Pow(2, i+2);
        }
        Console.WriteLine("vscode is dumb and wont tell u but im running brm brm");

        foreach (double stepSize in stepSizes){
            handler.addStepsizeDatapoint(stepSize);
            Integrator integrator = new Integrator(initialPosition, initialMomentum);

            double wholePeriodAndABit = wholePeriod + 1.5 * stepSize; // should make sure theres plenty of space to hit period + stepsize + epsilon so the handlers write condition hit, but also not too long to write down data from whole period + stepsize

            integrator.integrate(handler, wholePeriodAndABit, stepSize, method: "RK2", write: false);
        }

        handler.write();
    }
}

/// <summary>
/// this output handler's duty is to test the order of used method; it will only record position of the planet after exactly one period. It is expected you wont use this if your simulation takes longer than one period
/// </summary>
public class OutputHandlerForMethodOrderTesting : IOutputHandler {
    string outputFilename;
    public OutputHandlerForMethodOrderTesting(string outputFilename){
        this.outputFilename = outputFilename;
    }

    public int order{ set; get; }
    public double period { set; get; }
    bool afterAPeriod = false;
    public Queue<Vector2> positions = new Queue<Vector2>();
    public Queue<double> stepSizes = new Queue<double>();

    public void addStepsizeDatapoint(double stepSize){
        stepSizes.Enqueue(stepSize);
    }

    Vector2 lastPosition;
    public void addPositionDatapoint(Vector2 position){
        if (afterAPeriod && positions.Count < stepSizes.Count){
            positions.Enqueue(lastPosition);
            afterAPeriod = false;
        }
        lastPosition = position;
    }
    public void addTimeDatapoint(double time){
        if (time > period){
            afterAPeriod = true;
        }
    }

    /// <summary>
    /// it is expected youll call this after multiple runs with different dts
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void write(){
        if (positions.Count != stepSizes.Count){
            throw new Exception("You have a diferent number of dts and positions recorded");
        }

        using (StreamWriter sw = new StreamWriter(outputFilename)){
            string toWrite = $"order {order}\ndt\tpositionX\tpositionY"; // header of the file; unitless (ew)
            sw.WriteLine(toWrite);
            while (stepSizes.Count > 0){
                toWrite = stepSizes.Dequeue() + "\t" + positions.Dequeue().ToString();
                sw.WriteLine(toWrite);
            }
        }
    }

    public void addAngularMomentumDatapoint(double andgularMomentum){}//not used lol
    public void addEnergyDatapoint(double energy){}// not used lol
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