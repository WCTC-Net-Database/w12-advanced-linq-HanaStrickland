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
        private readonly MonsterRepository _monsterRepository;
        private readonly PlayerService _playerService;

        private Player _player;
        private Monster _goblin;
        private Monster _vampire;
        private Monster _wizard;

        private int _goblinSkip;
        private int _vampireSkip;
        private int _wizardSkip;

        public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager, ItemRepository itemRepository, 
        PlayerRepository playerRepository, AbilitiesRepository abilitiesRepository, RoomRepository roomRepository, MonsterRepository monsterRepository, PlayerService playerService)
        {
            _menuManager = menuManager;
            _outputManager = outputManager;
            _context = context;
            _itemRepository = itemRepository;
            _playerRepository = playerRepository;
            _abilitiesRepository = abilitiesRepository;
            _roomRepository = roomRepository;
            _monsterRepository = monsterRepository;
            _playerService = playerService;
            
            _goblinSkip = 0;
            _vampireSkip = 0;
            _wizardSkip = 0;
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
        private int NextRandomRoom()
        {
            Random rnd = new Random();
            int roomCount = _roomRepository.GetRoomCount();
            int randRoomInt = rnd.Next(0, roomCount);
            return randRoomInt;
        }

        private void LoadGoblin()
        {
            _goblin = _context.Monsters.OfType<Goblin>().Skip(_goblinSkip).FirstOrDefault();
            int randomRoomInt = NextRandomRoom();
            _goblin.RoomId = randomRoomInt;
            _monsterRepository.UpdateMonster(_goblin);
            _goblinSkip += 1;
        }
        private void LoadVampire()
        {
            _vampire = _context.Monsters.OfType<Vampire>().Skip(_vampireSkip).FirstOrDefault();
            int randomRoomInt = NextRandomRoom();
            _vampire.RoomId = randomRoomInt;
            _monsterRepository.UpdateMonster(_vampire);
            _vampireSkip +=1;
        }
        private void LoadWizard()
        {
            _wizard = _context.Monsters.OfType<Wizard>().Skip(_wizardSkip).FirstOrDefault();
            int randomRoomInt = NextRandomRoom();
            _wizard.RoomId = randomRoomInt;
            _monsterRepository.UpdateMonster(_wizard);
            _wizardSkip +=1;
        }
        
        private void LoadMonsters()
        {
            LoadGoblin();
            LoadVampire();
            LoadWizard();
        }

        private void SetupGame()
        {
            _player = _context.Players.FirstOrDefault();
            _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);
            _playerRepository.AddPlayerToStartRoomSunlitClearing(_player);

            // Load monsters into random rooms 
            LoadMonsters();

            // Pause before starting the game loop
            Thread.Sleep(100);
            GameLoop();
        }

        private List<Monster> GetMonsterList(int id)
        {
            var monsterList = _roomRepository.GetMonstersInASingleRoom(id);
            return monsterList;
        }

        private bool MonstersInCurrentRoomOptions(int roomId)
        {
            var monsterList = GetMonsterList(roomId);
            if (monsterList.Any())
            {
                foreach (var monster in monsterList)
                {
                    System.Console.WriteLine($"{monster.Id}. {monster.Name}");
                }
                return true;
            }
            else
            {
                System.Console.WriteLine("No Monster Options");
                return false;
            }
        }


        private string? MonsterRoomString(int roomId)
        {
            var monsterNames = new List<string>();
            var monsterList = GetMonsterList(roomId);

            if (monsterList.Any())
            {
                foreach (var monster in monsterList)
                {
                    monsterNames.Add(monster.Name);
                }

                    string stringOfNames = String.Join(',', monsterNames);
                    return stringOfNames;
            }
            else
            {
                return null;
            }                
        }

        private void PlayerMove()
        {
            bool moving = true;

            while (moving)
            {

                int playerCurrentRoomId = _playerRepository.GetPlayerCurrentRoomId(_player);

                var currentRoom = _roomRepository.GetRoomById(playerCurrentRoomId);
                System.Console.WriteLine($"Player is currently in room {currentRoom.Name}");
                // TODO: List Characters in Room
                string monstersInRoom = MonsterRoomString(playerCurrentRoomId);

                if (monstersInRoom is not null)
                {
                    System.Console.WriteLine($"Monsters Also In Room: {monstersInRoom}");
                }
                

                bool notValid = true;
                while (notValid)
                {
                    System.Console.WriteLine("Which direction do you want to move?");
                    System.Console.WriteLine("N. North\nS. South\nE. East\nW. West\nQ. Stop Moving");
                    char direction = Console.ReadLine().ToUpper()[0];
                    notValid = TryMoving(direction, playerCurrentRoomId);
                    if (direction == 'Q')
                    {
                        moving = false;
                    }
                }
            }
        }
        
        private bool TryMoving(char direction, int playerCurrentRoomId)
        {
            switch (direction)
            {
                case 'N':
                    bool exists = _roomRepository.CheckNorthDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        Room nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'N');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move North");
                        return true;
                    }
                case 'S':
                    exists = _roomRepository.CheckSouthDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        Room nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'S');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move South");
                        return true;
                    }
                case 'E':
                    exists = _roomRepository.CheckEastDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        Room nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'E');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move East");
                        return true;
                    }
                case 'W':
                    exists = _roomRepository.CheckWestDirection(playerCurrentRoomId);

                    if (exists)
                    {
                        Room nextRoom = _roomRepository.GetNextRoomByCurrentRoomId(playerCurrentRoomId, 'W');
                        _player.RoomId = nextRoom.Id;
                        _playerRepository.UpdatePlayer(_player);
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine("You Can't Move West");
                        return true;
                    }
                case 'Q':
                    return false;
                default:
                    System.Console.WriteLine("Invalid Selection");
                    return true;
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
    
        
        private Monster ElligibleMonster()
        {
            
            int playerCurrentRoomId = _playerRepository.GetPlayerCurrentRoomId(_player);
            bool monstersExistInCurrentRoom = MonstersInCurrentRoomOptions(playerCurrentRoomId);
            
            if (monstersExistInCurrentRoom)
            {
                System.Console.WriteLine("Select a monster to target.");
                while (true)

                {
                    int monsterChoice = Convert.ToInt16(Console.ReadLine());
                    Monster singleMonster = _roomRepository.CheckMonsterInRoom(monsterChoice, playerCurrentRoomId);

                    if (singleMonster != null)
                    {
                        return singleMonster;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private void AttackCharacter()
        {
            var attackMonster = ElligibleMonster();

            if (attackMonster != null)
            {
                if (attackMonster is ITargetable targetableMonster)
                {
                    _playerService.Attack(_player, targetableMonster);

                    int randomRoomInt;

                    if (attackMonster is Goblin)
                    {
                        LoadGoblin();
                    }

                    if (attackMonster is Vampire)
                    {
                        LoadVampire();
                    }

                    if (attackMonster is Wizard)
                    {
                        LoadWizard();
                    }
                }
            }
        }

        private void UseAbility()
        {
            var attackMonster = ElligibleMonster();

            if (attackMonster != null)
            {
                if (attackMonster is ITargetable targetableMonster)
                {
                    _playerService.UseAbility(_player, targetableMonster);
                }
            }
        }

        public void ManageItems()
        {
            System.Console.WriteLine("Please select an option");
            Console.WriteLine("1. Sort Items");
            Console.WriteLine("2. Choose Items");
            Console.WriteLine("3. Locate an Item"); // TODO: Find a specific piece of a equipment and list the associated character and location
            Console.WriteLine("4. List Player Items");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    SortItems();
                    break;
                case "2":
                    ChooseItems();
                    break;
                case "3":
                    Console.WriteLine("Pick an Item to Locate");
                    _itemRepository.LocateItem();
                    Thread.Sleep(500);
                    break;
                case "4":
                    _itemRepository.PlayerItems(_player.Id);
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
                    _playerRepository.SearchPlayers(); // TODO: Search for a specific character ignoring upper/lower case
                    break;
                case "3":
                    _playerRepository.DisplayAllPlayers(); // TODO: Display All Characters
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    break;
            }
        }

        // TODO: Add a New Character
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

        // TODO: Edit an Existing Character
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
                    AddAbilityToPlayer(); // TODO: Add Abilities to Player
                    break;
                case "2":
                    DisplayPlayerAbilities(); // TODO: Display Player Abilities
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
                    AddNewRoom(); // TODO: Add new Room
                    break;
                case "2":
                    ConnectRoom(); // TODO: Navigate Rooms
                    break;
                case "3":
                    DisplayRoomDetails(); // TODO: Display ROOM Details
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

        public void RoomDetails()
        {
           var query = from r in _context.Room
                join m in _context.Monsters on r.Id equals m.RoomId into roomMonster
                from m in roomMonster.DefaultIfEmpty()
                select new { room = r, monster = m };

            foreach (var room in query)
            {
                if (room.monster != null)
                {
                    System.Console.WriteLine($"{room.room.Id}. {room.room.Name}\t{room.monster.Name}");
                }
                else
                {
                    System.Console.WriteLine($"{room.room.Id}. {room.room.Name}\tNo Monsters");
                }
                
            }
        }

        public void DisplayRoomDetails()
        {
            RoomDetails();
        }
    }
}
