using System.Diagnostics;
using System.Text;
using SFPA.Solvers;

namespace SFPA.Benchmarks;

public class ResearchOrchestrator {
    private readonly int[] _densities = [/*2, 10,50,*/ 100];
    private readonly int[] _vertexCounts = [/*50000, 100000,*/500000];
    private readonly int _maxThreads = 16;
    private readonly int _benchmarkIterations = 3;

    public void RunAndExport(string fileName) {
        var allResults = new List<ResearchResult>();

        foreach (var vertexCount in _vertexCounts) {
            foreach (var density in _densities) {
                allResults.AddRange(RunScenario(vertexCount, density));
            }
        }

        ExportToCsv(allResults, fileName);
    }

    private List<ResearchResult> RunScenario(int vertexCount, int density) {
        var results = new List<ResearchResult>();
        var adj = ResearchGraphFactory.Create(vertexCount, density);
        Console.WriteLine($"\n>>>V={vertexCount}, Density={density}");

        double seqTime = BenchmarkSolver(new SequentialSpfaSolver(), vertexCount, adj);
        Console.WriteLine($" [Base] Sequential: {seqTime:F4}ms");

        for (int t = 1; t <= _maxThreads; t++) {
            double parTime = BenchmarkSolver(new BatchParallelSpfaSolver(t), vertexCount, adj);
            double speedup = seqTime / parTime;
            results.Add(new ResearchResult(vertexCount, vertexCount * density, density, t, parTime, speedup));
            
            Console.WriteLine($"Threads: {t} | Time: {parTime:F2}ms | S: {speedup:F2}x");
        }

        return results;
    }

    private double BenchmarkSolver(IShortestPathSolver solver, int v, List<Edge>[] adj) {
        WarmUp(solver, v, adj);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var sw = Stopwatch.StartNew();
        double totalMs = 0;
        for (int i = 0; i < _benchmarkIterations; i++) {
            sw.Restart();
            solver.Solve(v, adj, 0);
            sw.Stop();
            totalMs += sw.Elapsed.TotalMilliseconds;
        }

        return totalMs / _benchmarkIterations;
    }

    private void ExportToCsv(List<ResearchResult> results, string fileName) {
        var csv = new StringBuilder("Vertices;Edges;Density;Threads;TimeMs;Speedup\n");
        foreach (var r in results)
            csv.AppendLine($"{r.Vertices};{r.Edges};{r.Density};{r.Threads};{r.TimeMs:F4};{r.Speedup:F4}");
        File.WriteAllText(fileName, csv.ToString());
        Console.WriteLine($"\n[Done] Файл: {Path.GetFullPath(fileName)}");
    }

    private static void WarmUp(IShortestPathSolver solver, int v, List<Edge>[] adj) => 
        solver.Solve(v, adj, 0);
}