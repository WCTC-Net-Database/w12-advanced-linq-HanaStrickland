using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Characters.Monsters;

namespace ConsoleRpgEntities.Repositories
{
    public class MonsterRepository
    {
        private readonly GameContext _context;

        public MonsterRepository(GameContext context)
        {
            _context = context;
        }

        // CREATE

        // READ

        // UPDATE
        public void UpdateMonster(Monster monster)
        {
            _context.Monsters.Update(monster);
            _context.SaveChanges();
        }

        // DELETE
    }
}