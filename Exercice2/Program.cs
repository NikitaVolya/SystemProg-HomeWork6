namespace Exercice2
{
    internal class Program
    {
        static Random random = new Random();

        static object locker = new object();

        static int Height = 15;
        static int Width = 15;
        static float TargetProcent = 0.5f;
        static int AirplaneNumber = 20;

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
            Console.Clear();
            List<Position> airplanes_positions = (from ap in Airplanes select ap.Position).ToList();
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

        static void AirplaneWork(Airplane airplane)
        {
            while (true)
            {
                lock (locker)
                {
                    if (airplane.X >= Width || airplane.Y >= Height || airplane.X < 0 || airplane.Y < 0)
                        break;
                    DetectedField[airplane.Y, airplane.X] = Field[airplane.Y, airplane.X];
                    airplane.Move();
                }
                Thread.Sleep(500);
            }
        }

        static void Main(string[] args)
        {
            UpdateField();
            Position start_position = new Position(
                Width / 2 + random.Next(0, Width / 2), 
                Height / 2 + random.Next(2, Height / 2)
                );



            Task display_task = Task.Run(() =>
            {
                while (true)
                {
                    lock (locker)
                    {
                        PrintField();
                        if (AllAirplanesFinished())
                        {
                            Console.WriteLine("All airplanes finished their work.");
                            break;
                        }
                    }
                    Thread.Sleep(500);
                }
            });
            List<Task> airplanes_work = new List<Task>();
            for (int i = 0; i < AirplaneNumber; i++)
            {
                airplanes_work.Add(
                    Task.Run(() =>
                    {
                        Airplane new_airplane = new Airplane(start_position, random.Next(0, 360));
                        Airplanes.Add(new_airplane);
                        AirplaneWork(new_airplane);
                    })
                 );
            };

            display_task.Wait();
            foreach (var airplane in airplanes_work)
            {
                airplane.Wait();
            }

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

        public void Move()
        {
            _x += _delta_x;
            _y += _delta_y;
        }
    }
}
