using Microsoft.EntityFrameworkCore;
using NBA_StatTracker.Models;

namespace NBA_StatTracker.Data
{

    public class StatTrackerDbContext : DbContext
    {

        public StatTrackerDbContext(DbContextOptions<StatTrackerDbContext> options)
                : base(options)
        {   
        }


        public DbSet<PlayerModel> Players { get; set; }

        public DbSet<TeamModel> Teams { get; set; }

        public DbSet<PlayerStatsModel> PlayerStats { get; set; }

    }

}