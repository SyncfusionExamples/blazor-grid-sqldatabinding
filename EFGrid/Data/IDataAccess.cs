using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EFGrid.Data;

namespace EFGrid.Data
{
    public interface IDataAccess
    {
        Task<List<Order>> GetAllRecords();
    }
}
