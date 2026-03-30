using Client.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Desktop.Services
{
    public class DesktopSecureStorage : ISecureStorage
    {
        public async Task SaveAsync(string key, string value)
        {
            throw new ArgumentNullException("desktop secure storage caput");
        }
        public async Task<string?> GetAsync(string key)
        {
            throw new ArgumentNullException("desktop secure storage caput");
            return null;
        }
    }
}
