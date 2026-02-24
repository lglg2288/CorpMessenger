using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessengerAvalonia.Shared.LoginGrpc;

namespace Client.Services
{
    public interface ILoginService
    {
        /// <summary>
        /// Попытка входа в систему
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ответ от сервера</returns>
        Task<LoginResponse> SignInAsync(string login, string password);
    }
}