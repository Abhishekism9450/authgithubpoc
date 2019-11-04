using System.Collections.Generic;
using System.Linq;
using dotnet.Models;
using dotnet.DataContext;
namespace dotnet.Service
{
    public class InMemory
    {
        private TokenDbContext _context;

        public InMemory(TokenDbContext context)
        {
            _context = context;
        }

        public void Add(string token, string EmailId)
        {
            var _token = new Token { access_Token = token ,Email=EmailId};
            _context.TokenDb.Add(_token);
            _context.SaveChanges();
        }

        public IEnumerable<Token> Find(string EmailId)
        {
            return _context.TokenDb
                .Where(b => b.Email.Contains(EmailId))
                .OrderBy(b => b.Email)
                .ToList();
        }
    }
}