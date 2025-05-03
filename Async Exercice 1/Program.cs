namespace Exercice1
{
    internal class Program
    {
        static int GoldTotalSum = 0;

        static Random random = new Random();

        static void ConsoleBreakeLine()
        {
            Console.WriteLine("--------------------------------------------------");
        }

        static async Task GetGold(string unit_name)
        {
            Console.WriteLine($"Unit {unit_name}: Working...");
            ConsoleBreakeLine();
            await Task.Delay(2000);
            Console.WriteLine($"Unit {unit_name}: Received 30 gold...");
            ConsoleBreakeLine();
        }

        static async Task GoingToBase(string unit_name)
        {
            Console.WriteLine($"Unit {unit_name}: Going to the base...");
            ConsoleBreakeLine();
            await Task.Delay(random.Next(6000, 10000));
            Console.WriteLine($"Unit {unit_name}: Arrived at the base...");
            ConsoleBreakeLine();
        }

        static async Task DeposeGold(string unit_name)
        {
            GoldTotalSum += 30;
            Console.WriteLine($"Unit {unit_name}: Total gold in the base: {GoldTotalSum}");
            ConsoleBreakeLine();
            Console.WriteLine($"Unit {unit_name}: Going back to work...");
            ConsoleBreakeLine();
            await Task.Delay(random.Next(6000, 10000));
        }

        static async Task UnitWork(string unit_name)
        {
            while (true)
            {
                await GetGold(unit_name);
                await GoingToBase(unit_name);
                await DeposeGold(unit_name);
            }
        }

        static async Task Main(string[] args)
        {
            var task = new List<Task>
            {
                UnitWork("1"),
                UnitWork("2"),
                UnitWork("3"),
                UnitWork("4"),
            };

            await Task.WhenAll(task);
        }
    }
}
