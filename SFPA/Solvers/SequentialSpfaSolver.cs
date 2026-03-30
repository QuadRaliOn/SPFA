using SFPA.Benchmarks;

namespace SFPA.Solvers;

public class SequentialSpfaSolver : IShortestPathSolver {
    public long[] Solve(int verticesCount, List<Edge>[] adj, int source) {
        var distances = Prepare(verticesCount, source, out var inQueue, out var queue);

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

    private static long[] Prepare(int verticesCount, int source, out bool[] inQueue, out Queue<int> queue) {
        var distances = new long[verticesCount];
        Array.Fill(distances, long.MaxValue);
        inQueue = new bool[verticesCount];
        queue = new Queue<int>();

        distances[source] = 0;
        queue.Enqueue(source);
        inQueue[source] = true;
        return distances;
    }
}