﻿using System;
using FinTech.Domain;

namespace Nexter.Fintech.Core
{
    public class Session
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public DateTime WillExpiresAt { get; set; }
        public Session(Member input)
        {
            Id = input.Id;
            GroupId = input.GroupId;
            WillExpiresAt = DateTime.Now.AddHours(1);
        }
    }
}