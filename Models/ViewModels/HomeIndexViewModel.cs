using System.Collections.Generic;
using NBA_StatTracker.Models;

namespace NBA_StatTracker.ViewModels
{
    public class HomeIndexViewModel{

        public List<PlayerModel>? Players {get; set;}
        public List<TeamModel>? Teams {get; set;}
    }
    
}