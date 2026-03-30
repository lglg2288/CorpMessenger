using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface ISecureStorage
    {
        Task SaveAsync(string key, string value);
        Task<string?> GetAsync(string key);
    }
}
