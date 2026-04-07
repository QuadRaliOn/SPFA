using Xunit;
using SFPA.Solvers;
using SFPA.Benchmarks;

namespace Tests;

public class SpfaParallelTests {
    [Theory]
    [InlineData(1000, 10)]   
    [InlineData(5000, 50)]   
    [InlineData(10000, 2)]   
    [InlineData(30000, 20)]   
    [InlineData(40000, 35)]   
    [InlineData(35000, 30)]   
    [InlineData(50000, 100)]   
    [InlineData(100000, 50)]
    [InlineData(500000, 50)]
    public void Parallel_ShouldMatchSequential_OnRandomGraphs(int vCount, int density) {
        var adj = ResearchGraphFactory.Create(vCount, density);
        int source = 0;

        var seqResults = GenerateSequential(vCount, adj, source);
        var parResults = GenerateParallel(vCount, adj, source);

        Assert.Equal(seqResults.Length, parResults.Length);
        for (int i = 0; i < vCount; i++) {
            Assert.True(seqResults[i] == parResults[i], 
                $"Error in Vertex {i}: Sequential={seqResults[i]}, Parallel={parResults[i]}. " +
                $"Graph: V={vCount}, D={density}");
        }
    }

    private static long[] GenerateParallel(int vCount, List<Edge>[] adj, int source) {
        var parSolver = new BatchParallelSpfaSolver(Environment.ProcessorCount);
        long[] parResults = parSolver.Solve(vCount, adj, source);
        return parResults;
    }

    private static long[] GenerateSequential(int vCount, List<Edge>[] adj, int source) {
        var seqSolver = new SequentialSpfaSolver();
        long[] seqResults = seqSolver.Solve(vCount, adj, source);
        return seqResults;
    }
}