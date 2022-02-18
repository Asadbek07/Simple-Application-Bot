using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBot
{
    public class UserData
    {
        public long ChatId { get; set; }    
        public string UserName { get; set; }    
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRegisterState State { get; set; }
        public UserData(long chatId, string userName)
        {
            ChatId = chatId;
            UserName = userName == "Empty" ? "Empty" : $"{userName}";
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Password = string.Empty;
            State = UserRegisterState.None;

        }
    
    }
}
