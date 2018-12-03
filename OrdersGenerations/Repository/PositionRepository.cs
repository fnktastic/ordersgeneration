using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Repository
{
    public class PositionRepository : IPositionRepository
    {
        private readonly Context _context;

        public PositionRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Position> Positions => _context.Positions.ToList();

        public Position RemovePosition(int positionID)
        {
            var dbEntry = _context.Positions.FirstOrDefault(x => x.ID == positionID);

            if(dbEntry != null)
            {
                _context.Positions.Remove(dbEntry);
                _context.SaveChanges();
            }

            return dbEntry;
        }
    }
}
