using System.Collections.Concurrent;
using SFPA.Benchmarks;

namespace SFPA.Solvers;

public class BatchParallelSpfaSolver : IShortestPathSolver {
    private readonly int _maxDegreeOfParallelism;

    private readonly int _batchSize = 16;

    public BatchParallelSpfaSolver(int maxThreads) {
        _maxDegreeOfParallelism = maxThreads;
    }
    
    public BatchParallelSpfaSolver(int maxThreads, int batchSize = 16) {
        _maxDegreeOfParallelism = maxThreads;
        _batchSize = batchSize;
    }

    public long[] Solve(int verticesCount, List<Edge>[] adj, int source) {
        var distances = SolverUtils.Prepare<int, ConcurrentQueue<int>>(verticesCount, source, out var inQueue, out var queue);
        var cts = new CancellationTokenSource();
        int activeWorkers = 0; 

        Parallel.For(0, _maxDegreeOfParallelism, _ => {
            while (!cts.IsCancellationRequested) {
                Interlocked.Increment(ref activeWorkers); 
            
                var batch = new List<int>(_batchSize);
                while (batch.Count < _batchSize && queue.TryDequeue(out int v)) 
                    batch.Add(v);

                if (batch.Count > 0) {
                    foreach (var u in batch) {
                        Interlocked.Exchange(ref inQueue[u], 0);
                        foreach (var edge in adj[u])
                            if (RelaxationEngine.TryRelax(u, edge, distances))
                                if (Interlocked.CompareExchange(ref inQueue[edge.To], 1, 0) == 0) 
                                    queue.Enqueue(edge.To);
                    }
                    Interlocked.Decrement(ref activeWorkers);
                } 
                else {
                    Interlocked.Decrement(ref activeWorkers);
                
                    if (Volatile.Read(ref activeWorkers) == 0 && queue.IsEmpty) {
                        cts.Cancel();
                        break;
                    }
                }
            }
        });

        return distances;
    }
}