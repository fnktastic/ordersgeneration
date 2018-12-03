using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Repository
{
    public interface IPositionRepository
    {
        IEnumerable<Position> Positions { get; }

        Position RemovePosition(int positionID);
    }
}
