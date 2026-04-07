using System.Collections.Concurrent;

namespace SFPA.Solvers;

public static class SolverUtils {
    public static long[] Prepare<TInQueue, TQueue>(
        int verticesCount, 
        int source, 
        out TInQueue[] inQueue, 
        out TQueue queue) 
        where TQueue : class, new()
    {
        var distances = new long[verticesCount];
        Array.Fill(distances, long.MaxValue);
        inQueue = new TInQueue[verticesCount];
        queue = new TQueue();

        distances[source] = 0;
        
        if (inQueue is bool[] boolArr && queue is Queue<int> q)
        {
            boolArr[source] = true;
            q.Enqueue(source);
        }
        else if (inQueue is int[] intArr && queue is ConcurrentQueue<int> cq)
        {
            intArr[source] = 1;
            cq.Enqueue(source);
        }

        return distances;
    }
}