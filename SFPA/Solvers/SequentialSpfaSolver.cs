using SFPA.Benchmarks;

namespace SFPA.Solvers;

public class SequentialSpfaSolver : IShortestPathSolver {
    public long[] Solve(int verticesCount, List<Edge>[] adj, int source) {
        long[] distances = SolverUtils.Prepare<bool, Queue<int>>(verticesCount, source, out var inQueue, out var queue);
        
        while (queue.Count > 0) {
            int u = queue.Dequeue();
            inQueue[u] = false;

            foreach (var edge in adj[u]) {
                if (RelaxationEngine.TryRelax(u, edge, distances)) {
                    if (!inQueue[edge.To]) {
                        queue.Enqueue(edge.To);
                        inQueue[edge.To] = true;
                    }
                }
            }
        }

        return distances;
    }
}