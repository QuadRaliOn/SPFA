using SFPA.Benchmarks;

namespace SFPA.Solvers;

public enum RealizationMethod {
    Parallel,
    Sequential
}

public static class RelaxationEngine {
    public static bool TryRelax(int from, Edge edge, long[] distances) {
        long newDist = distances[from] + edge.Weight;

        if (newDist >= Volatile.Read(ref distances[edge.To]))
            return false;

        long initialDist;
        do {
            initialDist = Interlocked.Read(ref distances[edge.To]);
            if (newDist >= initialDist) 
                return false;
        } while (Interlocked.CompareExchange(ref distances[edge.To], newDist, initialDist) != initialDist);

        return true;
    }
}