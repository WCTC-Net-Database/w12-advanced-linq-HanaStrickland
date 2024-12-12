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
        private readonly MonsterRepository _monsterRepository;

        public PlayerService(ItemRepository itemRepository, AbilitiesRepository abilitiesRepository, MonsterRepository monsterRepository)
        {
            _itemRepository = itemRepository;
            _abilitiesRepository = abilitiesRepository;
            _monsterRepository = monsterRepository;
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
                    monster.RoomId = null;
                    _monsterRepository.UpdateMonster(monster);
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

         public void UseAbility(Player player, ITargetable target) // TODO: Execute Ability in an Attack
        {
            if (player.Abilities.Any())
            {
                var ability = player.Abilities.FirstOrDefault();

                ability.Activate(player, target);
                var damage = ability.Damage;
                _abilitiesRepository.RemovePlayerAbilities(player, ability);
                target.Health -= damage;

                if (target is Monster monster)

                {
                    _monsterRepository.UpdateMonster(monster);
                }
            }
            else
            {
                System.Console.WriteLine("You have no abilities");
            }
        }
    }
}