using Xunit;
using SFPA.Solvers;
using SFPA.Benchmarks;

public class SpfaCorrectnessTests {
    [Fact]
    public void SimplePath_ShouldReturnCorrectDistances() {
        // Граф: 0 --(5)--> 1 --(10)--> 2
        int vCount = 3;
        var adj = new List<Edge>[vCount];
        for (int i = 0; i < vCount; i++) adj[i] = new List<Edge>();
        adj[0].Add(new Edge(1, 5));
        adj[1].Add(new Edge(2, 10));

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(0, distances[0]);
        Assert.Equal(5, distances[1]);
        Assert.Equal(15, distances[2]);
    }

    [Fact]
    public void NegativeWeights_NoCycles_ShouldWork() {
        // Граф з від'ємною вагою: 0 --(10)--> 1, 0 --(5)--> 2, 2 --(-2)--> 1
        // Найкоротший шлях до 1: 0 -> 2 -> 1 (вага 3)
        int vCount = 3;
        var adj = new List<Edge>[vCount];
        for (int i = 0; i < vCount; i++) adj[i] = new List<Edge>();
        adj[0].Add(new Edge(1, 10));
        adj[0].Add(new Edge(2, 5));
        adj[2].Add(new Edge(1, -2));

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(3, distances[1]);
    }

    [Fact]
    public void UnreachableNodes_ShouldStayInfinity() {
        int vCount = 2; // 0 і 1 не з'єднані
        var adj = new List<Edge>[vCount];
        adj[0] = new List<Edge>();
        adj[1] = new List<Edge>();

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(long.MaxValue, distances[1]);
    }
}