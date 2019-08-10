using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace Nexter.FinTech.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        public MeController(IRepository store)
        {
            Store = store;
        }

        protected IRepository Store { get; }

        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var session = this.GetSession();
            var result = await Store.AsQueryable<Member>()
                .FirstOrDefaultAsync(e => e.Id == session.Id);
            return Result.Complete(result);
        }
    }
}
