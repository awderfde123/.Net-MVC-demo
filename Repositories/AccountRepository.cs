using demo;
using demo.Models;
using System;
using System.Collections.Generic;

namespace demo.Repositories
{
    public class AccountRepository
    {
        private readonly DemoDbContext _context;

        public AccountRepository(DemoDbContext context)
        {
            _context = context;
        }

        public List<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account? GetById(int id)
        {
            return _context.Accounts.FirstOrDefault(p => p.Id == id);
        }

        public Account? GetByUserName(string userName)
        {
            return _context.Accounts.FirstOrDefault(p => p.UserName == userName);
        }

        public void Add(Account Account)
        {
            _context.Accounts.Add(Account);
            _context.SaveChanges();
        }
        //TODO 安全更新
        public void Update(Account Account)
        {
            _context.Accounts.Update(Account);
            _context.SaveChanges();
        }

    }
}
