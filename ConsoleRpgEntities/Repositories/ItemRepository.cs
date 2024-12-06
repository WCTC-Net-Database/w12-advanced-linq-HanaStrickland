using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models;

namespace ConsoleRpgEntities.Repositories
{
    public class ItemRepository
    {
        private readonly GameContext _context;

        public ItemRepository(GameContext context)
        {
            _context = context;
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

        public void SortByName()
        {
            var allItems = _context.Items.OrderBy(i => i.Name);
            foreach (var item in allItems)
            {
                Console.WriteLine($"\n{item.Name}");
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