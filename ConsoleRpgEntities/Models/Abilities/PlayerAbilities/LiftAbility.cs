using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class LiftAbility : Ability
    {
        public override void Activate(IPlayer user, ITargetable target)
        {
            Console.WriteLine($"{user.Name} lifts and drops {target.Name} {Metric} feet, dealing {Damage} damage!");
        }
    }
}
