using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models;
using Microsoft.EntityFrameworkCore;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Repositories;
using ConsoleRpgEntities.Services;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpg.Services
{
    public class GameEngine
    {
        private readonly GameContext _context;
        private readonly MenuManager _menuManager;
        private readonly OutputManager _outputManager;
        private readonly ItemRepository _itemRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly AbilitiesRepository _abilitiesRepository;
        private readonly RoomRepository _roomRepository;
        private readonly PlayerService _playerService;

        private Player _player;
        private IMonster _goblin;

        public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager, ItemRepository itemRepository, 
        PlayerRepository playerRepository, AbilitiesRepository abilitiesRepository, RoomRepository roomRepository, PlayerService playerService)
        {
            _menuManager = menuManager;
            _outputManager = outputManager;
            _context = context;
            _itemRepository = itemRepository;
            _playerRepository = playerRepository;
            _abilitiesRepository = abilitiesRepository;
            _roomRepository = roomRepository;
            _playerService = playerService;
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
                _outputManager.WriteLine("1. Make a Play");
                _outputManager.WriteLine("2. Make a Move");
                _outputManager.WriteLine("3. Manage Players");
                _outputManager.WriteLine("4. Manage Items");
                _outputManager.WriteLine("5. Manage Abilities");
                _outputManager.WriteLine("6. Manage Rooms");
                _outputManager.WriteLine("7. Cheat and Increase Stats");
                _outputManager.WriteLine("0. Quit");

                _outputManager.Display();

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        MakeAPlay();
                        break;
                    case "2":
                        PlayerMove();
                        break;
                    case "3":
                        ManagePlayers();
                        break;
                    case "4":
                        ManageItems();
                        break;
                    case "5":
                        ManageAbilities();
                        break;
                    case "6":
                        ManageRooms();
                        break;
                    case "7":
                        CheatMode();
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
        private void LoadMonsters()
        {
            _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
        }

        private void SetupGame()
        {
            _player = _context.Players.FirstOrDefault();
            _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);
            _playerRepository.AddPlayerToStartRoomSunlitClearing(_player);

            // Load monsters into random rooms 
            LoadMonsters();

            // Pause before starting the game loop
            Thread.Sleep(500);
            GameLoop();
        }

        private void PlayerMove()
        {
            bool moving = true;

            while (moving)
            {

                int playerCurrentRoomId = _playerRepository.GetPlayerCurrentRoomId(_player);

                var currentRoom = _roomRepository.GetRoomById(playerCurrentRoomId);
                System.Console.WriteLine($"Player is currently in room {currentRoom.Name}");

                Room? nextRoom = null;

                while (nextRoom == null)
                {
                    System.Console.WriteLine("Which direction do you want to move?");
                    System.Console.WriteLine("N. North\nS. South\nE. East\nW. West\nQ. Stop Moving");
                    char input = Console.ReadLine().ToUpper()[0];

                if (input == 'N')
                {
                    bool exists = _roomRepository.CheckNorthDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'N');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move North");
                    }

                }
                else if (input == 'S')
                {
                    bool exists = _roomRepository.CheckSouthDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'S');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move South");
                    }
                }
                else if (input == 'E')
                {
                    bool exists = _roomRepository.CheckEastDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'E');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move East");
                    }
                }
                else if (input == 'W')
                {
                    bool exists = _roomRepository.CheckWestDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'W');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move West");
                    }
                }
                else if (input == 'Q')
                {
                    moving = false;
                    break;
                }
                else
                {
                    System.Console.WriteLine("Invalid Input");
                }
                }
                
            }
        }

        private void MakeAPlay()
        {
            System.Console.WriteLine("Do you want to\n1. Attack\n2.Use Ability");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AttackCharacter();
                    break;
                case "2":
                    UseAbility();
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    break;
            }
        }
    
        private void AttackCharacter()
        {
            if (_goblin is ITargetable targetableGoblin)
            {
                _playerService.Attack(_player, targetableGoblin);
            }
        }

        private void UseAbility()
        {
            if (_goblin is ITargetable targetableGoblin)
            {
                _playerService.UseAbility(_player, targetableGoblin);
            }
        }

        public void ManageItems()
        {
            System.Console.WriteLine("Please select an option\n\t1. Sort Items\n\t2. Choose Items");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    SortItems();
                    break;
                case "2":
                    ChooseItems();
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    break;
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

        public void ManagePlayers()
        {
            System.Console.WriteLine("Please select an option\n\t1. Create a New Player\n\t2. Search Players\n\t3. Display All Players");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CreateNewPlayer();
                    break;
                case "2":
                    _playerRepository.SearchPlayers();
                    break;
                case "3":
                    _playerRepository.DisplayAllPlayers();
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    break;
            }
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
                    AddAbilityToPlayer();
                    break;
                case "2":
                    DisplayPlayerAbilities();
                    break;
                default:
                    System.Console.WriteLine("Invalid Selection");
                    break;
            }
        }
        public void AddAbilityToPlayer()
        {
            // Ask for player and validate player selection
            Player player =  _playerRepository.GetValidPlayer();

            // Provide list of valid abilities
            System.Console.WriteLine("Select an ability by ID to add to your player");
            _abilitiesRepository.DisplayAbilities();

            Ability selectedAbility = _abilitiesRepository.GetValidAbility();

            System.Console.WriteLine($"You've selected the ability {selectedAbility.Name}");
            _abilitiesRepository.AddPlayerAbilities(player, selectedAbility);
        }

        public void DisplayPlayerAbilities()
        {
            Player player =  _playerRepository.GetValidPlayer();

            var playerAbilitiesList = player.Abilities.ToList();

            foreach (var ability in playerAbilitiesList)
            {
                System.Console.WriteLine(ability.Name);
            }
        }

        public void ManageRooms()
        {
            System.Console.WriteLine("Please select an option:");
            System.Console.WriteLine("\t1. Add a New Room");
            System.Console.WriteLine("\t2. Connect Rooms");
            System.Console.WriteLine("\t3. Display Room Details");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddNewRoom();
                    break;
                case "2":
                    ConnectRoom();
                    break;
                case "3":
                    DisplayRoomDetails();
                    break;
                default:
                    System.Console.WriteLine("Invalid Selection");
                    break;
            }
        }
        public void AddNewRoom()
        {
            bool rewrite = true;

            while (rewrite)
            {
                System.Console.WriteLine("Enter New Room Name: ");
                string newRoomName = Console.ReadLine();
                System.Console.WriteLine("Enter New Room Description: ");
                string newRoomDescription = Console.ReadLine();
                System.Console.WriteLine("Save this room? (Y/N)");
                char saveRoomInput = Console.ReadLine().ToUpper()[0];
                if (saveRoomInput == 'Y')
                {       
                var newRoom = new Room
                {
                    Name = newRoomName,
                    Description = newRoomDescription
                };

                _roomRepository.AddRoom(newRoom);
                rewrite = false;
                } 
            }

        }

        public void ConnectRoom()
        {
            System.Console.WriteLine("Let's connect two rooms!");
            System.Console.WriteLine("What room would you like to be the start room?");
            Room startRoom = _roomRepository.GetValidRoom();
            System.Console.WriteLine("What room would you like to connect to?");
            Room connectRoom = _roomRepository.GetValidRoom();

            System.Console.WriteLine($"How would you like to connect from {startRoom.Name} to {connectRoom.Name}? (North/South/East/West)");
            char direction = Console.ReadLine().ToUpper()[0];

            bool connectRooms = TryConnectRooms(startRoom, connectRoom, direction);

            if (!connectRooms)
            {
                System.Console.WriteLine($"{startRoom.Name} and {connectRoom.Name} cannot be connected.");
            }

        }
     
        private bool TryConnectRooms(Room startRoom, Room connectRoom, char direction)
        {
            switch (direction)
            {
                case 'N':
                    if (_roomRepository.CheckNorthDirection(startRoom.Id) || _roomRepository.CheckSouthDirection(connectRoom.Id))
                        return false;
                    startRoom.NorthId = connectRoom.Id;
                    connectRoom.SouthId = startRoom.Id;
                    break;
                case 'S':
                    if (_roomRepository.CheckSouthDirection(startRoom.Id) || _roomRepository.CheckNorthDirection(connectRoom.Id))
                        return false;
                    startRoom.SouthId = connectRoom.Id;
                    connectRoom.NorthId = startRoom.Id;
                    break;
                case 'E':
                    if (_roomRepository.CheckEastDirection(startRoom.Id) || _roomRepository.CheckWestDirection(connectRoom.Id))
                            return false;
                        startRoom.EastId = connectRoom.Id;
                        connectRoom.WestId = startRoom.Id;
                        break;
                case 'W':
                    if (_roomRepository.CheckWestDirection(startRoom.Id) || _roomRepository.CheckEastDirection(connectRoom.Id))
                            return false;
                        startRoom.WestId = connectRoom.Id;
                        connectRoom.EastId = startRoom.Id;
                        break;
                
            }
            _roomRepository.UpdateRoom(startRoom);
            _roomRepository.UpdateRoom(connectRoom);
            return true;
        }

        public void DisplayRoomDetails()
        {

        }
    }
}
