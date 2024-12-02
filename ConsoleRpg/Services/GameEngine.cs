using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models;
using Microsoft.EntityFrameworkCore;

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
            _outputManager.WriteLine("2. Display All Players");
            _outputManager.WriteLine("3. Search Players");
            _outputManager.WriteLine("4. Sort Items");
            _outputManager.WriteLine("5. Choose Items");
            _outputManager.WriteLine("6. Create a New Player");
            _outputManager.WriteLine("7. Cheat and Increase Stats");
            _outputManager.WriteLine("0. Quit");

            _outputManager.Display();

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AttackCharacter();
                    break;
                case "2":
                    DisplayAllPlayers();
                    break;
                case "3":
                    SearchPlayers();
                    break;
                case "4":
                    SortItems();
                    break;
                case "5":
                    ChooseItems();
                    break;
                case "6":
                    CreateNewPlayer();
                    break;
                case "7":
                {
                    CheatMode();
                    break;
                }
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

    public void DisplayAllPlayers()
    {
        var players = _context.Players;

        foreach (var player in players)
        {
            System.Console.WriteLine($"Id: {player.Id}\tName: {player.Name}\tExperience: {player.Experience}\tHealth: {player.Health}");
        }
    }

    public void SearchPlayers()
    {
        var players = _context.Players;

        bool searchDone = false;

        while (!searchDone)
        {
            // check if there are any players in the table

            if (players.Any())
            {
               System.Console.WriteLine("Enter player name or ID");

                string input = Console.ReadLine();
                Player player = FindPlayer(input);

                if (player != null)
                {
                    System.Console.WriteLine($"Id: {player.Id}\tName: {player.Name}\tExperience: {player.Experience}\tHealth: {player.Health}");
                    searchDone = true;
                }
                else
                {
                    System.Console.WriteLine("The player does not exist");
                }

            }
            else
            {
                System.Console.WriteLine("There are no players");
                searchDone = true;
            }
        }
    }


    public void SearchInventory()
    {
    Console.WriteLine("Search by Name: ");
    string input = Console.ReadLine();
     
    var items = _context.Items.Where(item => item.Name.Contains(input));    
               
        foreach (var item in items)
        {
            Console.WriteLine($"Item: {item.Name}\tType:{item.Type}");
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

    public void ChooseItems()
    {
        Console.WriteLine("Select a Player Id: ");
        int playerIdSelection = Convert.ToInt32(Console.ReadLine());

        var playerSelection = _context.Players.Where(p => p.Id == playerIdSelection).FirstOrDefault();

        if (playerSelection == null)
        {
        }
        else
        {
            AddItem("Weapon", playerIdSelection);
            AddItem("Armor", playerIdSelection);
        }
    }

    public void AddItem(string itemType , int playerId)
    {
        bool addItem;

        var query = from p in _context.Players
            join i in _context.Items on p.Id equals i.PlayerId into itemGroup
            from i in itemGroup.DefaultIfEmpty()
            select new { player = p, item = i };
        
        var itemLookup = from q in query
                   where q.item.Type == itemType && q.item.PlayerId == playerId
                   select q;

        
        if (!itemLookup.Any()) // Check if itemLookup is empty
        {
            Console.WriteLine($"You have no items of type {itemType}");
            addItem = true;
        }
        else
        {
            Console.WriteLine($"Do you want to add another item of type {itemType}? (Y/N)");
            var input = Console.ReadLine().ToUpper()[0];

            if (input == 'Y')
            {
                Console.WriteLine($"You've chosen to add an additional {itemType}.");
                addItem = true;
            }
            else
            {
                Console.WriteLine($"You've chosen not to add an additional {itemType}.");
                addItem = false;
            }

        }

    bool itemNotAdded = true;
    while (itemNotAdded)
    {
        if (addItem)
            {
                Console.WriteLine($"Let's add an item of type {itemType}");
                Console.WriteLine($"Name the {itemType} you'd like to add: ");
                var itemToAdd = Console.ReadLine();

                var finditem = from i in _context.Items
                    where i.Name == itemToAdd
                    select i;

                var itemsOfItemType = from f in finditem
                        where f.Type == itemType
                        select f;
                
                var itemHasNoPlayer = from f in itemsOfItemType
                        where f.PlayerId == null
                        select f;      

                
                    // Check the item exists
                    if (finditem.Any())
                    {
                        // Check if item is the correct type
                        if (itemsOfItemType.Any())
                            {
                                // Check it item is already taken
                                if (itemHasNoPlayer.Any())
                                {
                                    System.Console.WriteLine($"The {itemType} {itemToAdd} is available.");
                                    
                                    var itemHasNoPlayerFirst = itemHasNoPlayer.FirstOrDefault();
                                    itemHasNoPlayerFirst.PlayerId = playerId;

                                    UpdateItem(itemHasNoPlayerFirst);
                                    itemNotAdded = false;
                                }
                                else
                                {
                                    System.Console.WriteLine($"{itemToAdd} is already taken.");
                                }
                        }
                        else
                        {
                            System.Console.WriteLine($"The item {itemToAdd} is not a valid {itemType}");
                        }

                    }
                    else
                    {
                        System.Console.WriteLine($"That is not a valid item name.");
                    }
            } 
        else
        {
            itemNotAdded = false;
        }
    }

    }
    
    private void AttackCharacter()
    {
        if (_goblin is ITargetable targetableGoblin)
        {
            int itemIdForAttack = _player.Attack(targetableGoblin);
            _player.UseAbility(_player.Abilities.First(), targetableGoblin);

            if (itemIdForAttack == 0)
            {
                Console.WriteLine("There was no attack, so no items to remove.");
            }
            else
            {
                var itemForAttack = getItem(itemIdForAttack);
                itemForAttack.PlayerId = null;
                UpdateItem(itemForAttack);
            }

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

    public void UpdateItem(Item item)
    {
        _context.Items.Update(item);
        _context.SaveChanges();
    }

    public void DeleteItem(Item item)
    {
        _context.Items.Remove(item);
        _context.SaveChanges();
    }

    public Item getItem(int id)
    {
        var retrievedItem = _context.Items.Where(i=> i.Id == id).FirstOrDefault();
        return retrievedItem;
    }

    public void CreateNewPlayer()
    {
        Console.WriteLine("Enter player NAME: ");
        string newPlayerName = Console.ReadLine();

        var newPlayer = new Player
                {
                    Name = newPlayerName,
                    Experience = 0,
                    Health = 100
                };

        AddPlayer(newPlayer);
        
    }

    public Player GetCheatingPlayer()
    {
        bool invalidPlayer = true;

        while (invalidPlayer)
        {
            System.Console.WriteLine("Select a player by ID or Name: ");
            string playerSearchInput = Console.ReadLine();

            Player cheatingPlayer = FindPlayer(playerSearchInput);

            if (cheatingPlayer != null)
            {
                System.Console.WriteLine($"You've chosen to cheat using player {cheatingPlayer.Name}");
                invalidPlayer = false;
                return cheatingPlayer;
            } 
            else
            {
                System.Console.WriteLine("That Player does not exist.");
            }
        }

        return null;
    }

    public void CheatMode()
    {

        Player cheatingPlayer = GetCheatingPlayer();

        Console.WriteLine("How would you like to cheat?:\n    1. Increase Experience\n    2. Increase Health");

        string cheatInput = Console.ReadLine();

        switch (cheatInput)
        {
            case "1":
                Console.WriteLine("Let's increase your experience points");
                IncreasePlayerExperiencePoints(cheatingPlayer);
                break;
            case "2":
                Console.WriteLine("Let's increase your health points");
                IncreasePlayerHealthPoints(cheatingPlayer);
                break;
            default:
                Console.WriteLine("Invalid Selection");
                break;
        }

    }

    public void IncreasePlayerExperiencePoints(Player player)
    {
        player.Experience += 5;
        UpdatePlayer(player);
    }

    public void IncreasePlayerHealthPoints(Player player)
    {
        player.Health += 5;
        UpdatePlayer(player);
    }

    public void UpdatePlayer(Player player)
    {
        _context.Players.Update(player);
        _context.SaveChanges();
    }

    public void AddPlayer(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
    }

    public Player FindPlayer(string search)
    {
        // If user entered a number, assume it's the ID,
        // Else assume it's the Name
        if (int.TryParse(search, out int result))
        {
            System.Console.WriteLine("Let's Search by Player ID");
            Player player = _context.Players.Where(p => p.Id == result).FirstOrDefault();
            return player;
        }
        else
        {
            System.Console.WriteLine("Let's Search by Player Name");
            Player player = _context.Players
                .ToList()
                .FirstOrDefault(p => p.Name.Equals(search, StringComparison.Ordinal));
            return player;

        }
    }

}
