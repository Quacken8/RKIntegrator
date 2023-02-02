using System.Numerics;

namespace Simul1.ParticleSimulation;

public class Particle {
    /// <summary>
    /// sets up the particle with initial position and momentum
    /// </summary>
    /// <param name="initialPos">Position at t = 0</param>
    /// <param name="initialMom">momentum at t = 0</param>
    public Particle(Vector<double> initialPos, Vector<double> initialMom){
        Position = initialPos;
        Momentum = initialMom;
    }
    public Vector<double> Position { set; get; }
    public Vector<double> Momentum { set; get; }
    public double Energy { get => AssortedFunctions.TotalEnergy(Position, Momentum); }
    public double AngularMomentum { get => AssortedFunctions.AngularMomentum(Position, Momentum); }
}