using System.Numerics;

namespace Simul1.ParticleSimulation;

public static class AssortedFunctions
{
    public static Vector<double> dpdt(Vector<double> position){
        double pe = PotentialEnergy(position);
        return pe*pe*pe*position;
    }

    public static Vector<double> dxdt(Vector<double> momentum){
        return momentum;
    }

    /// <summary>
    /// returns potential of a point particle in -1/r well
    /// </summary>
    /// <param name="position">vector of position of said particle in units </param>
    /// <returns></returns>
    public static double PotentialEnergy(Vector<double> position){
        return -1.0 / Math.Sqrt(position.Norm());
    }

    /// <summary>
    /// returns classical momentum of point particle
    /// </summary>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public static double KineticEnergy(Vector<double> momentum){
        return 0.5*momentum.Norm();
    }
    /// <summary>
    /// returns total energy of a particle 
    /// </summary>
    /// <param name="position">vector of position of said particle in units</param>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public static double TotalEnergy(Vector<double> position, Vector<double> momentum){
        return PotentialEnergy(position) + KineticEnergy(momentum);
    }

    /// <summary>
    /// returns angular momentum of a point particle with respect to center at (0,0)
    /// </summary>
    /// <param name="position">vector of position of said particle in units</param>
    /// <param name="momentum">vector of momentum of said particle in units</param>
    /// <returns></returns>
    public static double AngularMomentum(Vector<double> position, Vector<double> momentum){
        double x = position[0];
        double y = position[1];
        double px = momentum[0];
        double py = momentum[1];
        return x * py - y * px;
    }
}