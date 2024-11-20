using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using ConsoleRpgEntities.Models;
using ConsoleRpgEntities.Models.Characters.Monsters;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }

        // Foreign key
        public int? EquipmentId { get; set; }

        // Navigation properties
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }

        public void Attack(ITargetable target)
        {
            // Player-specific attack logic

            Item attackItem = Items.Where(i=>i.Attack != null).FirstOrDefault(); // only retrive item with Attack value

            if (attackItem != null)
            {
                Console.WriteLine($"{Name} attacks {target.Name} with a {attackItem.Name}");

                if (this is Player && target is Goblin goblin)
                {
                    goblin.Health -= attackItem.Attack;
                }   
            }
            else
            {
                Console.WriteLine("You have no weapons for attacking");
            }

        }

        public void UseAbility(IAbility ability, ITargetable target)
        {
            if (Abilities.Contains(ability))
            {
                ability.Activate(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} does not have the ability {ability.Name}!");
            }
        }
    }
}
