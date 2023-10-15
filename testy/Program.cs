using GWO;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hakuna");
        IOptimizationAlgorithm optimizer = new GreyWolfOptimizer();
        double result = 50;
        result = optimizer.Solve();
        Console.WriteLine(result);
    }
}