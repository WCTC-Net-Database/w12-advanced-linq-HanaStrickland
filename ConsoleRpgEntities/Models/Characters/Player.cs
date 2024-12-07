using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using ConsoleRpgEntities.Models;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Rooms;


namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }

        // Navigation properties
        public virtual Room Room { get; set; }
        public virtual int? RoomId { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }

    }
}
