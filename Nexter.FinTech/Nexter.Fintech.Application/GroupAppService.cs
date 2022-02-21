using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application
{
    /// <summary>
    /// Group
    /// </summary>
    public class GroupAppService : IDynamicApiController
    {
        private readonly IRepository<Member> _memberRep;
        private readonly IRepository<Group> _groupRep;
        private readonly IRepository<Transaction> _transcationRep;

        public GroupAppService(IRepository<Member> memberRep, IRepository<Transaction> transcationRep, IRepository<Group> groupRep)
        {
            _memberRep = memberRep;
            _transcationRep = transcationRep;
            _groupRep = groupRep;
        }


        public async Task<Result> GetAsync()
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var group = await _groupRep.AsQueryable().FirstOrDefaultAsync(e => e.Id == session.GroupId);
            var members = await _memberRep.AsQueryable().Where(e => e.GroupId == session.GroupId).ToListAsync();
            var memberIds = members.Select(e => e.Id).ToArray();
            var result = await _transcationRep.AsQueryable().Where(e => memberIds.Contains(e.MemberId)).ToArrayAsync();
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

        public async Task<Result> PostAsync([FromBody] GroupRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var group = await _groupRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.CreateMemberId == session.Id);
            if (group == null)
            {
                group = new Group(request.Name, session.Id);
                await _groupRep.InsertAsync(group);
            }
            else
            {
                group.Name = request.Name;
            }
            var member = await _memberRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == session.Id);
            member?.SetGroup(group.Id); ;
            await _groupRep.SaveNowAsync();
            return Result.Complete(group);
        }

        [Route("Quit")]
        public async Task<Result> PostAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var member = await _memberRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == session.Id && e.GroupId == id);
            member?.QuitGroup();
            await _memberRep.SaveNowAsync();
            return Result.Complete();
        }


        public async Task<Result> DeleteAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var group = await _groupRep.AsQueryable().FirstOrDefaultAsync(e => e.Id == id && e.CreateMemberId == session.Id);
            await _groupRep.DeleteNowAsync(group);
            return Result.Complete(group);
        }
    }
}