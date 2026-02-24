using MessengerAvalonia.Shared.ChatsGrpc;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IChatsService
    {
        /// <summary>
        /// Попытка получить сообщения в чате для пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="friendslogin">Логин добавляемого друга</param>
        /// <returns>Ответ от сервера</returns>
        Task<GetChatsResponse> GetMessages(string login, string password, string friendsLogin);

        /// <summary>
        /// Попытка отправить сообщение другу в чате для пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="friendslogin">Логин добавляемого друга</param>
        /// <param name="text">Текст сообщения</param>
        /// <returns>Ответ от сервера</returns>
        Task<AddMessageResponse> SendMessage(string login,
                                             string password,
                                             string friendslogin,
                                             string text);
    }
}
