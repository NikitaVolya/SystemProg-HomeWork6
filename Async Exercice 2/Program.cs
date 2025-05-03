namespace Exercice2
{
    internal class Program
    {
        static Random random = new Random();

        static int Height = 15;
        static int Width = 15;
        static float TargetProcent = 0.25f;
        static int AirplaneNumber = 180;

        static bool[,] Field = new bool[Height, Width];
        static bool[,] DetectedField = new bool[Height, Width];

        static List<Airplane> Airplanes = new List<Airplane>();

        static bool AllAirplanesFinished()
        {
            foreach (var airplane in Airplanes)
            {
                if (airplane.X < Width && airplane.Y < Height && airplane.X >= 0 && airplane.Y >= 0)
                    return false;
            }
            return true;
        }

        static void UpdateField()
        {
            if (1 < TargetProcent)
                TargetProcent = 1;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                { 
                    Field[y, x] = false;
                    DetectedField[y, x] = false;
                }

            int target_count = (int)(Height * Width * TargetProcent);
            for (int i = 0; i < target_count; i++)
            {
                int tmp_x, tmp_y;
                do
                {
                    tmp_x = random.Next(0, Width);
                    tmp_y = random.Next(0, Height);
                } while (Field[tmp_y, tmp_x]);

                Field[tmp_y, tmp_x] = true;
            }
        }

        static void PrintField()
        {

            List<Position> airplanes_positions = (from ap in Airplanes select ap.Position).ToList();

            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Position current_pos = new Position(x, y);
                    if (airplanes_positions.Contains(current_pos))
                        Console.Write("# ");
                    else if (DetectedField[y, x])
                        Console.Write("X ");
                    else
                        Console.Write(". ");
                }
                Console.WriteLine();
            }
        }

        static void FinalPrintField()
        {
            int detected_count = 0;
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Field[y, x] && DetectedField[y, x])
                    { 
                        Console.Write("X ");
                        detected_count++;
                    }
                    else if (Field[y, x])
                        Console.Write("o ");
                    else
                        Console.Write(". ");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Detected {detected_count} targets out of {(int)(Height * Width * TargetProcent)}.");
            Console.WriteLine($"{(float)detected_count / (int)(Height * Width * TargetProcent) * 100}% of targets.");
        }

        static async Task AirplaneWork(Airplane airplane)
        {
            while (true)
            {
                if (airplane.X >= Width || airplane.Y >= Height || airplane.X < 0 || airplane.Y < 0)
                    break;
                DetectedField[airplane.Y, airplane.X] = Field[airplane.Y, airplane.X];
                await airplane.Move();
                await Task.Delay(500);
            }
        }

        static async Task AsyncDisplay()
        {
            while (true)
            {
                PrintField();
                if (AllAirplanesFinished())
                    break;
                await Task.Delay(500);
            }
        }

        static async Task Main(string[] args)
        {
            UpdateField();
            Position start_position = new Position(
                Width / 2 + random.Next(0, Width / 4), 
                Height / 2 + random.Next(2, Height / 4)
                );
            List<Task> airplanes_work = new List<Task>();

            List<Task> tasks = new List<Task>() {};

            for (int i = 0; i < AirplaneNumber; i++)
            {
                Airplane new_airplane = new Airplane(start_position, random.Next(0, 360));
                Airplanes.Add(new_airplane);
                tasks.Add(AirplaneWork(new_airplane));
            };
            tasks.Add(AsyncDisplay());

            await Task.WhenAll(tasks);

            FinalPrintField();
            Console.WriteLine("All airplanes finished their work.");

        }
    }

    record Position(int x, int y);

    class Airplane
    {
        float _x, _y, _delta_x, _delta_y;

        public Airplane(int x, int y, float angle)
        {
            _x = x;
            _y = y;
            _delta_x = (float)Math.Cos(angle % 360 / 180 * Math.PI);
            _delta_y = (float)Math.Sin(angle % 360 / 180 * Math.PI);
        }
        public Airplane(Position start_position, float angle) : this(start_position.x, start_position.y, angle) { }

        public int X { get => (int)_x; }
        public int Y { get => (int)_y; }

        public Position Position { get => new Position(X, Y); }

        public async Task Move()
        {
            _x += _delta_x;
            _y += _delta_y;
        }
    }
}
