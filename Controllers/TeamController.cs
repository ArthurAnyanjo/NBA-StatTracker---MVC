using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBA_StatTracker.Data;
using NBA_StatTracker.Models;
using NBA_StatTracker.ViewModels;

namespace NBA_StatTracker.Controllers;

public class TeamController : Controller
{

    private readonly StatTrackerDbContext _context;
    private readonly NbaDataService _nbaDataService;

    public TeamController(StatTrackerDbContext context, NbaDataService nbaDataService)
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
