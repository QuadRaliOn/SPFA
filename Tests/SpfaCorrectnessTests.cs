using SFPA.Benchmarks;
using SFPA.Solvers;

namespace Tests;

public class SpfaCorrectnessTests {
    //  0 --(5)--> 1 --(10)--> 2
    [Fact]
    public void SimplePath_ShouldReturnCorrectDistances() {
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

    //  0 --(10)--> 1, 0 --(5)--> 2, 2 --(-2)--> 1
    // shortest path 1: 0 -> 2 -> 1 (weight 3)
    [Fact]
    public void NegativeWeights_NoCycles_ShouldWork() {
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
        int vCount = 2; 
        var adj = new List<Edge>[vCount];
        adj[0] = new List<Edge>();
        adj[1] = new List<Edge>();

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(long.MaxValue, distances[1]);
    }
    
    // 0 -> 1 (10), 0 -> 2 (5), 2 -> 3 (2), 3 -> 1 (1)
    // Path 0-1: 10
    // Path 0-2-3-1: 5 + 2 + 1 = 8 (correct)
    [Fact]
    public void MultiplePaths_ShouldPickShortest() {

        int vCount = 4;
        var adj = Array.ConvertAll(new int[vCount], _ => new List<Edge>());
        adj[0].Add(new Edge(1, 10));
        adj[0].Add(new Edge(2, 5));
        adj[2].Add(new Edge(3, 2));
        adj[3].Add(new Edge(1, 1));

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(8, distances[1]);
    }
    
    // 0 -> 1 (5), 1 -> 2 (5), 2 -> 1 (5)
    [Fact]
    public void PositiveCycle_ShouldTerminateCorrectly() {

        int vCount = 3;
        var adj = Array.ConvertAll(new int[vCount], _ => new List<Edge>());
        adj[0].Add(new Edge(1, 5));
        adj[1].Add(new Edge(2, 5));
        adj[2].Add(new Edge(1, 5)); 

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(5, distances[1]);
        Assert.Equal(10, distances[2]);
    }
    
    [Fact]
    public void SelfLoop_ShouldBeIgnored() {
        int vCount = 1;
        var adj = new List<Edge>[vCount];
    
        adj[0] = new List<Edge> { new Edge(0, 10) };

        var solver = new SequentialSpfaSolver();
        var distances = solver.Solve(vCount, adj, 0);

        Assert.Equal(0, distances[0]);
    }
}