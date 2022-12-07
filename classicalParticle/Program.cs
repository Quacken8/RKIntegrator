using System.Text;
[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("particle_tests")]
internal class Program
{
    private static void Main(string[] args)
    {
        Integrator integrator = new Integrator();
        //integrator.integrate();
    }


}

public interface IOutputHandler{

    public void addPositionDatapoint(Vector2 position);
    public void addEnergyDatapoint(double energy);
    public void addAngularMomentumDatapoint(double angularMomentum);
    public void addTimeDatapoint(double t);
    public void write();

    // I/O stuffs
    //const string outfilename = "out.txt";    
    //const char delimiter = '\t';
    //const char linebreak = '\n';
    //StringBuilder stringbuilder = new StringBuilder();

    //stringbuilder.Append(t);
    //stringbuilder.Append(delimiter);
    ////evaluate angular momentum and energy, save it
    //stringbuilder.Append(earth.Energy);
    //stringbuilder.Append(delimiter);
    //stringbuilder.Append(earth.AngularMomentum);
    //stringbuilder.Append(delimiter);
    ////save position
    //stringbuilder.Append(earth.Position);
    //stringbuilder.Append(linebreak);
}


public class Integrator{
    public void integrate(IOutputHandler outputHandler){

    //const string outfilename = "out.txt";
    //StreamWriter outputHandler = new StreamWriter(outfilename);
    
        // tecnical simulation parameters (prolly gonna be replaced by user input if i feel like it)
        const double finalTime = 1e3; // at what time the simulation should end
        double t = 0;
        const double stepSize = 1e-3; // dt
        int totalNumOfSteps = (int)((finalTime - t) / stepSize); // gonna be useful???? prolly not, the best way would be to use a buffer that fits the processor memory
        AssortedFunctions af = new AssortedFunctions();

        // simulation initial parameters
        const double eccentricity = 0.3; // unitless eccentricity of an orbit
        Vector2 initialPosition = new Vector2(1 - eccentricity, 0);
        Vector2 initialMomentum = new Vector2(0, Math.Sqrt((1 + eccentricity) / (1 - eccentricity)));
        Particle earth = new Particle(initialPosition, initialMomentum);

        while (t < finalTime){

            outputHandler.addTimeDatapoint(t);
            outputHandler.addEnergyDatapoint(earth.Energy);
            outputHandler.addAngularMomentumDatapoint(earth.AngularMomentum);
            outputHandler.addPositionDatapoint(earth.Position);

            //step forward using rk2/4

            Vector2 newposition = af.RK2Step(stepSize, earth.Position, af.dxdt);
            Vector2 newmomentum = af.RK2Step(stepSize, earth.Momentum, af.dpdt);

            earth.Position = newposition;
            earth.Momentum = newmomentum;

            t += stepSize;
        }

        outputHandler.write();

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
        return $"{X}, {Y}"; // TBD this might be super slow ew
    }
}

public class Particle{

    /// <summary>
    /// sets up the particle with initial position and momentum
    /// </summary>
    /// <param name="initialPos">Position at t = 0</param>
    /// <param name="initialMom">momentum at t = 0</param>

    AssortedFunctions af = new AssortedFunctions();
    public Particle(Vector2 initialPos, Vector2 initialMom){
        Position = initialPos;
        Momentum = initialMom;
    }
    public Vector2 Position { set; get; }
    public Vector2 Momentum { set; get; }
    public double Energy { get => af.TotalEnergy(Position, Momentum); }
    public double AngularMomentum { get => af.AngularMomentum(Position, Momentum); }
}


public class AssortedFunctions{
    public Vector2 dpdt(Vector2 position){
        double pe = PotentialEnergy(position);
        return -pe*pe*pe*position;
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
        double x = position.X;
        double y = position.Y;
        return -1.0 / Math.Sqrt(x * x + y * y);
    }

    /// <summary>
    /// returns classical momentum of point particle
    /// </summary>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public double KineticEnergy(Vector2 momentum){
        double px = momentum.X;
        double py = momentum.Y;
        return 0.5*(px*px+py*py);
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
            /// <summary>
        /// Calculates the next, (n+1)th value of integrated function func using runge kutta method of order 2 for func that's independent of time
        /// </summary>
        /// <param name="stepSize">desired time step</param>
        /// <param name="lastResult">nth value</param>
        /// <param name="func">function that we're trying to integrate</param>
        /// <returns></returns>
        public Vector2 RK2Step(double stepSize, Vector2 lastResult, Func<Vector2, Vector2> func)
        {

            Vector2 k1 = func(lastResult);
            Vector2 k2 = func(lastResult + k1);
            return lastResult + stepSize * 0.5 * (k1 + k2);
        }

        /// <summary>
        /// Steps forward using runge kutta method of order 2 for a function func that's independent of time
        /// </summary>
        /// <param name="stepSize">desired time step</param>
        /// <param name="lastResult">nth value</param>
        /// <param name="func">function that we're trying to integrate</param>
        /// <returns></returns>
        public Vector2 RK4Step(double stepSize, Vector2 lastResult, Func<Vector2, Vector2> func)
        {

            Vector2 k1 = func(lastResult);
            Vector2 k2 = func(lastResult + 0.5 * k1);
            Vector2 k3 = func(lastResult + 0.5 * k2);
            Vector2 k4 = func(lastResult + k3);
            return lastResult + stepSize / 6 * (k1 + 2 * k2 + 2 * k3 + k4);
        }
}