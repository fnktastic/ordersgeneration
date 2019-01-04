using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly Context _context;

        public ClientRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Client> Clients => _context.Clients;

        public void SaveClient(Client client)
        {
            if (client.ID == 0)
            {
                _context.Clients.Add(client);
            }
            else
            {
                Client dbEntry = _context.Clients.FirstOrDefault(x => x.ID == client.ID);
                if (dbEntry != null)
                {
                    dbEntry.FirstName = client.FirstName;
                    dbEntry.SurnameName = client.SurnameName;
                    dbEntry.LastName = client.LastName;
                    dbEntry.Address = client.Address;
                    dbEntry.Description = client.Description;
                }
            }

            _context.SaveChanges();
        }
    }
}
