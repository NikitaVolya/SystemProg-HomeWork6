namespace Exercice1
{
    internal class Program
    {
        static int GoldSum = 0;
        static Random random = new Random();

        static void UnitWork(string unit_name)
        {
            int current_gold = 0;
            while (true)
            {
                Console.WriteLine($"Unit {unit_name}: Working...");
                Thread.Sleep(2000);
                Console.WriteLine($"Unit {unit_name}: Received 30 gold...");
                current_gold = 30;

                Console.WriteLine($"Unit {unit_name}: Going to the base...");

                Thread.Sleep(random.Next(6000, 10000));


                Console.WriteLine($"Unit {unit_name}: Arrived at the base...");
                GoldSum += current_gold;
                current_gold = 0;
                Console.WriteLine($"Unit {unit_name}: Total gold in the base: {GoldSum}");
                Thread.Sleep(random.Next(6000, 10000));
            }
        }

        static void Main(string[] args)
        {
            Task[] units = new Task[] {
                Task.Run(() => UnitWork("1")),
                Task.Run(() => UnitWork("2")),
                Task.Run(() => UnitWork("3")),
                Task.Run(() => UnitWork("4")),
            };

            foreach (var unit in units)
            {
                unit.Wait();
            }
        }
    }
}
