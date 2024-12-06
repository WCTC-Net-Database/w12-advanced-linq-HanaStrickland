using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Abilities;
using ConsoleRpgEntities.Models;
using ConsoleRpgEntities.Repositories;

namespace ConsoleRpgEntities.Services
{
    public class PlayerService
    {
        private readonly ItemRepository _itemRepository;
        private readonly AbilitiesRepository _abilitiesRepository;

        public PlayerService(ItemRepository itemRepository, AbilitiesRepository abilitiesRepository)
        {
            _itemRepository = itemRepository;
            _abilitiesRepository = abilitiesRepository;
        }
        public void Attack(Player player, ITargetable target)
        {
            // Player-specific attack logic

            Item attackItem = player.Items.Where(i=>i.Type == "Weapon").FirstOrDefault();

            if (attackItem != null)
            {
                 if (target is Monster monster)
                {
                    Console.WriteLine($"{player.Name} attacks {target.Name} with a {attackItem.Name}");
                    monster.Health -= attackItem.Attack;
                    

                    attackItem.PlayerId = null;
                    _itemRepository.UpdateItem(attackItem);
                } 
                else
                {
                    System.Console.WriteLine("Target is not a monster");
                }
            }
            else
            {
                Console.WriteLine("You have no weapons for attacking");
            }
        }

         public void UseAbility(Player player, ITargetable target)
        {
            if (player.Abilities.Any())
            {
                var ability = player.Abilities.FirstOrDefault();

                ability.Activate(player, target);
                _abilitiesRepository.RemovePlayerAbilities(player, ability);                
            }
            else
            {
                System.Console.WriteLine("You have no abilities");
            }
        }
    }
}