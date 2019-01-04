using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.Repository
{
    public interface IClientRepository
    {
        IEnumerable<Client> Clients { get; }
        void SaveClient(Client client);
    }
}
