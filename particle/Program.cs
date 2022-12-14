using System.Text;
[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("particle_tests")]

internal class Program
{
    private static void Main(string[] args)
    {
        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
        Integrator integrator = new Integrator(initialPosition, initialMomentum);
        //integrator.integrate();
    }


}

public interface IOutputHandler{

    public void addPositionDatapoint(Vector2 position);
    public void addEnergyDatapoint(double energy);
    public void addAngularMomentumDatapoint(double angularMomentum);
    public void addTimeDatapoint(double t);
    public void write();
}


public class Integrator{
    Vector2 initialPosition;
    Vector2 initialMomentum;
    public Integrator(Vector2 initialPosition, Vector2 initialMomentum){
        this.initialPosition = initialPosition;
        this.initialMomentum = initialMomentum;
    }

    /// <summary>
    /// simulate the whole movement
    /// </summary>
    /// <param name="outputHandler">class that can remember energies, angular momenta, times and positions of the particle at each step</param>
    /// <param name="finalTime">desired final time of the simulation</param>
    /// <param name="timeStep">timestep of the simulation</param>
    /// <param name="method">method with which to integrate. Viable options are "euler", "RK2" and "RK4" (not case sensitive)</param>
    /// <exception cref="ArgumentException">thrown if you dont choose viable method for integration</exception>
    public void integrate(IOutputHandler outputHandler, double finalTime, double timeStep, string method = "RK2", bool write = true){

        AssortedFunctions af = new AssortedFunctions();

        double t = 0;
        // const double timeStep = 1e-3; // dt
        int totalNumOfSteps = (int)((finalTime - t) / timeStep) + 1; // could be later added into the output handler so it can create the correct sized array for results without having to constantly update List<> lengths; TBD tho, first need to actually find the slow places of this program and it probably won't matter in the end
        // also for a long enough simulation the double result of that division will lose accuracy so this number isnt rly good anyway; should do a bit of bit magic to it first

        Particle earth = new Particle(initialPosition, initialMomentum);

        while (t < finalTime){

            outputHandler.addTimeDatapoint(t);
            outputHandler.addEnergyDatapoint(earth.Energy);
            outputHandler.addAngularMomentumDatapoint(earth.AngularMomentum);
            outputHandler.addPositionDatapoint(earth.Position);

            //step forward using chosen method
            switch(method.ToUpper()){
                case ("RK2"): 
                    earth.updateRK2(timeStep, af.dxdt, af.dpdt);
                    break;
                case ("RK4"):
                    earth.updateRK4(timeStep, af.dxdt, af.dpdt);
                    break;
                case ("EULER"):
                    earth.updateEuler(timeStep, af.dxdt, af.dpdt);
                    break;
                default:
                    throw new ArgumentException($"Method {method} not found! Only acceptable methods are 'RK2', 'RK4' and 'Euler'.");
            }

            t += timeStep;
        }

        if (write){
            outputHandler.write();
        }
    }
}

public struct Vector2{
    public Vector2(double x, double y){
        X = x;
        Y = y;
    }
    public double X { set; get; }
    public double Y { set; get; }
    /// <summary>
    /// the quadrate of magnitude
    /// </summary>
    public double Norm { get => X * X + Y * Y; }
    public static Vector2 operator *(double c, Vector2 v) => new Vector2(v.X * c, v.Y * c);
    public static Vector2 operator *(int c, Vector2 v) => new Vector2(v.X * c, v.Y * c);
    public static Vector2 operator +(Vector2 v, Vector2 w) => new Vector2(v.X + w.X, v.Y + w.Y);
    public override string ToString(){
        return $"{X}\t{Y}"; // TBD this might be super slow ew
    }
}

public class Particle{
    AssortedFunctions af = new AssortedFunctions();

    /// <summary>
    /// sets up the particle with initial position and momentum
    /// </summary>
    /// <param name="initialPos">Position at t = 0</param>
    /// <param name="initialMom">momentum at t = 0</param>
    public Particle(Vector2 initialPos, Vector2 initialMom){
        Position = initialPos;
        Momentum = initialMom;
    }
    public Vector2 Position { set; get; }
    public Vector2 Momentum { set; get; }
    public double Energy { get => af.TotalEnergy(Position, Momentum); }
    public double AngularMomentum { get => af.AngularMomentum(Position, Momentum); }

    /// <summary>
    /// Calculates the next, (n+1)th value of integrated function func using runge kutta method of order 2 for func that's independent of time
    /// </summary>
    /// <param name="stepSize">desired time step</param>
    /// <param name="lastResult">nth value</param>
    /// <param name="func">function that we're trying to integrate</param>
    /// <returns></returns>
    public void updateRK2(double stepSize, Func<Vector2, Vector2> dxdt, Func<Vector2, Vector2> dpdt){

        Vector2 lastMomentum = this.Momentum;
        Vector2 k1 = dxdt(lastMomentum);
        Vector2 k2 = dxdt(lastMomentum + k1);

        this.Position = this.Position + stepSize * 0.5 * (k1 + k2);

        Vector2 lastPosition = this.Position;
        Vector2 l1 = dpdt(lastPosition);
        Vector2 l2 = dpdt(lastPosition + l1);

        this.Momentum = this.Momentum + stepSize * 0.5 * (l1 + l2);
    }

    /// <summary>
    /// Steps forward using runge kutta method of order 2 for a function func that's independent of time
    /// </summary>
    /// <param name="stepSize">desired time step</param>
    /// <param name="lastResult">nth value</param>
    /// <param name="func">function that we're trying to integrate</param>
    /// <returns></returns>
    public void updateRK4(double stepSize, Func<Vector2, Vector2> dxdt, Func<Vector2, Vector2> dpdt){

        Vector2 lastMomentum = this.Momentum;
        Vector2 k1 = dxdt(lastMomentum);
        Vector2 k2 = dxdt(lastMomentum + 0.5 * k1);
        Vector2 k3 = dxdt(lastMomentum + 0.5 * k2);
        Vector2 k4 = dxdt(lastMomentum + k3);

        this.Position = this.Position + stepSize / 6 * (k1 + 2 * k2 + 2 * k3 + k4);


        Vector2 lastPosition = this.Position;
        Vector2 l1 = dpdt(lastPosition);
        Vector2 l2 = dpdt(lastPosition + 0.5 * l1);
        Vector2 l3 = dpdt(lastPosition + 0.5 * l2);
        Vector2 l4 = dpdt(lastPosition + l3);

        this.Momentum = this.Momentum + stepSize / 6 * (l1 + 2 * l2 + 2 * l3 + l4);
    }

    /// <summary>
    /// step forward using euler method of first order
    /// </summary>
    /// <param name="stepSize">dt</param>
    /// <param name="dxdt">function to update position</param>
    /// <param name="dpdt">function to update momentum</param>
    public void updateEuler(double stepSize, Func<Vector2, Vector2> dxdt, Func<Vector2, Vector2> dpdt){
        Vector2 lastMomentum = this.Momentum;
        this.Position = this.Position + stepSize*dxdt(lastMomentum);

        Vector2 lastPosition = this.Position;
        this.Momentum = this.Momentum + stepSize*dpdt(lastPosition);
    }
}


public class AssortedFunctions{
    public Vector2 dpdt(Vector2 position){
        double pe = PotentialEnergy(position);
        return +pe*pe*pe*position;
    }

    public Vector2 dxdt(Vector2 momentum){
        return momentum;
    }

    /// <summary>
    /// returns potential of a point particle in -1/r well
    /// </summary>
    /// <param name="position">vector of position of said particle in units </param>
    /// <returns></returns>
    public double PotentialEnergy(Vector2 position){
        return -1.0 / Math.Sqrt(position.Norm);
    }

    /// <summary>
    /// returns classical momentum of point particle
    /// </summary>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public double KineticEnergy(Vector2 momentum){
        return 0.5*momentum.Norm;
    }
    /// <summary>
    /// returns total energy of a particle 
    /// </summary>
    /// <param name="position">vector of position of said particle in units</param>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public double TotalEnergy(Vector2 position, Vector2 momentum){
        return PotentialEnergy(position) + KineticEnergy(momentum);
    }

    /// <summary>
    /// returns angular momentum of a point particle with respect to center at (0,0)
    /// </summary>
    /// <param name="position">vector of position of said particle in units</param>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public double AngularMomentum(Vector2 position, Vector2 momentum){
        double x = position.X;
        double y = position.Y;
        double px = momentum.X;
        double py = momentum.Y;
        return x * py - y * px;
    }
}

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
    Queue<Vector2> positions = new Queue<Vector2>();

    public void addTimeDatapoint(double time){
        times.Enqueue(time);
    }
    public void addEnergyDatapoint(double energy){
        energies.Enqueue(energy);
    }
    public void addAngularMomentumDatapoint(double angularMomentum){
        angularMomenta.Enqueue(angularMomentum);
    }
    public void addPositionDatapoint(Vector2 position){
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