using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;

    private IPlayer _player;
    private IMonster _goblin;

    public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager)
    {
        _menuManager = menuManager;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void GameLoop()
    {
        _outputManager.Clear();

        while (true)
        {
            _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Attack");
            _outputManager.WriteLine("2. Search Inventory");
            _outputManager.WriteLine("3. Search Inventory by Type");
            _outputManager.WriteLine("4. Sort Items");
            _outputManager.WriteLine("0. Quit");

            _outputManager.Display();

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AttackCharacter();
                    break;
                case "2":
                    SearchInventory();
                    break;
                case "3":
                    ListItemsByType();
                    break;
                case "4":
                    SortItems();
                    break;
                case "0":
                    _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    _outputManager.Display();
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose 1.", ConsoleColor.Red);
                    break;
            }
        }
    }

    public void SearchInventory()
    {
        var items = _context.Items.Where(item => item.Name == "Elven Bow");
        foreach (var item in items)
        {
            Console.WriteLine($"Your Item is {item.Name} of type {item.Type}");
        }
    }

    public void ListItemsByType()
    {
        var itemsByType = _context.Items
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToList();
        foreach (var item in itemsByType)
        {
            Console.WriteLine($"{item.Type}\t{item.Count}");
        }
    }

    public void SortItems()      
    {
        Console.WriteLine("\nSort Options:");
        Console.WriteLine("1. Sort by Name");
        Console.WriteLine("2. Sort by Attack Value");
        Console.WriteLine("3. Sort by Defense Value");

        var input = Console.ReadLine();

        switch (input)
        {
            case "1":
                Console.WriteLine("Sort by Name");
                SortByName();
                break;
            case "2":
                Console.WriteLine("Sort by Attack Value");
                SortByAttackValue();
                break;
            case "3":
                Console.WriteLine("Sort by Defense Vaue");
                SortByDefenseValue();
                break;
            default:
                Console.WriteLine("Invalid Selection");
                break;
        }
    }
    private void SortByName()
    {
        var allItems = _context.Items.OrderBy(i => i.Name);
        foreach (var item in allItems)
        {
            Console.WriteLine($"\n{item.Name}");
        }
    }
    private void SortByAttackValue()
    {
        var allItems = _context.Items.OrderBy(i => i.Attack);
        foreach (var item in allItems)
        {
            Console.WriteLine($"\n{item.Name}\t{item.Attack}");
        }
    }
    private void SortByDefenseValue()
    {
        var allItems = _context.Items.OrderBy(i => i.Defense);
        foreach (var item in allItems)
        {
            Console.WriteLine($"\n{item.Name}\t{item.Defense}");
        }
    }
    private void AttackCharacter()
    {
        if (_goblin is ITargetable targetableGoblin)
        {
            _player.Attack(targetableGoblin);
            _player.UseAbility(_player.Abilities.First(), targetableGoblin);
        }
    }

    private void SetupGame()
    {
        _player = _context.Players.FirstOrDefault();
        _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);

        // Load monsters into random rooms 
        LoadMonsters();

        // Pause before starting the game loop
        Thread.Sleep(500);
        GameLoop();
    }

    private void LoadMonsters()
    {
        _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
    }

}
