using ConsoleRpgEntities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Repositories
{
    public class PlayerRepository
    {
        private readonly GameContext _context;

        public PlayerRepository(GameContext context)
        {
            _context = context;
        }

        // CREATE
        public void AddPlayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        // READ

        public Player FindPlayer(string search)
        {
            // If user entered a number, assume it's the ID,
            // Else assume it's the Name
            if (int.TryParse(search, out int result))
            {
                System.Console.WriteLine("Let's Search by ID");
                Player player = _context.Players.Where(p => p.Id == result).FirstOrDefault();
                return player;
            }
            else
            {
                System.Console.WriteLine("Let's Search by Name");
                Player player = _context.Players
                    .ToList()
                    .FirstOrDefault(p => p.Name.Equals(search, StringComparison.Ordinal));
                return player;

            }
        }
        public Player GetValidPlayer()
        {
            bool invalid = true;

            while (invalid)
            {
                System.Console.WriteLine("Select a player by ID or Name: ");
                string searchInput = Console.ReadLine();

                Player toValidate = FindPlayer(searchInput);

                if (toValidate != null)
                {
                    System.Console.WriteLine($"You've chosen player {toValidate.Name}");
                    invalid = false;
                    return toValidate;
                } 
                else
                {
                    System.Console.WriteLine("That Player does not exist.");
                }
            }

            return null;
        }

        // UPDATE

        public void UpdatePlayer(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        public void IncreasePlayerExperiencePoints(Player player)
        {
            player.Experience += 5;
            UpdatePlayer(player);
        }

        public void IncreasePlayerHealthPoints(Player player)
        {
            player.Health += 5;
            UpdatePlayer(player);
        }
    }
}