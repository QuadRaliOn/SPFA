namespace SFPA.Benchmarks;

public record Edge(int To, int Weight);

public record ResearchResult(
    int Vertices, 
    int Edges, 
    double Density, 
    int Threads, 
    double TimeMs, 
    double Speedup
);