using MessengerAvalonia.Shared.FriendsGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IFriendService
    {
        /// <summary>
        /// Попытка получить список друзей для пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ответ от сервера</returns>
        Task<FriendsResponse> GetFriendsAsync(string login, string password);

        /// <summary>
        /// Попытка добавить друга для пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="friendslogin">Логин добавляемого друга</param>
        /// <returns>Ответ от сервера</returns>
        Task<AddFriendResponse> AddFriendAsync(string login, string password, string friendsLogin);
    }
}
