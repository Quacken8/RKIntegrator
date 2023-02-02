using System.Numerics;
using Simul1.ParticleSimulation;

namespace Simul1.OutputHandlers;

/// <summary>
/// Output handler class that writes the results to a file (which is given to it in the constructor) row by row separated by '\t's
/// </summary>
public class FileWriter : IOutputHandler {
    string outputFilename;
    public FileWriter(string outputFilename){
        this.outputFilename = outputFilename;
    }
    Queue<double> energies = new Queue<double>();
    Queue<double> angularMomenta = new Queue<double>();
    Queue<double> times = new Queue<double>();
    Queue<Vector<double>> positions = new ();

    public void addTimeDatapoint(double time){
        times.Enqueue(time);
    }
    public void addEnergyDatapoint(double energy){
        energies.Enqueue(energy);
    }
    public void addAngularMomentumDatapoint(double angularMomentum){
        angularMomenta.Enqueue(angularMomentum);
    }
    public void addPositionDatapoint(Vector<double> position){
        positions.Enqueue(position);
    }

    public void write(){
        using (StreamWriter sw = new StreamWriter(outputFilename)){
            string toWrite = "time\tenergy\tangMomentum\tpositionX\tpositionY"; // header of the file; unitless (ew)
            sw.WriteLine(toWrite);
            while(times.Count>0){
                // this string concat is eww; worth using stringbuilder??
                toWrite = times.Dequeue() + "\t" + energies.Dequeue() + "\t" + angularMomenta.Dequeue() + "\t" + positions.Dequeue().ToString();
                sw.WriteLine(toWrite);
            }
        }
    }
}