//using NagypapaHazaiBlazor.MODELS;

//namespace NagypapaHazaiBlazor.Services
//{
//    public class AuthState
//    {
//        public User? CurrentUser { get; private set; }

//        public bool IsLoggedIn => CurrentUser != null;

//        public void SetUser(User? user)
//        {
//            CurrentUser = user;
//        }

//        public void Logout()
//        {
//            CurrentUser = null;
//        }
//    }
//}

using Microsoft.AspNetCore.Http;
using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazaiBlazor.Services
{
    public class AuthState
    {
        private readonly IHttpContextAccessor _http;

        public AuthState(IHttpContextAccessor http)
        {
            _http = http;
        }

        private ISession? Session
        {
            get
            {
                try { return _http.HttpContext?.Session; }
                catch { return null; }
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                try { return Session?.GetInt32("UserId") != null; }
                catch { return false; }
            }
        }

        public User? CurrentUser
        {
            get
            {
                try
                {
                    var id = Session?.GetInt32("UserId");
                    if (id == null) return null;
                    return new User
                    {
                        Id = id.Value,
                        UserName = Session?.GetString("UserName") ?? "",
                        Email = Session?.GetString("Email") ?? "",
                        PasswordHash = ""
                    };
                }
                catch { return null; }
            }
        }

        public void SetUser(User? user)
        {
            try
            {
                if (user == null)
                {
                    Session?.Remove("UserId");    
                    Session?.Remove("UserName");  
                    Session?.Remove("Email");     
                }
                else
                {
                    Session?.SetInt32("UserId", user.Id);
                    Session?.SetString("UserName", user.UserName);
                    Session?.SetString("Email", user.Email);
                }
            }
            catch { }
        }

    }
}
