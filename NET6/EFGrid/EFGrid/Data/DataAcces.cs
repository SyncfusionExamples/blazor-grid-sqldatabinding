using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EFGrid.Data;


namespace EFGrid.Data
{
    public class DataAccess: IDataAccess
    {
        public HttpClient _client { get; set; }

        public List<Order> emplist { get; set; }

        public DataAccess(HttpClient client)
        {
            this._client = client;
        }

        public async Task<List<Order>> GetAllRecords()
        {
            await GetOrders();
            return emplist;
        }

        public async Task GetOrders()
        {
          //  emplist = await _client.GetJsonAsync<Order[]>("api/Default");
        }
    }
}
