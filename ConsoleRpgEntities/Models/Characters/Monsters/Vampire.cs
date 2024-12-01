using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Vampire : Monster
    {
        public int SupernaturalSpeed { get; set; }

        public override void Attack(ITargetable target)
        {
            // Goblin-specific attack logic
            Console.WriteLine($"{Name} uses supernatural speed and attacks {target.Name}!");
        }
    }
}
