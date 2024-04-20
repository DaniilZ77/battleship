// Прочитай README перед тем как играть

namespace Battleship
{
    
    // рекорд для координаты
    internal record Coordinate(int X, int Y);

    
    // рекорд для корабля и его координаты, метод для удаления координаты если противник попал в корабль
    internal record Ship(List<Coordinate> Coordinates)
    {
        public readonly List<Coordinate> DeletedCoordinates = new();
        public bool DeleteShipCell(Coordinate coordinate)
        {
            if (!Coordinates.Contains(coordinate)) return false;
            Coordinates.Remove(coordinate);
            DeletedCoordinates.Add(coordinate);
            return true;
        }
    }
    
    // игроки
    internal enum Player
    {
        First,
        Second
    }
    
    // варианты расстановок
    internal enum InitMode
    {
        Random,
        Manual
    }
    
    public class Battleship
    {
        // размер поля
        private static readonly int FieldSize = 10;
        
        // генерирую корабли
        internal static Ship GenerateShip(int shipLen, List<Ship> ships, int fieldSize)
        {
            var rand = new Random();
            List<Coordinate> coordinates = new();
            var correctPlace = false;
            
            while (!correctPlace)
            {
                // генерирую рандомный корбаль заданного размера
                var direction = rand.Next(0, 2); // 0 - горизонтальное, 1 - вертикальное
                var startX = rand.Next(0, fieldSize);
                var startY = rand.Next(0, fieldSize);
                coordinates = new List<Coordinate>();
                
                if (direction == 0)
                {
                    
                    for (var x = startX % (fieldSize - shipLen + 1); x < startX % (fieldSize - shipLen + 1) + shipLen; x++)
                    {
                        coordinates.Add(new Coordinate(x, startY));
                    }
                }
                else
                {
                    for (var y = startY % (fieldSize - shipLen + 1); y < startY % (fieldSize - shipLen + 1) + shipLen; y++)
                    {
                        coordinates.Add(new Coordinate(startX, y));
                    }
                }
                
                // проверяю не пересекаются ли корабли
                correctPlace = CheckShip(coordinates, ships);
            }
            
            return new Ship(coordinates);
        }
        
        // проверка корабля на то что он не касается с другими и не пересекается
        internal static bool CheckShip(List<Coordinate> ship, List<Ship> ships)
        {
            var correctCoordinates = true;
            
            for (var i = 1; i < ship.Count; i++)
            {
                if (ship[i - 1].X != ship[i].X)
                {
                    correctCoordinates = false;
                }
            }
            
            if (!correctCoordinates)
            {
                correctCoordinates = true;
                for (var i = 1; i < ship.Count; i++)
                {
                    if (ship[i - 1].Y != ship[i].Y)
                    {
                        correctCoordinates = false;
                    }
                }
            }
            
            var correctPlace = true;
            
            foreach (var t in ships)
            {
                foreach (var t1 in ship)
                {
                    if (t.Coordinates.Contains(t1))
                    {
                        correctPlace = false;
                    }
                    foreach (var c in new List<Coordinate>(){ new(t1.X - 1, t1.Y), new(t1.X + 1, t1.Y), new(t1.X, t1.Y - 1), new(t1.X, t1.Y + 1) }.Where(c => t.Coordinates.Contains(c)))
                    {
                        correctPlace = false;
                    }
                }
            }
            
            return correctPlace && correctCoordinates;
        }
        
        // генерирую расстановку кораблей для игроков (РАНДОМНО)
        internal static Dictionary<Player, List<Ship>> Init()
        {
            Dictionary<Player, List<Ship>> ships = new()
            {
                { Player.First, new List<Ship>()},
                { Player.Second, new List<Ship>()}
            };
            
            // 4-ех палубный корабль
            var ship1 = GenerateShip(4, ships[Player.First], FieldSize);
            var ship2 = GenerateShip(4, ships[Player.Second], FieldSize);
            ships[Player.First].Add(ship1);
            ships[Player.Second].Add(ship2);
            
            // 3-х палубный корабль
            for (var i = 0; i < 2; i++)
            {
                ship1 = GenerateShip(3, ships[Player.First], FieldSize);
                ship2 = GenerateShip(3, ships[Player.Second], FieldSize);
                ships[Player.First].Add(ship1);
                ships[Player.Second].Add(ship2);
            }
            
            // 2-ух палубный корабль
            for (var i = 0; i < 3; i++)
            {
                ship1 = GenerateShip(2, ships[Player.First], FieldSize);
                ship2 = GenerateShip(2, ships[Player.Second], FieldSize);
                ships[Player.First].Add(ship1);
                ships[Player.Second].Add(ship2);
            }
            
            // 1 палубный корабль
            for (var i = 0; i < 4; i++)
            {
                ship1 = GenerateShip(1, ships[Player.First], FieldSize);
                ship2 = GenerateShip(1, ships[Player.Second], FieldSize);
                ships[Player.First].Add(ship1);
                ships[Player.Second].Add(ship2);
            }

            return ships;
        }
        
        // ручная расстановка кораблей в ручную
        internal static Dictionary<Player, List<Ship>> ManualInit()
        {
            Dictionary<Player, List<Ship>> ships = new()
            {
                { Player.First, new List<Ship>()},
                { Player.Second, new List<Ship>()}
            };
            
            var amountOfShips = new Dictionary<int, int>()
            {
                {1, 4},
                {2, 3},
                {3, 2},
                {4, 1},
            };
            
            // ввод кораблей 1 игрока
            for (var i = 1; i <= 4; i++)
            {
                for (var j = 0; j < amountOfShips[i]; j++)
                {
                    Console.WriteLine($"Игрок 1, введите координаты {i}-палубного корабля в формате 'X Y', где 'X' - горизонтальная координата, 'Y' - вертикальная. 0<=X,Y<=9");
                    var ship = GetShip(i, FieldSize);
                    
                    while (!CheckShip(ship, ships[Player.First]))
                    {
                        WarningMessageForUserInput();
                        ship = GetShip(i, FieldSize);
                    }
                    
                    ships[Player.First].Add(new Ship(ship));
                    DrawInit(Player.First, ships[Player.First]);
                }
            }
            
            // ввод кораблей 2 игрока
            for (var i = 1; i <= 4; i++)
            {
                for (var j = 0; j < amountOfShips[i]; j++)
                {
                    Console.WriteLine($"Игрок 2, введите координаты {i}-палубного корабля в формате 'X Y', где 'X' - горизонтальная координата, 'Y' - вертикальная. 0<=X,Y<=9");
                    var ship = GetShip(i, FieldSize);
                    
                    while (!CheckShip(ship, ships[Player.First]))
                    {
                        WarningMessageForUserInput();
                        ship = GetShip(i, FieldSize);
                    }
                    
                    ships[Player.Second].Add(new Ship(ship));
                    DrawInit(Player.Second, ships[Player.Second]);
                }
            }
            
            return ships;
        }

        private static void WarningMessageForUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Введите все координаты корабля заново в формате 'X Y', где 'X' - горизонтальная координата, 'Y' - вертикальная. Какие-то из введенных координат находятся рядом с другим коряблем, либо пересекают его, либо введенные координаты корабля находятся не на одной линии. 0<=X,Y<=9");
            Console.ResetColor();
        }

        // Запрос координат кораблей для ручного ввода
        internal static List<Coordinate> GetShip(int shipLen, int fieldSize)
        {
            List<Coordinate> coordinates = new();
            
            for (var i = 0; i < shipLen; i++)
            {
                var coords = Console.ReadLine()!.Split(" ");
                int x, y;
                while (coords.Length != 2 || !int.TryParse(coords[0], out x) || !int.TryParse(coords[1], out y) || x < 0 || x >= fieldSize || y < 0 || y >= fieldSize)
                {
                    WrongCoordinateFromUserInput();
                    coords = Console.ReadLine()!.Split(" ");
                }
                
                var coordinate = new Coordinate(x, y);
                coordinates.Add(coordinate);
            }
            
            return coordinates;
        }

        private static void WrongCoordinateFromUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                "Введите координаты еще раз в формате 'X Y', где 'X' - горизонтальная координата, 'Y' - вертикальная");
            Console.ResetColor();
        }

        // выбор игроком варианта расстоновки: рандомная или ручная
        internal static Tuple<InitMode, Dictionary<Player, List<Ship>>> ChooseInit()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Выберите как расставить корабли");
            Console.ResetColor();
            Console.WriteLine("1. Рандомно");
            Console.WriteLine("2. Вручную");
            
            var choose = Console.ReadLine();
            int input;
            
            while (!int.TryParse(choose, out input) || input < 1 || input > 2)
            {
                Console.WriteLine("Повторите попытку");
                choose = Console.ReadLine();
            }
            
            var mode = InitMode.Random;
            Dictionary<Player, List<Ship>> ships = new();
            
            switch (input)
            {
                case 1:
                    mode = InitMode.Random;
                    ships = Init();
                    break;
                case 2:
                    mode = InitMode.Manual;
                    ships = ManualInit();
                    break;
            }
            
            return new Tuple<InitMode, Dictionary<Player, List<Ship>>>(mode, ships);
        }
        
        // отрисовка игрового поля с метками если игрок уже ходил в определенную координату
        internal static void DrawField(List<Coordinate> destroyedShips, List<Coordinate> tries, List<Coordinate> fullyDestroyedShips)
        {
            var field = new List<List<char>>()
            {
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
            };
            
            foreach (var t in destroyedShips)
            {
                field[t.Y][t.X] = '+';
            }
            
            foreach (var t in tries)
            {
                field[t.Y][t.X] = '#';
            }

            foreach (var t in fullyDestroyedShips)
            {
                field[t.Y][t.X] = '*';
            }
            
            Console.WriteLine("0 1 2 3 4 5 6 7 8 9");
            
            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    Console.Write(field[i][j] + " ");
                }
                Console.WriteLine(i);
            }
        }
        
        // запрашиваю координату у игрока и проверяю правильность ввода
        internal static Coordinate GetCoordinate(Player player)
        {
            Console.WriteLine($"Ход игрока {player}. Введите координату в формате 'X Y', где 'X' - горизонтальная координата, 'Y' - вертикальная");
            while (true)
            {
                var input = Console.ReadLine();
                var coordinates = input?.Split(" ");
                
                if (coordinates is { Length: 2 } && int.TryParse(input?.Split(" ")[0], out var x) && int.TryParse(input.Split(" ")[1], out var y) && x is >= 0 and <= 9 && y is >= 0 and <= 9)
                {
                    return new Coordinate(x, y);
                }

                WrongCoordinateFromUserInput();
            }
        }
        
        // ход игрока currentPlayer
        internal static bool MakeMove(Dictionary<Player, List<Ship>> ships, Dictionary<Player, List<Coordinate>> destroyedShips, Dictionary<Player, List<Coordinate>> fullyDestroyedShips, Dictionary<Player, List<Coordinate>> tries, Player currentPlayer, Player opponent, Coordinate coordinate)
        {
            var hitTarget = false;
            for (var i = 0; i < ships[opponent].Count; i++)
            {
                hitTarget = ships[opponent][i].DeleteShipCell(coordinate);
                
                if (!hitTarget) continue;
                
                if (ships[opponent][i].Coordinates.Count == 0)
                {
                    fullyDestroyedShips[currentPlayer].AddRange(ships[opponent][i].DeletedCoordinates);
                    ships[opponent].Remove(ships[opponent][i]);
                }
                
                destroyedShips[currentPlayer].Add(coordinate);
                break;
            }
            
            if (!hitTarget)
            {
                tries[currentPlayer].Add(coordinate);
            }
            
            return hitTarget;
        }
        
        // изначальная отрисовка поля, чтобы дать игрокам посмотреть на расстановку их кораблей
        internal static void DrawInit(Player player, List<Ship> ships)
        {
            var field = new List<List<char>>()
            {
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
                new() {'.', '.', '.', '.', '.', '.', '.', '.', '.', '.'},
            };
            
            foreach (var t1 in ships.SelectMany(t => t.Coordinates))
            {
                if (player == Player.First)
                {
                    field[t1.Y][t1.X] = '1';    
                }
                else
                {
                    field[t1.Y][t1.X] = '2';
                }
            }
            
            Console.WriteLine("0 1 2 3 4 5 6 7 8 9");
            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    Console.Write(field[i][j] + " ");
                }
                Console.WriteLine(i);
            }
        }
        
        // главный цикл игры
        internal static Player Game()
        {
            // вывожу памятку для пользователя
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ВНИМАНИЕ");
            Console.ResetColor();
            Console.WriteLine("Перед началом игры рекомендуется прочитать файл README (путь: battleship/battleship/README.md), так как там подробно расписаны правила игры, ограничения, режимы игры. При выборе ручной расстоновки кораблей вводите координаты каждой клетки, при вводе координаты в неправильном формате или в случае если невозможно поставить корабль, введите заново. '#' означает промах, '+' означает попадание, '*' означает, что корабль полностью уничтожен.");
            // -------------------------------
            const int numberOfBlankLines = 30;
            var (chosenInit, ships) = ChooseInit();
            
            // показываю игрокам их расстановку если она была рандомной
            if (chosenInit == InitMode.Random)
            {
                Console.WriteLine("Доска игрока 1");
                
                DrawInit(Player.First, ships[Player.First]);
                
                Console.WriteLine("Нажмите Enter чтобы показать доску следующего игрока");
                Console.ReadLine();
                
                for (var i = 0; i < numberOfBlankLines; i++)
                {
                    Console.WriteLine("/");
                }
                
                Console.WriteLine("Нажмите Enter чтобы показать доску игрока");
                Console.ReadLine();
                
                Console.WriteLine("Доска игрока 2");
                
                DrawInit(Player.Second, ships[Player.Second]);
                
                Console.WriteLine("Нажмите Enter чтобы начать игру");
                Console.ReadLine();
                
                for (var i = 0; i < numberOfBlankLines; i++)
                {
                    Console.WriteLine("/");
                }   
            }
            // --------------------------------
            
            Dictionary<Player, List<Coordinate>> destroyedShips = new()
            {
                {Player.First, new List<Coordinate>()},
                {Player.Second, new List<Coordinate>()},
            };
            
            Dictionary<Player, List<Coordinate>> tries = new()
            {
                {Player.First, new List<Coordinate>()},
                {Player.Second, new List<Coordinate>()},
            };
            
            Dictionary<Player, List<Coordinate>> fullyDestroyedShips = new()
            {
                {Player.First, new List<Coordinate>()},
                {Player.Second, new List<Coordinate>()},
            };
            
            var currentPlayer = Player.First;
            var opponent = Player.Second;
            
            while (ships[Player.First].Count > 0 && ships[Player.Second].Count > 0)
            {
                DrawField(destroyedShips[currentPlayer], tries[currentPlayer], fullyDestroyedShips[currentPlayer]);
                var coordinate = GetCoordinate(currentPlayer);
                
                while (destroyedShips[currentPlayer].Contains(coordinate) || tries[currentPlayer].Contains(coordinate))
                {
                    coordinate = GetCoordinate(currentPlayer);
                }
                
                var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, currentPlayer, opponent, coordinate);
                
                DrawField(destroyedShips[currentPlayer], tries[currentPlayer], fullyDestroyedShips[currentPlayer]);
                
                Console.WriteLine("Нажмите Enter чтобы продолжить");
                Console.ReadLine();
                
                if (hitTarget) continue;
                
                currentPlayer = currentPlayer == Player.First ? Player.Second : Player.First;
                opponent = opponent == Player.First ? Player.Second : Player.First;
            }
            
            return ships[Player.First].Count > 0 ? Player.First : Player.Second;
        }
        public static void Main()
        {
            
            var winner = Game();
            Console.WriteLine($"Победитель: {winner}");
        }
    }
}