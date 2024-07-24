using Newtonsoft.Json;
using NBA_StatTracker.Models;
using NBA_StatTracker.Data;
using Microsoft.EntityFrameworkCore;

public class NbaDataService
{

    private readonly StatTrackerDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public NbaDataService(StatTrackerDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task FetchAndStorePlayerData()
    {

        if (!await _context.Teams.AnyAsync())
        {
            Console.WriteLine("Teams data not available. Fetching team data first.");
            return;
        }

        string apiUrl = _configuration["NbaApi:PlayerStatsUrl"];
        string apiKey = _configuration["NbaApi:ApiKey"];

        _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

        HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiUrl);

        if (responseMessage.IsSuccessStatusCode)
        {

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            Console.WriteLine(Convert.ToString(jsonResponse));

            var playerResponse = JsonConvert.DeserializeObject<ApiResponse<PlayerModel>>(jsonResponse);

            var existingPlayers = await _context.Players.AsNoTracking().ToListAsync();

            foreach (var player in playerResponse.Data)
            {
                var existingPlayer = existingPlayers.SingleOrDefault(p => p.Id == player.Id);

                if (existingPlayer != null)
                {
                    _context.Entry(existingPlayer).State = EntityState.Detached;
                }

                _context.Entry(player).State = existingPlayer == null ? EntityState.Added : EntityState.Modified;
            }

            await _context.SaveChangesAsync();

        }

        else
        {
            Console.WriteLine("Failed to fetch Player data from API: " + responseMessage.StatusCode);

        }
    }

    public async Task FetchAndStoreTeamData()
    {
        string apiUrl = _configuration["NbaApi:TeamStatsUrl"];
        string apiKey = _configuration["NbaApi:ApiKey"];

        _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

        HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiUrl);

        if (responseMessage.IsSuccessStatusCode)
        {

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            var teamsResponse = JsonConvert.DeserializeObject<ApiResponse<TeamModel>>(jsonResponse);

            foreach (var team in teamsResponse.Data)
            {
                var existingTeam = await _context.Teams.FindAsync(team.Id);

                if (existingTeam == null)
                {
                    _context.Teams.Add(team);
                }
                else
                {
                    _context.Entry(existingTeam).CurrentValues.SetValues(team);
                }
            }

            await _context.SaveChangesAsync();


        }
        else
        {
            Console.WriteLine("Failed to fetch Team data from API: " + responseMessage.StatusCode);
        }

    }


public async Task<List<PlayerStatsModel>> FetchPlayerStats(int playerId)
{
    string statsApiUrl = $"https://api.balldontlie.io/v1/season_averages?season=2023&player_ids[]={playerId}";
    string apiKey = _configuration["NbaApi:ApiKey"];
    
    _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

    // Fetch player stats
    HttpResponseMessage statsResponse = await _httpClient.GetAsync(statsApiUrl);
    if (!statsResponse.IsSuccessStatusCode)
    {
        Console.WriteLine("Failed to fetch Player stats from API: " + statsResponse.StatusCode);
        return null;
    }

    // Deserialize player stats response
    string statsJsonResponse = await statsResponse.Content.ReadAsStringAsync();
    var playerStatsResponse = JsonConvert.DeserializeObject<ApiResponse<PlayerStatsModel>>(statsJsonResponse);


    foreach (var playerStat in playerStatsResponse.Data)
    {
        // Retrieve existing player stats from the database
        var existingPlayerStats = await _context.PlayerStats.FindAsync(playerStat.Player_Id); 
         playerStat.Player = await _context.Players.SingleOrDefaultAsync(p => p.Id == playerId);
         

        if (existingPlayerStats == null)
        {
            _context.PlayerStats.Add(playerStat);
        }
        else
        {
            _context.Entry(existingPlayerStats).CurrentValues.SetValues(playerStat);
        }
    }

    await _context.SaveChangesAsync();
    return playerStatsResponse.Data;
}







    public async Task FetchAndStoreMorePlayersData(string playerQuery)
    {

        if (!await _context.Teams.AnyAsync())
        {
            Console.WriteLine("Teams data not available. Fetching team data first.");
            return;
        }

        string apiUrl = _configuration["NbaApi:PlayerStatsUrl"] + $"&search={playerQuery}";
        string apiKey = _configuration["NbaApi:ApiKey"];

        _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

        HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiUrl);

        if (responseMessage.IsSuccessStatusCode)
        {

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            Console.WriteLine(Convert.ToString(jsonResponse));

            var playerResponse = JsonConvert.DeserializeObject<ApiResponse<PlayerModel>>(jsonResponse);

            var existingPlayers = await _context.Players.AsNoTracking().ToListAsync();

            foreach (var player in playerResponse.Data)
            {
                var existingPlayer = existingPlayers.SingleOrDefault(p => p.Id == player.Id);

                if (existingPlayer != null)
                {
                    _context.Entry(existingPlayer).State = EntityState.Detached;
                }

                _context.Entry(player).State = existingPlayer == null ? EntityState.Added : EntityState.Modified;
            }

            await _context.SaveChangesAsync();

        }

        else
        {
            Console.WriteLine("Failed to fetch Player data from API: " + responseMessage.StatusCode);

        }
    }





    public class ApiResponse<T>
    {
        public List<T> Data { get; set; }

    }


}