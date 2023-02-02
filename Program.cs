using System.Numerics;
using System.Text;
using Simul1.Integrators;
using Simul1.OutputHandlers;
using Simul1.ParticleSimulation;

namespace Simul1;

internal class Program
{
    /// <summary>
    /// only for this one test. I do it here becuase unit tests dont give real time output which is dumb but I need it 
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        TestStepsizeDependency("stepsizeTestOutput.txt");
    }

    public static void TestStepsizeDependency(string outputFilename)
    {

        var wholePeriod = 8.0 * Math.Atan(1.0);
        var stepSizes = new []{ 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6, 1e-7, 1e-8 };
        for (int i = 0; i < stepSizes.Count(); i++){
            stepSizes[i] *= wholePeriod;
        }

        var methods = new []{ EIntegrationMethod.EULER, EIntegrationMethod.RK2, EIntegrationMethod.RK4 };

        var handler = new NoActionOutputHandler();
        var outputString = new StringBuilder("");
        outputString.AppendLine("\nFinal Time\tstepsize\tx\ty\tEnergy error");
        foreach (var method in methods)
        {
            outputString.AppendLine("\n" + method);
            Console.WriteLine($"Currently trying the {method} method");
            foreach (var stepSize in stepSizes)
            {
                Console.WriteLine($"Currently running stepsize {stepSize}");
                // setup new simnulation
                const double eccentricity = 0.3; // unitless eccentricity of an orbit
                var initialPosition = new Vector<double>(new [] {1 - eccentricity, 0, 0, 0}); // four dimensions cuz .NET 6.0 is kinda weird
                var initialMomentum = new Vector<double>(new [] {0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)), 0, 0});
                
                var simulation = new Simulation(initialPosition, initialMomentum);

                var simulationResult = simulation.Integrate(handler, wholePeriod, stepSize, write: false,
                    method: method);
                    
                outputString.AppendLine(simulationResult);
            }
        }

        using (StreamWriter sw = new StreamWriter(outputFilename)){
            sw.Write(outputString);
        }
    }
}