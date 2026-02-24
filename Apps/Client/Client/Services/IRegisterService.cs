using MessengerAvalonia.Shared.LoginGrpc;
using MessengerAvalonia.Shared.RegisterGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IRegisterService
    {
        /// <summary>
        /// Попытка регистрации в системе
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ответ от сервера</returns>
        Task<RegisterResponse> RegisterAsync(string login, string password);
    }
}