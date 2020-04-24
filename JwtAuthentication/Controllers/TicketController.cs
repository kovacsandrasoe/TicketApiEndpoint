using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtAuthentication.Data;
using JwtAuthentication.Hubs;
using JwtAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<TicketHub> _hub;

        public TicketController(UserManager<IdentityUser> userManager, IConfiguration configuration, ApplicationDbContext context, IHubContext<TicketHub> hub)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _hub = hub;
        }


        // GET api/ticket
        [HttpGet]
        public ActionResult<IEnumerable<Ticket>> GetAll()
        {
            return _context.Tickets;
        }

        // GET api/ticket/5HFG6
        [HttpGet("{id}")]
        public ActionResult<Ticket> Get(string id)
        {
            return _context.Tickets.FirstOrDefault(t => t.Uid == id);
        }


        // POST api/ticket
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ticket value)
        {
            value.Uid = Guid.NewGuid().ToString();

            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var myself = await _userManager.GetUserAsync(this.User);

            value.Seller = myself;
            _context.Tickets.Add(value);
            _context.SaveChanges();

            await _hub.Clients.All.SendAsync("NewTicket", value);
            return Ok();

        }

        // PUT api/ticket/5HFG6
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Ticket value)
        {
            var old = _context.Tickets.FirstOrDefault(t => t.Uid == id);
            value.Uid = id;
            _context.Remove(old);
            _context.Add(value);
            _context.SaveChanges();
        }

        // DELETE api/ticket/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var old = _context.Tickets.FirstOrDefault(t => t.Uid == id);
            _context.Remove(old);
            _context.SaveChanges();
        }
    }
}
