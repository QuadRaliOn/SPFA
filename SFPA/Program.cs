using System.Diagnostics;
using System.Text;
using SFPA.Benchmarks;

namespace SFPA
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            new ResearchOrchestrator().RunBatchSizeBenchmark();
            //new ResearchOrchestrator().RunAndExport("research_results.csv");
            
            Console.ReadKey();
        }
        
        
    }
}