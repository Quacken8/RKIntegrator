using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.OutputHandlers;


/// <summary>
/// This handler's only purpose is to write down position after one period
/// </summary>
public class MethodOrderTestingOutputHandler : IOutputHandler
{
    string outputFilename;
    public MethodOrderTestingOutputHandler(string outputFilename)
    {
        this.outputFilename = outputFilename;
        outputString += "stepsize\tposX\tposY\n";
    }

    public double period { set; get; }
    bool afterAPeriod = false;
    public Queue<Vector<double>> positions = new Queue<Vector<double>>();
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

    Vector<double> lastPosition = new (new [] {0d, 0d});
    /// <summary>
    /// is expected to run every timestep during the integration, but will only record once per simulation and after the period
    /// </summary>
    /// <param name="position"></param>
    public void addPositionDatapoint(Vector<double> position)
    {
        if (afterAPeriod && positions.Count < stepSizes.Count)
        {
            positions.Enqueue(lastPosition);

            afterAPeriod = false;
            outputString += position[0] + "\t" + position[1] + "\n";
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