using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Repositories
{
    public class AbilitiesRepository
    {
        private readonly GameContext _context;
        private readonly PlayerRepository _playerRepository;

        public AbilitiesRepository(GameContext context, PlayerRepository playerRepository)
        {
            _context = context;
            _playerRepository = playerRepository;
        }

        // READ

        public void DisplayAbilities()
        {
            var abilities = _context.Abilities;

            foreach (var ability in abilities)
            {
                var id = ability.Id;
                var name = ability.Name;
                var damage = ability.Damage;
                Console.WriteLine($"Id: {id}\tName: {name}\tDamage: {damage}");
            }
        }

        public Ability FindAbility(string search)
        {
            // If user entered a number, assume it's the ID,
            // Else assume it's the Name
            if (int.TryParse(search, out int result))
            {
                Ability ability = _context.Abilities.Where(a => a.Id == result).FirstOrDefault();
                return ability;
            }
            else
            {
                Ability ability = _context.Abilities
                    .ToList()
                    .FirstOrDefault(p => p.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
                return ability;

            }
        }

        public Ability GetValidAbility()
        {
            bool invalid = true;

            while (invalid)
            {
                System.Console.WriteLine("Select an Ability by ID or Name: ");
                string searchInput = Console.ReadLine();

                Ability toValidate = FindAbility(searchInput);

                if (toValidate != null)
                {
                    System.Console.WriteLine($"You've chosen ability {toValidate.Name}");
                    invalid = false;
                    return toValidate;
                } 
                else
                {
                    System.Console.WriteLine("That ability does not exist.");
                }
            }

            return null;
        }


        // CREATE

        public void AddPlayerAbilities(Player player, Ability ability)
        {
            player.Abilities.Add(ability);
            _context.SaveChanges();
            System.Console.WriteLine($"Added Ability {ability.Name} for player {player.Name}");
        }

        // DELETE
        public void RemovePlayerAbilities(Player player, Ability ability)
        {
            player.Abilities.Remove(ability);
            _context.SaveChanges();
        }
        

    }
}