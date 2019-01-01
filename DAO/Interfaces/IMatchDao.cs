﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAO.Interfaces
{
    public interface IMatchDao
    {
        void AddMatch(Match match);
        void ReplaceMatch(int id, Match match);
        Task<Match> FindMatch(int id);
    }
}