using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class StabAbility : Ability
    {
        public override void Activate(IPlayer user, ITargetable target)
        {
            Console.WriteLine($"{user.Name} stabs {target.Name}, dealing {Damage} damage!");
        }
    }
}
