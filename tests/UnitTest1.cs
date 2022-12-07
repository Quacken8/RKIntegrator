namespace particle_tests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void TestMethod1()
    {

        Integrator integrator = new Integrator();
    }
}

public class MockupOutputHandler : IOutputHandler{
    
    public List<Vector2> positions = new List<Vector2>();
    public List<double> energies = new List<double>();
    public List<double> angularMomenta = new List<double>();
    public void write(){
        Console.WriteLine();
    }
}