using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazaiBlazor.Services
{
    public class AuthState
    {
        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public void SetUser(User? user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
