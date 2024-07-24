namespace NBA_StatTracker.Models;

public class PlayerStatsModel
{
    public int Id { get; set; }
    public int Player_Id { get; set; }
    public PlayerModel Player { get; set; }
    public float Pts { get; set; }
    public float Reb { get; set; }
    public float Turnover { get; set; }
    public float Fg_pct { get; set; }
    public float Fg3_pct { get; set; }
    public float Oreb { get; set; }
    public float Dreb { get; set; }
    public float Ast { get; set; }
    public float Stl { get; set; }
    public float Blk { get; set; }
    public string? Min { get; set; }
    public string? Season { get; set; } = "2023";
    
}
