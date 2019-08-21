using System.Linq;
using System.Threading.Tasks;
using FinTech.API.Wechat.Dto;
using FinTech.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace FinTech.API.Wechat.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        public GroupController(IRepository store)
        {
            Store = store;
        }
        protected IRepository Store { get; }

        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var session = this.GetSession();
            var group = await Store.AsQueryable<Group>().FirstOrDefaultAsync(e => e.Id == session.GroupId);
            var members = await Store.AsQueryable<Member>().Where(e => e.GroupId == session.GroupId).ToListAsync();
            var memberIds = members.Select(e => e.Id).ToArray();
            var result = await Store.AsQueryable<Transaction>().Where(e => memberIds.Contains(e.MemberId)).ToArrayAsync();
            return Result.Complete(new
            {
                id = group.Id,
                IsAdmin = session.Id == group?.CreateMemberId,
                totalIncome = result.Sum(e => e.Income),
                totalSpending = result.Sum(e => e.Spending),
                totalMoney = result.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                members = members.Select(e => new
                {
                    e.Id,
                    e.NickName,
                    e.Avatar,
                    money = result.Where(r => r.MemberId == e.Id).Sum(t => t.Income ?? 0 - t.Spending ?? 0)
                })
            });
        }

        public class GroupRequest
        {
            public string Name { get; set; }
        }

        [HttpPost]
        public async Task<Result> PostAsync([FromBody]GroupRequest request)
        {
            var session = this.GetSession();
            var group = await Store.AsQueryable<Group>().FirstOrDefaultAsync(e => e.CreateMemberId == session.Id);
            if (group == null)
            {
                group = new Group(request.Name, session.Id);
                await Store.AddAsync(group);
            }
            else
            {
                group.Name = request.Name;
            }
            var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.Id == session.Id);
            member.GroupId = group.Id;
            await Store.CommitAsync();
            return Result.Complete(group);
        }

        [Route("Quit")]
        [HttpPost]
        public async Task<Result> PostAsync([FromQuery] long id)
        {
            var session = this.GetSession();
            var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.Id == session.Id);
            member.GroupId = 0;
            await Store.CommitAsync();
            return Result.Complete(member.GroupId);
        }


        [HttpDelete]
        public async Task<Result> DeleteAsync([FromQuery] long id)
        {
            var session = this.GetSession();
            var group = await Store.AsQueryable<Group>().FirstOrDefaultAsync(e => e.Id == id && e.CreateMemberId == session.Id);
            await Store.RemoveAsync(group);
            await Store.CommitAsync();
            return Result.Complete(group);
        }
    }
}
