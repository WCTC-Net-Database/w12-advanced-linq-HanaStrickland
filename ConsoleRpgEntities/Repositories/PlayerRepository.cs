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
        // TODO: Add New Player to DB
        public void AddPlayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        // READ

        public void DisplayAllPlayers()
        {
            var players = _context.Players;

            foreach (var player in players)
            {
                System.Console.WriteLine($"Id: {player.Id}\tName: {player.Name}\tExperience: {player.Experience}\tHealth: {player.Health}");
            }
        }

        public void SearchPlayers()
        {
            var players = _context.Players;

            bool searchDone = false;

            while (!searchDone)
            {
                // check if there are any players in the table

                if (players.Any())
                {
                   System.Console.WriteLine("Enter player name or ID");

                    string input = Console.ReadLine();
                    Player player = FindPlayer(input);

                    if (player != null)
                    {
                        System.Console.WriteLine($"Id: {player.Id}\tName: {player.Name}\tExperience: {player.Experience}\tHealth: {player.Health}");
                        searchDone = true;
                    }
                    else
                    {
                        System.Console.WriteLine("The player does not exist");
                    }

                }
                else
                {
                    System.Console.WriteLine("There are no players");
                    searchDone = true;
                }
            }
        }

        public Player FindPlayer(string search)
        {
            // If user entered a number, assume it's the ID,
            // Else assume it's the Name
            if (int.TryParse(search, out int result))
            {
                Player player = _context.Players.Where(p => p.Id == result).FirstOrDefault();
                return player;
            }
            else
            {
                Player player = _context.Players
                    .ToList()
                    .FirstOrDefault(p => p.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
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

        public int GetPlayerCurrentRoomId(Player player)
        {
            var id = player.RoomId;

            return Convert.ToInt32(id);
        }

        // UPDATE

        public void UpdatePlayer(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        // TODO: Allow user to update experience points
        public void IncreasePlayerExperiencePoints(Player player)
        {
            player.Experience += 5;
            UpdatePlayer(player);
        }

        // TODO: Allow user to update health points
        public void IncreasePlayerHealthPoints(Player player)
        {
            player.Health += 5;
            UpdatePlayer(player);
        }

        public void AddPlayerToStartRoomSunlitClearing(Player player)
        {
            player.RoomId = 0;
            UpdatePlayer(player);
        }
    }
}