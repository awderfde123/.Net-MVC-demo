using demo.Models;
using demo.Repositories;
using System.Collections.Generic;

namespace demo.Services
{
    public class AccountService
    {
        private readonly AccountRepository _repo;

        public AccountService(AccountRepository repo)
        {
            _repo = repo;
        }

        public List<Account> GetAccounts()
        {
            return _repo.GetAll();
        }

        public Account? GetAccountById(int id)
        {
            return _repo.GetById(id);
        }

        public void AddAccount(Account account)
        {
            _repo.Add(account);
        }

        public void UpdateAccount(Account account)
        {
            _repo.Update(account);
        }
        public Account? GetLoginByUserName(string userName)
        {
            return _repo.GetByUserName(userName);
        }
    }
}
