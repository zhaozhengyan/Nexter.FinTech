﻿using FinTech.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexter.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.FinTech.Controllers
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
            var members = await Store.AsQueryable<Member>().Where(e => e.GroupId == session.GroupId).ToListAsync();
            var memberIds = members.Select(e => e.Id).ToArray();
            var result = await Store.AsQueryable<Transaction>().Where(e => memberIds.Contains(e.MemberId)).ToArrayAsync();
            return Result.Complete(new
            {
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

        [HttpPost]
        public async Task<Result> PostAsync([FromQuery]string name)
        {
            var session = this.GetSession();
            var group = new Group(name, session.Id);
            await Store.AddAsync(group);
            var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.Id == session.Id);
            member.GroupId = group.Id;
            await Store.CommitAsync();
            return Result.Complete();
        }

        [Route("Quit")]
        [HttpPost]
        public async Task<Result> PostAsync([FromQuery] long id)
        {
            var session = this.GetSession();
            var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.Id == session.Id);
            member.GroupId = 0;
            await Store.CommitAsync();
            return Result.Complete();
        }


        [HttpDelete]
        public async Task<Result> DeleteAsync([FromQuery] long id)
        {
            var session = this.GetSession();
            var group = await Store.AsQueryable<Group>().FirstOrDefaultAsync(e => e.Id == id && e.CreateMemberId == session.Id);
            await Store.RemoveAsync(group);
            await Store.CommitAsync();
            return Result.Complete();
        }
    }
}
