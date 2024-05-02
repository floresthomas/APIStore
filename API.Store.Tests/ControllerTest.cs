using API.Store.Controllers;
using API.Store.Services;
using API.Store.Services.Interfaces;
using API.StoreShared;

namespace API.Store.Tests
{
    public class ControllerTest
    {
        private readonly ICrudService<Client> _clientService;
        private readonly ClientsController _clientController;

        public ControllerTest()
        {
            _clientService = new ClientService();
            _clientController = new ClientsController(_clientService);
        }

        [Fact]
        public void TestClientGetAll()
        {

        }
    }
}