﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class BetDao : IBetDao
    {
        private readonly IMongoCollection<Bet> _collection;

        public BetDao(IMongoCollection<Bet> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<Bet>("bet");
        }

        public async Task<List<Bet>> FindFinishBets(User user, int competitionId)
        {
            var betsByUser = await _collection.Find(bet => bet.User.Id == user.Id).ToListAsync();
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
            }
            var betsByCompetition = betsByUser.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == "FINISHED");

            return betsByMatchStatus;
        }

        public async void AddBet(Bet bet)
        {
            await _collection.InsertOneAsync(bet);
        }

        public async void AddListBet(List<Bet> bets)
        {
            await _collection.InsertManyAsync(bets);
        }
    }
}