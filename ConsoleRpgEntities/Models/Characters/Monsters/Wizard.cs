using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Wizard : Monster
    {
        public int SpellMastery { get; set; }

        public override void Attack(ITargetable target)
        {
            // Goblin-specific attack logic
            Console.WriteLine($"{Name} uses spell mastery and attacks {target.Name}!");
        }
    }
}
