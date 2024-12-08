using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpgEntities.Repositories
{
    public class RoomRepository
    {
        private readonly GameContext _context;

        public RoomRepository(GameContext context)
        {
            _context = context;
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
    }
}