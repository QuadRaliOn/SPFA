namespace SFPA.Benchmarks;

public static class ResearchGraphFactory {
    public static List<Edge>[] Create(int v, int density) {
        var adj = Enumerable.Range(0, v).Select(_ => new List<Edge>()).ToArray();
        Random rand = new Random(42);

        long totalEdgesNeeded = v * density;
        long additionalEdges = totalEdgesNeeded - (v - 1);
        
        CreateVerticesSpine(v, adj, rand);
        ConnectVertices(v, additionalEdges, rand, adj);

        return adj;
    }

    private static void CreateVerticesSpine(int v, List<Edge>[] adj, Random rand) {
        for (int i = 0; i < v - 1; i++) {
            adj[i].Add(new Edge(i + 1, rand.Next(1, 100)));
        }
    }

    private static void ConnectVertices(int v, long additionalEdges, Random rand, List<Edge>[] adj) {
        for (long i = 0; i < additionalEdges; i++) {
            int from = rand.Next(v);
            int to = rand.Next(v);

            if (from == to || adj[from].Any(e => e.To == to)) {
                i--;
                continue;
            }

            adj[from].Add(new Edge(to, rand.Next(1, 100)));
        }
    }
}