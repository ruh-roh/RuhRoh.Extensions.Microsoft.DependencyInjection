using System.Collections.Generic;
using System.Threading.Tasks;
using RuhRoh.Extensions.Microsoft.DependencyInjection.Tests.Models;

namespace RuhRoh.Extensions.Microsoft.DependencyInjection.Tests.Services
{
    public interface ITestServiceContract
    {
        IEnumerable<TestItem> GetItems();
        TestItem GetItemById(int id);

        Task<TestItem> GetItemByIdAsync(int id);
    }
}