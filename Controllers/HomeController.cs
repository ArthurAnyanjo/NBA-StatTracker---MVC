using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBA_StatTracker.Data;
using NBA_StatTracker.Models;
using NBA_StatTracker.ViewModels;

namespace NBA_StatTracker.Controllers;

public class HomeController : Controller
{
    private readonly StatTrackerDbContext _context;
    private readonly NbaDataService _nbaDataService;

    public HomeController(StatTrackerDbContext context, NbaDataService nbaDataService)
    {
        _context = context;

        _nbaDataService = nbaDataService;
    }

    public async Task<IActionResult> Index()
    {

        var players = await _context.Players.ToListAsync();
        var teams = await _context.Teams.ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            Players = players,
            Teams = teams
        };

        return View(viewModel);
    }

    public IActionResult SearchPlayer()
    {
        return View(new SearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> SearchPlayer(SearchViewModel model)
    {

        if (_context.Players == null)
        {
            return Problem("Entity set 'PlayerContext' is  null.");
        }

        if (String.IsNullOrWhiteSpace(model.Query))
        {
            model.Players = new List<PlayerModel>();
        }
        else
        {
            model.Players = await _context.Players.Include(p => p.Team).Where(p => p.First_Name.Contains(model.Query) || p.Last_Name.Contains(model.Query)).ToListAsync();

            if(model.Players.Count == 0){

                 await _nbaDataService.FetchAndStoreMorePlayersData(model.Query);

                 model.Players = await _context.Players.Include(p => p.Team).Where(p => p.First_Name.Contains(model.Query) || p.Last_Name.Contains(model.Query)).ToListAsync();
            }
            
        }

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> FetchPlayerData()
    {
        await _nbaDataService.FetchAndStorePlayerData();

        if (!await _context.Teams.AnyAsync())
        {
            Console.WriteLine("Teams data not available. Fetch team data first.");
            ModelState.AddModelError("", "Teams data not available. Fetch team data first.");
        }

        return RedirectToAction("Index", "Player");
    }

    public async Task<IActionResult> FetchTeamData()
    {
        await _nbaDataService.FetchAndStoreTeamData();

        TempData["Success"] = "Team Data Added Successfully!";

        return RedirectToAction("Index", "Home");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
