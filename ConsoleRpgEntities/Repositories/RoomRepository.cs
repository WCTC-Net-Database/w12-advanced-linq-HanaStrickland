using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Rooms;
using System.Diagnostics.Metrics;
using ConsoleRpgEntities.Models.Characters.Monsters;

namespace ConsoleRpgEntities.Repositories
{
    public class RoomRepository
    {
        private readonly GameContext _context;

        public RoomRepository(GameContext context)
        {
            _context = context;
        }

        // CREATE
        public void AddRoom(Room room)
        {
            _context.Room.Add(room);
            _context.SaveChanges();
            System.Console.WriteLine($"Created a new room called {room.Name}");
        }
        
        // READ

        public Room GetRoomById(int id)
        {
            return _context.Room.FirstOrDefault(i=> i.Id == id);
        }

        public Room GetNextRoomByCurrentRoomId(int id, char direction)
        {
            Room currentRoom = GetRoomById(id);
            int? nextRoomId = null;

            if (direction == 'N')
            {
                nextRoomId = currentRoom.NorthId;
            }

            if (direction == 'S')
            {
                nextRoomId = currentRoom.SouthId;
            }

            if (direction == 'E')
            {
                nextRoomId = currentRoom.EastId;
            }

            if (direction == 'W')
            {
                nextRoomId = currentRoom.WestId;
            }

            if (nextRoomId.HasValue)
            {
                Room nextRoom = GetRoomById(nextRoomId.Value);
                return nextRoom;
            }
            else
            {
                return null;
            }
        }

        public bool CheckNorthDirection(int id)
        {
            var checkRoom = GetRoomById(id);
            var room = checkRoom.NorthId;

            if (room != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckSouthDirection(int id)
        {
            var checkRoom = GetRoomById(id);
            var room = checkRoom.SouthId;

            if (room != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckEastDirection(int id)
        {
            var checkRoom = GetRoomById(id);
            var room = checkRoom.EastId;

            if (room != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckWestDirection(int id)
        {
            var checkRoom = GetRoomById(id);
            var room = checkRoom.WestId;

            if (room != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Room FindRoom(string search)
        {
            // If user entered a number, assume it's the ID,
            // Else assume it's the Name
            if (int.TryParse(search, out int result))
            {
                System.Console.WriteLine("Let's Search by ID");
                Room room = _context.Room.Where(p => p.Id == result).FirstOrDefault();
                return room;
            }
            else
            {
                System.Console.WriteLine("Let's Search by Name");
                Room room = _context.Room
                    .ToList()
                    .FirstOrDefault(p => p.Name.Equals(search, StringComparison.Ordinal));
                return room;

            }
        }
        public Room GetValidRoom()
        {
            bool invalid = true;

            while (invalid)
            {
                System.Console.WriteLine("Select a Room by ID or Name: ");
                string searchInput = Console.ReadLine();

                Room toValidate = FindRoom(searchInput);

                if (toValidate != null)
                {
                    System.Console.WriteLine($"You've chosen Room {toValidate.Name}");
                    invalid = false;
                    return toValidate;
                } 
                else
                {
                    System.Console.WriteLine("That Room does not exist.");
                }
            }

            return null;
        }

        public int GetRoomCount()
        {
            int counter = 0;

            var allRooms = _context.Room.ToList();

            foreach (var a_room in allRooms)
            {
                counter += 1;
            }

            return counter;
        }

        public IQueryable<Monster> GetMonstersInRooms()
        {
            var monsters = _context.Room
            .SelectMany(room => room.Monsters);

            return monsters;
        }

        public List<Monster> GetMonstersInASingleRoom(int _roomId)
        {
            var monsterQueryable = GetMonstersInRooms();
            var monsters = monsterQueryable.Where(i=>i.RoomId == _roomId).ToList();
            return monsters;
        }

        // Update
        public void UpdateRoom(Room room)
        {
            _context.Room.Update(room);
            _context.SaveChanges();
        }
    }
}