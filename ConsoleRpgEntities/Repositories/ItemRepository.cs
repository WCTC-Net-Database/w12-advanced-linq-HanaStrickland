using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Rooms;
using Microsoft.EntityFrameworkCore;

namespace ConsoleRpgEntities.Repositories
{
    public class ItemRepository
    {
        private readonly GameContext _context;
        private readonly PlayerRepository _playerRepository;
        private readonly RoomRepository _roomRepository;

        public ItemRepository(GameContext context, PlayerRepository playerRepository, RoomRepository roomRepository)
        {
            _context = context;
            _playerRepository = playerRepository;
            _roomRepository = roomRepository;
        }

        // CREATE
        public void AddItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
        }


        // READ
        public Item GetItemById(int id)
        {
            return _context.Items.FirstOrDefault(i => i.Id == id);
        }

         public Item FindItem(string search)
        {
            // If user entered a number, assume it's the ID,
            // Else assume it's the Name
            if (int.TryParse(search, out int result))
            {
                Item item = _context.Items.Where(p => p.Id == result).FirstOrDefault();
                return item;
            }
            else
            {
                Item item = _context.Items
                    .ToList()
                    .FirstOrDefault(p => p.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
                return item;

            }
        }
        public Item GetValidItem()
        {
            bool invalid = true;

            while (invalid)
            {
                System.Console.WriteLine("Select an item by ID or Name: ");
                string searchInput = Console.ReadLine();

                Item toValidate = FindItem(searchInput);

                if (toValidate != null)
                {
                    System.Console.WriteLine($"You've chosen item {toValidate.Name}");
                    invalid = false;
                    return toValidate;
                } 
                else
                {
                    System.Console.WriteLine("That item does not exist.");
                }
            }

            return null;
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

        public void SortByName()
        {
            var allItems = _context.Items.OrderBy(i => i.Name);
            foreach (var item in allItems)
            {
                Console.WriteLine($"\n{item.Id}. {item.Name}");
            }
        }
        public void SortByAttackValue()
        {
            var allItems = _context.Items.OrderBy(i => i.Attack);
            foreach (var item in allItems)
            {
                Console.WriteLine($"\n{item.Name}\t{item.Attack}");
            }
        }
        public void SortByDefenseValue()
        {
            var allItems = _context.Items.OrderBy(i => i.Defense);
            foreach (var item in allItems)
            {
                Console.WriteLine($"\n{item.Name}\t{item.Defense}");
            }
        }

        // TODO: Find a specific piece of a equipment and list the associated character and location
        public void LocateItem()
        {
            Item item = GetValidItem();
            var itemCharacter = item.PlayerId;
            if (itemCharacter != null)
            {
                string playerId = itemCharacter.ToString();
                var player = _playerRepository.FindPlayer(playerId);
                var playerRoomId = player.RoomId;

                if (playerRoomId != null)
                {
                    var playerRoom = _roomRepository.GetRoomById((int)playerRoomId);
                    System.Console.WriteLine($"The item {item.Name} is being used by {player.Name} in room {playerRoom.Name}.");
                }
                else
                {
                    System.Console.WriteLine($"Player {player.Name} is not in a room.");
                }               
            }
            else
            {
                System.Console.WriteLine($"The item {item.Name} is not currently in use");
            }
        }

        public void PlayerItems(int id)
        {
            var query = from p in _context.Players
                join i in _context.Items on p.Id equals i.PlayerId into itemGroup
                from i in itemGroup.DefaultIfEmpty()
                select new { player = p, item = i };
            
            var itemLookup = from q in query
                    where q.item.PlayerId == id
                    select q;

            if (!itemLookup.Any())
            {
                System.Console.WriteLine("No items to list");
            }
            else
            {
                foreach (var item in itemLookup)
                {
                    System.Console.WriteLine($"Id: {item.item.Id}\tType: {item.item.Type}\t{item.item.Name}");
                }
            }
        }

        // UPDATE
        public void UpdateItem(Item item)
        {
            _context.Items.Update(item);
            _context.SaveChanges();
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

        // DELETE
        public void DeleteItem(Item item)
        {
            _context.Items.Remove(item);
            _context.SaveChanges();
        }

        // DELETE - ensure we are deleting the item in the database
        public void DeleteItemById(int id)
        {
            // MUST get existing item from database to delete it
            var item = GetItemById(id);
            DeleteItem(item);
        }
    }
}