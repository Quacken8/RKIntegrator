namespace Simul1.Integrators;

public enum EIntegrationMethod
{
    RK4,
    RK2,
    EULER
}

public static class EIntegrationMethodExtensions
{
    public static IIntegrator GetIntegrator(this EIntegrationMethod method) => method switch
    {
        EIntegrationMethod.RK2 => new RK2Integrator(),
        EIntegrationMethod.RK4 => new RK4Integrator(),
        EIntegrationMethod.EULER => new EulerIntegrator(),
        _ => throw new ArgumentException("Integrator not found")
    };
}