using NUnit.Framework;
using static Battleship.Battleship;

namespace Battleship;

public class Tests
{
    private readonly TextWriter _standartOut = Console.Out;
    private StringWriter _stringWriter = new();

    [SetUp]
    public void Setup()
    {
        var stringWriter = new StringWriter();
        _stringWriter = stringWriter;
        Console.SetOut(_stringWriter);
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(_standartOut);
        _stringWriter.Close();
    }
    
    private void AssertOut(string expected)
    {
        Assert.That(_stringWriter.ToString().TrimEnd(Environment.NewLine.ToCharArray()), Is.EqualTo(expected));
    }
    
    [Test]
    public void GenerateShipTest1()
    {
        var ships = new List<Ship>()
        {
            new(new List<Coordinate> { new(0, 0), new(1, 0), new(2, 0), }),
        };
        
        var ship = GenerateShip(3, ships, 3);
        var expected = new Ship(new List<Coordinate>() { new(0, 2), new(1, 2), new(2, 2), });
        
        Assert.That(ship.Coordinates, Has.Count.EqualTo(expected.Coordinates.Count));
        for (var i = 0; i < expected.Coordinates.Count; i++)
        {
            Assert.That(expected.Coordinates, Does.Contain(ship.Coordinates[i]));
        }
    }
    
    [Test]
    public void GenerateShipTest2()
    {
        var ships = new List<Ship>()
        {
            new(new List<Coordinate> { new(0, 0), new(1, 0) }),
            new(new List<Coordinate> { new(0, 1), new(1, 1) }),
            new(new List<Coordinate> { new(0, 2), new(1, 2) }),
            new(new List<Coordinate> { new(0, 3), new(1, 3) }),
        };
        
        var ship = GenerateShip(4, ships, 4);
        var expected = new Ship(new List<Coordinate>() { new(3, 0), new(3, 1), new(3, 2), new(3, 3), });
        
        Assert.That(ship.Coordinates, Has.Count.EqualTo(expected.Coordinates.Count));
        for (var i = 0; i < expected.Coordinates.Count; i++)
        {
            Assert.That(expected.Coordinates, Does.Contain(ship.Coordinates[i]));
        }
    }
    
    [Test]
    public void GenerateShipTest3()
    {
        var ships = new List<Ship>()
        {
            new(new List<Coordinate> { new(0, 0), }),
        };
        
        var ship = GenerateShip(1, ships, 2);
        var expected = new Ship(new List<Coordinate>() { new(1, 1), });
        
        Assert.That(ship.Coordinates, Has.Count.EqualTo(expected.Coordinates.Count));
        for (var i = 0; i < expected.Coordinates.Count; i++)
        {
            Assert.That(expected.Coordinates, Does.Contain(ship.Coordinates[i]));
        }
    }

    [Test]
    public void CheckShipTest1()
    {
        var ship = new Ship(new List<Coordinate>() { new(4, 4), new(5, 5) });
        
        Assert.That(CheckShip(ship.Coordinates, new List<Ship>()), Is.False);
    }
    
    [Test]
    public void CheckShipTest2()
    {
        var ship = new Ship(new List<Coordinate>() { new(4, 4), new(5, 5) });
        
        Assert.That(CheckShip(ship.Coordinates, new List<Ship>() { new(new List<Coordinate>() { new(4, 5) }) }), Is.False);
    }
    
    [Test]
    public void CheckShipTest3()
    {
        var ship = new Ship(new List<Coordinate>() { new(4, 4), new(4, 5) });
        
        Assert.That(CheckShip(ship.Coordinates, new List<Ship>()), Is.True);
    }
    
    [Test]
    public void InitTest()
    {
        var ships = Init();
        var shipsCount = new int[4];
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            shipsCount[ships[Player.First][i].Coordinates.Count - 1]++;
        }

        Assert.Multiple(() =>
        {
            Assert.That(shipsCount[0], Is.EqualTo(4));
            Assert.That(shipsCount[1], Is.EqualTo(3));
            Assert.That(shipsCount[2], Is.EqualTo(2));
            Assert.That(shipsCount[3], Is.EqualTo(1));
        });
        
        shipsCount = new int[4];
        for (var i = 0; i < ships[Player.Second].Count; i++)
        {
            shipsCount[ships[Player.Second][i].Coordinates.Count - 1]++;
        }

        Assert.Multiple(() =>
        {
            Assert.That(shipsCount[0], Is.EqualTo(4));
            Assert.That(shipsCount[1], Is.EqualTo(3));
            Assert.That(shipsCount[2], Is.EqualTo(2));
            Assert.That(shipsCount[3], Is.EqualTo(1));
        });
    }

    [Test]
    public void MakeMoveTest1()
    {
        var ships = Init();
        var coordinate = ships[Player.Second][0].Coordinates[0];
        
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
        
        var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, Player.First, Player.Second, coordinate);
        
        Assert.Multiple(() =>
        {
            Assert.That(hitTarget, Is.True);
            Assert.That(destroyedShips[Player.First], Does.Contain(coordinate));
        });
        
        for (var i = 0; i < ships[Player.Second].Count; i++)
        {
            Assert.That(ships[Player.Second][i].Coordinates, !Does.Contain(coordinate));
        }
    }

    [Test]
    public void MakeMoveTest2()
    {
        var ships = Init();
        for (var i = 0; i < ships[Player.Second].Count; i++)
        {
            if (ships[Player.Second][i].DeleteShipCell(new Coordinate(0, 0)))
            {
                break;
            }
        }
        
        var coordinate = new Coordinate(0, 0);
        
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
        
        var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, Player.First, Player.Second, coordinate);
        Assert.Multiple(() =>
        {
            Assert.That(hitTarget, Is.False);
            Assert.That(tries[Player.First], Does.Contain(coordinate));
        });
        
        for (var i = 0; i < ships[Player.Second].Count; i++)
        {
            Assert.That(ships[Player.Second][i].Coordinates, !Does.Contain(coordinate));
        }
    }

    [Test]
    public void MakeMoveTest3()
    {
        var ships = Init();
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            if (ships[Player.First][i].DeleteShipCell(new Coordinate(0, 0)))
            {
                break;
            }
        }
        
        var coordinate = new Coordinate(0, 0);
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
        
        var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, Player.Second, Player.First, coordinate);
        Assert.Multiple(() =>
        {
            Assert.That(hitTarget, Is.False);
            Assert.That(tries[Player.Second], Does.Contain(coordinate));
        });
        
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            Assert.That(ships[Player.First][i].Coordinates, !Does.Contain(coordinate));
        }
    }

    [Test]
    public void MakeMoveTest4()
    {
        var ships = Init();
        var coordinate = ships[Player.First][0].Coordinates[0];
        
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
        
        var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, Player.Second, Player.First, coordinate);
        
        Assert.Multiple(() =>
        {
            Assert.That(hitTarget, Is.True);
            Assert.That(destroyedShips[Player.Second], Does.Contain(coordinate));
        });
        
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            Assert.That(ships[Player.First][i].Coordinates, !Does.Contain(coordinate));
        }
    }
    [Test]
    public void MakeMoveTest5()
    {
        var ships = Init();
        Coordinate coordinate = new(0, 0);
        
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            if (ships[Player.First][i].Coordinates.Count != 1) continue;
            coordinate = ships[Player.First][i].Coordinates[0];
            break;
        }
        
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
        
        var hitTarget = MakeMove(ships, destroyedShips, fullyDestroyedShips, tries, Player.Second, Player.First, coordinate);
        
        Assert.Multiple(() =>
        {
            Assert.That(hitTarget, Is.True);
            Assert.That(destroyedShips[Player.Second], Does.Contain(coordinate));
            Assert.That(fullyDestroyedShips[Player.Second], Does.Contain(coordinate));
        });
        
        for (var i = 0; i < ships[Player.First].Count; i++)
        {
            Assert.That(ships[Player.First][i].Coordinates, !Does.Contain(coordinate));
        }
    }
    [Test]
    public void DrawFieldTest()
    {
        List<Coordinate> destroyedShips = new()
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 0),
        };
        
        List<Coordinate> tries = new()
        {
            new Coordinate(5, 2),
            new Coordinate(4, 1),
            new Coordinate(9, 9),
        };
        
        List<Coordinate> fullyDestroyedShips = new()
        {
            new Coordinate(3, 5),
            new Coordinate(4, 5),
            new Coordinate(5, 5),
        };
        
        DrawField(destroyedShips, tries, fullyDestroyedShips);
        AssertOut(
        @"0 1 2 3 4 5 6 7 8 9
+ + . . . . . . . . 0
+ . . . # . . . . . 1
. . . . . # . . . . 2
. . . . . . . . . . 3
. . . . . . . . . . 4
. . . * * * . . . . 5
. . . . . . . . . . 6
. . . . . . . . . . 7
. . . . . . . . . . 8
. . . . . . . . . # 9"
        );
    }
    [Test]
    public void DrawInitTest1()
    {
        List<Ship> ships1 = new()
        {
            new Ship(new List<Coordinate>() { new(0, 5), new(1, 5), new(2, 5), }),
            new Ship(new List<Coordinate>() { new(7, 8), new(8, 8), }),
            new Ship(new List<Coordinate>() { new(7, 1), new(8, 1), }),
        };
        
        DrawInit(Player.First, ships1);
        AssertOut(
            @"0 1 2 3 4 5 6 7 8 9
. . . . . . . . . . 0
. . . . . . . 1 1 . 1
. . . . . . . . . . 2
. . . . . . . . . . 3
. . . . . . . . . . 4
1 1 1 . . . . . . . 5
. . . . . . . . . . 6
. . . . . . . . . . 7
. . . . . . . 1 1 . 8
. . . . . . . . . . 9"
        );
    }
    [Test]
    public void DrawInitTest2()
    {
        List<Ship> ships2 = new()
        {
            new Ship(new List<Coordinate>() { new(0, 5), new(0, 6), new(0, 7), }),
            new Ship(new List<Coordinate>() { new(7, 8), new(8, 8), }),
            new Ship(new List<Coordinate>() { new(3, 5), new(3, 6), new(3, 7), new(3, 8),  }),
        };
        
        DrawInit(Player.Second, ships2);
        AssertOut(
            @"0 1 2 3 4 5 6 7 8 9
. . . . . . . . . . 0
. . . . . . . . . . 1
. . . . . . . . . . 2
. . . . . . . . . . 3
. . . . . . . . . . 4
2 . . 2 . . . . . . 5
2 . . 2 . . . . . . 6
2 . . 2 . . . . . . 7
. . . 2 . . . 2 2 . 8
. . . . . . . . . . 9"
        );
    }
}