using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models;
using Microsoft.EntityFrameworkCore;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Repositories;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;
    private readonly ItemRepository _itemRepository;
    private readonly PlayerRepository _playerRepository;
    private readonly AbilitiesRepository _abilitiesRepository;

    private IPlayer _player;
    private IMonster _goblin;

    public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager, ItemRepository itemRepository, PlayerRepository playerRepository, AbilitiesRepository abilitiesRepository)
    {
        _menuManager = menuManager;
        _outputManager = outputManager;
        _context = context;
        _itemRepository = itemRepository;
        _playerRepository = playerRepository;
        _abilitiesRepository = abilitiesRepository;
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
            _outputManager.WriteLine("8. Manage Abilities");
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
                    CheatMode();
                    break;
                case "8":
                    ManageAbilities();
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
                Player player = _playerRepository.FindPlayer(input);

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
                _itemRepository.SortByName();
                break;
            case "2":
                Console.WriteLine("Sort by Attack Value");
                _itemRepository.SortByAttackValue();
                break;
            case "3":
                Console.WriteLine("Sort by Defense Vaue");
                _itemRepository.SortByDefenseValue();
                break;
            default:
                Console.WriteLine("Invalid Selection");
                break;
        }
    }

    public void ChooseItems()
    {
        Player player = _playerRepository.GetValidPlayer();
        Console.WriteLine("Select a Player Id: ");
        int playerIdSelection = player.Id;

        _itemRepository.AddItem("Weapon", playerIdSelection);
        _itemRepository.AddItem("Armor", playerIdSelection);
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
                var itemForAttack = _itemRepository.GetItemById(itemIdForAttack);
                itemForAttack.PlayerId = null;
                _itemRepository.UpdateItem(itemForAttack);
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

        _playerRepository.AddPlayer(newPlayer);
        
    }


    public void CheatMode()
    {

        Player cheatingPlayer = _playerRepository.GetValidPlayer();

        Console.WriteLine("How would you like to cheat?:\n    1. Increase Experience\n    2. Increase Health");

        string cheatInput = Console.ReadLine();

        switch (cheatInput)
        {
            case "1":
                Console.WriteLine("Let's increase your experience points");
                _playerRepository.IncreasePlayerExperiencePoints(cheatingPlayer);
                break;
            case "2":
                Console.WriteLine("Let's increase your health points");
                _playerRepository.IncreasePlayerHealthPoints(cheatingPlayer);
                break;
            default:
                Console.WriteLine("Invalid Selection");
                break;
        }

    }


public void ManageAbilities()
    {
        Console.WriteLine("Please select an option: ");
        Console.WriteLine("    1. Add Abilities to Player");
        Console.WriteLine("    2. Display Player Abilities");

        string input = Console.ReadLine();

        switch (input)
        {
            case "1":
                _abilitiesRepository.AddPlayerAbilities();
                break;
            case "2":
                System.Console.WriteLine("Display");
                break;
            default:
                System.Console.WriteLine("Invalid Selection");
                break;
        }
    }
}
