namespace NBA_StatTracker.Models;

public class PlayerModel{

    public int Id { get; set; }
    public string? First_Name { get; set; } 
    public string? Last_Name { get; set; } 
    public string? Position { get; set; } 
    public string? Height { get; set; }
    public int Weight { get; set; }
    public int Jersey_Number{get; set;}
    public string? College{get; set;} 
    public int Team_Id {get;set;}
    public TeamModel? Team{get; set;}


}
