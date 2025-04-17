using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Dto;
using WebApplication1.Hubs;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProcessController : ControllerBase
{
    
    private readonly IHubContext<ProcessHub> _hubContext;
    
    public ProcessController(IHubContext<ProcessHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    [HttpPost("atualizar")]
    public async Task<IActionResult> AtualizarProcesso()
    {
        Process[] processes = Process.GetProcesses();
        List<Dictionary<string, string>> namesAndIds = new(); 
        foreach (var process in processes)
        {
            Dictionary<string, string> nameAndId = new Dictionary<string, string>();
            nameAndId.Add("name", process.ProcessName);
            nameAndId.Add("id", process.Id.ToString());
            namesAndIds.Add(nameAndId);
        }

        await _hubContext.Clients.All.SendAsync("process:update", namesAndIds);

        return Ok(new { namesAndIds });
    }}