using System.Collections.Concurrent;
using SFPA.Benchmarks;

namespace SFPA.Solvers;

public class BatchParallelSpfaSolver : IShortestPathSolver {
    private readonly int _maxDegreeOfParallelism;
    private int _activeWorkers;
    
    private const int BatchSize = 16;

    public BatchParallelSpfaSolver(int maxThreads) {
        _maxDegreeOfParallelism = maxThreads;
    }

    public long[] Solve(int verticesCount, List<Edge>[] adj, int source) {
        var distances = Prepare(verticesCount, source, out var inQueue, out var queue);
        var cts = new CancellationTokenSource();

        Parallel.For(0, _maxDegreeOfParallelism,
            new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, body: _ => {
                int[] localBatch = new int[BatchSize];

                while (!cts.IsCancellationRequested) {
                    int count = 0;

                    while (count < BatchSize && queue.TryDequeue(out int vertex)) 
                        localBatch[count++] = vertex;

                    if (count > 0)
                        ProcessBatch(adj, inQueue, localBatch, count, distances, queue);
                    else
                        CheckWorkEnded(queue, cts);
                }
            });

        return distances;
    }

    private void ProcessBatch(List<Edge>[] adj, int[] inQueue, int[] batch, int count, long[] distances, ConcurrentQueue<int> queue) {
        Interlocked.Add(ref _activeWorkers, count);

        for (int i = 0; i < count; i++) {
            int u = batch[i];
            
            Interlocked.Exchange(ref inQueue[u], 0);

            foreach (var edge in adj[u]) {
                long newDist = distances[u] + edge.Weight;
                if (newDist < Volatile.Read(ref distances[edge.To])) {
                    if (RelaxationEngine.TryRelax(u, edge, distances)) {
                        if (Interlocked.CompareExchange(ref inQueue[edge.To], 1, 0) == 0)
                            queue.Enqueue(edge.To);
                    }
                }
            }
        }

        Interlocked.Add(ref _activeWorkers, -count);
    }

    private void CheckWorkEnded(ConcurrentQueue<int> queue, CancellationTokenSource cts) {
        if (Volatile.Read(ref _activeWorkers) == 0 && queue.IsEmpty)
            cts.Cancel();
        else
            Thread.Yield();
    }

    private static long[] Prepare(int verticesCount, int source, out int[] inQueue, out ConcurrentQueue<int> queue) {
        var distances = new long[verticesCount];
        Array.Fill(distances, long.MaxValue);
        inQueue = new int[verticesCount];
        queue = new ConcurrentQueue<int>();

        distances[source] = 0;
        queue.Enqueue(source);
        inQueue[source] = 1;
        return distances;
    }
}