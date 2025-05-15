using demo.Models;
using demo.Repositories;
using System.Collections.Generic;

namespace demo.Services
{
    public class LoginService
    {
        private readonly AccountRepository _repo;

        public LoginService(AccountRepository repo)
        {
            _repo = repo;
        }

        public Account? GetLoginByUserName(string userName)
        {
            return _repo.GetByUserName(userName);
        }

    }
}
