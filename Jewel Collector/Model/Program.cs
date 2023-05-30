

public interface ICell
{
    void Print();//Alteração realizada por Byron
    bool CanMoveTo();
}

public class Jewel : ICell
{
    public enum JewelType { Red = 80, Green = 50, Blue = 10 };// Outra alteração realizada por Byron Conflicto red 100 mudado a 80
    public JewelType Type { get; set; }

    public Jewel(JewelType type) => Type = type;

    public bool CanMoveTo() => true;
    public void Print() => Console.Write(Type switch
    {
        JewelType.Red => "JR",
        JewelType.Green => "JG",
        JewelType.Blue => "JB",
        _ => throw new NotImplementedException()
    });
}

public class Obstacle : ICell
{
    public enum ObstacleType { Water, Tree }
    public ObstacleType Type { get; set; }

    public Obstacle(ObstacleType type) => Type = type;

    public bool CanMoveTo() => false;
    public void Print() => Console.Write(Type switch
    {
        ObstacleType.Water => "##",
        ObstacleType.Tree => "$$",
        _ => throw new NotImplementedException()
    });
}

public class Empty : ICell
{
    public bool CanMoveTo() => true;
    public void Print() => Console.Write("--");
}

public class Robot : ICell
{
    private Dictionary<Jewel.JewelType, int> bag = new Dictionary<Jewel.JewelType, int>();

    public bool CanMoveTo() => true;
    public void Print() => Console.Write("ME");

    public void CollectJewel(Jewel jewel)
    {
        if (!bag.ContainsKey(jewel.Type))
        {
            bag[jewel.Type] = 0;
        }

        bag[jewel.Type]++;
    }

    public void PrintBag()
    {
        Console.WriteLine("Bag Contents:");
        foreach (var jewel in bag)
        {
            Console.WriteLine($"{jewel.Key}: {jewel.Value}");
        }
    }
}

public class Map
{
    private ICell[,] cells;

    public Map(int width, int height)
    {
        cells = new ICell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                cells[i, j] = new Empty();
            }
        }
    }

    public void AddCell(int x, int y, ICell cell)
    {
        cells[x, y] = cell;
    }

    public ICell GetCell(int x, int y)
    {
        return cells[x, y];
    }

    public void Print()
    {
        Console.Write("\n");
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i, j].Print();
                Console.Write(" ");
            }

            Console.WriteLine();
        }
    }
}

public class JewelCollector
{
    public static void Main()
    {
        bool running = true;
        int size = 10;
        Map map = new Map(size, size);


        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                map.AddCell(i, j, new Empty());

        Robot robot = new Robot();
        map.AddCell(0, 0, robot);


        map.AddCell(1, 9, new Jewel(Jewel.JewelType.Red));
        map.AddCell(8, 8, new Jewel(Jewel.JewelType.Red));
        map.AddCell(9, 1, new Jewel(Jewel.JewelType.Green));
        map.AddCell(7, 6, new Jewel(Jewel.JewelType.Green));
        map.AddCell(3, 4, new Jewel(Jewel.JewelType.Blue));
        map.AddCell(2, 1, new Jewel(Jewel.JewelType.Blue));


        int totalJewels = 6;


        for (int i = 0; i <= 6; i++)
            map.AddCell(5, i, new Obstacle(Obstacle.ObstacleType.Water));

        map.AddCell(5, 9, new Obstacle(Obstacle.ObstacleType.Tree));
        map.AddCell(3, 9, new Obstacle(Obstacle.ObstacleType.Tree));
        map.AddCell(8, 3, new Obstacle(Obstacle.ObstacleType.Tree));
        map.AddCell(2, 5, new Obstacle(Obstacle.ObstacleType.Tree));
        map.AddCell(1, 4, new Obstacle(Obstacle.ObstacleType.Tree));


        int robotX = 0;
        int robotY = 0;
        do
        {
            map.Print();
            robot.PrintBag();

            Console.Write("Enter the command: ");
            ConsoleKey command = Console.ReadKey().Key;

            if (command == ConsoleKey.Q)
            {
                running = false;
            }
            else
            {
                int targetX = robotX;
                int targetY = robotY;
                if (command == ConsoleKey.W) targetX--;
                else if (command == ConsoleKey.A) targetY--;
                else if (command == ConsoleKey.S) targetX++;
                else if (command == ConsoleKey.D) targetY++;
                else if (command == ConsoleKey.G)
                {
                    var jewel = map.GetCell(targetX, targetY) as Jewel;
                    if (jewel != null)
                    {
                        robot.CollectJewel(jewel);
                        map.AddCell(targetX, targetY, new Empty());
                        totalJewels--;
                    }
                    continue;
                }

                if (targetX >= 0 && targetY >= 0 && targetX < size && targetY < size && map.GetCell(targetX, targetY).CanMoveTo())
                {
                    map.AddCell(robotX, robotY, new Empty());
                    map.AddCell(targetX, targetY, robot);
                    robotX = targetX;
                    robotY = targetY;
                }
            }


            //if (totalJewels == 0)
            //{
            //    Console.WriteLine("\nCongratulations! All jewels have been collected!");
            //    running = false;
            //}
        } while (running);
    }
}



