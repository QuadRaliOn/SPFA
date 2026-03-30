using SFPA.Benchmarks;

namespace SFPA.Solvers;

public interface IShortestPathSolver
{
    long[] Solve(int verticesCount, List<Edge>[] adj, int source);
}