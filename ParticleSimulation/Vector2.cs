using System.Numerics;

namespace Simul1.ParticleSimulation;

public static class VectorExtensions
{
    public static double Norm(this Vector<double> v)
    {
        return Vector.Dot(v, v);
    }
}