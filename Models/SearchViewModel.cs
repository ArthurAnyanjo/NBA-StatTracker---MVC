namespace NBA_StatTracker.Models
{
    public class SearchViewModel
    {

        public string? Query { get; set; }
        public List<PlayerModel>? Players { get; set; }

        public List<TeamModel>? Teams { get; set; }
    }

}