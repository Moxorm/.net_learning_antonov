using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PwServer.Models;

namespace webapi.Data
{
    public class webapiContext : DbContext
    {
        public webapiContext (DbContextOptions<webapiContext> options)
            : base(options)
        {
        }

        public DbSet<UserInfoModel> UserInfoModel { get; set; }
        public DbSet<TransactionModel> TransactionModel { get; set; } = default!;
    }
}
