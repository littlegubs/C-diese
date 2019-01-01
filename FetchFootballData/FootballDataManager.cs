﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using DAO;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FetchFootballData
{
    public class FootballDataManager
    {
        private readonly HttpClient _http = new HttpClient();

        private readonly int[] _availableCompetitions = new int[]
            {2000, 2001, 2002, 2003, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2021};

        public FootballDataManager()
        {
            _http.BaseAddress = new Uri("https://api.football-data.org/v2/");
            _http.DefaultRequestHeaders.Add("X-Auth-Token", "f74e0beb5501485895a1ebb03ba925db");
        }

        public async void GetAllCompetitions()
        {
            try
            {
                Console.WriteLine("----- Begin Fetch football data ----- ");
                Console.WriteLine("     ----- Begin Fetch competitions ----- ");
                var response = await _http.GetAsync("competitions");
                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                var jsonCompetitions = json["competitions"];
                var competitions =
                    JsonConvert.DeserializeObject<List<Competition>>(JsonConvert.SerializeObject(jsonCompetitions));

                foreach (var competition in competitions)
                {
                    var findCompetition = Singleton.Instance.CompetitionDao.FindCompetition(competition.Id).Result;
                    if (findCompetition == null)
                    {
                        Console.WriteLine("Add competition " + competition.Id + " " + competition.Name);
                        Singleton.Instance.CompetitionDao.AddCompetition(competition);
                    }
                    else
                    {
                        Console.WriteLine("Replace competition " + competition.Id + " " + competition.Name);
                        Singleton.Instance.CompetitionDao.ReplaceCompetition(findCompetition.Id, competition);
                    }
                }
                Console.WriteLine("     ----- End Fetch competitions ----- ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void GetAllTeams()
        {
            try
            {
                Console.WriteLine("     ----- Begin Fetch teams ----- ");
                foreach (var availableCompetition in _availableCompetitions)
                {
                    var response = await _http.GetAsync("competitions/" + availableCompetition + "/teams");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseContent);
                    var jsonTeams = json["teams"];
                    var teams = JsonConvert.DeserializeObject<List<Team>>(JsonConvert.SerializeObject(jsonTeams));

                    foreach (var team in teams)
                    {
                        var findTeam = Singleton.Instance.TeamDao.FindTeam(team.Id).Result;
                        if (findTeam == null)
                        {
                            Console.WriteLine("Add team " + team.Id + " " + team.Name);
                            Singleton.Instance.TeamDao.AddTeam(team);
                        }
                        else
                        {
                            Console.WriteLine("Replace team " + team.Id + " " + team.Name);
                            Singleton.Instance.TeamDao.ReplaceTeam(findTeam.Id, team);
                        }
                    }
                }
                Console.WriteLine("     ----- End Fetch teams ----- ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async void GetAllMatchForTheWeek()
        {
            try
            {
                Console.WriteLine("     ----- Begin Fetch matches ----- ");
                var dateFrom = DateTime.Now;
                var dateTo = dateFrom.AddDays(7);

                var response = await _http.GetAsync("matches?dateFrom=" + dateFrom.ToString("yyyy-MM-dd") + "&dateTo=" +
                                                    dateTo.ToString("yyyy-MM-dd"));
                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                var jsonMatch = json["matches"];
                var matches = JsonConvert.DeserializeObject<List<Match>>(JsonConvert.SerializeObject(jsonMatch));

                foreach (var match in matches)
                {
                    var findMatch = Singleton.Instance.MatchDao.FindMatch(match.Id).Result;
                    if (findMatch == null)
                    {
                        Console.WriteLine("Add match " + match.Id);
                        Singleton.Instance.MatchDao.AddMatch(match);
                    }
                    else
                    {
                        Console.WriteLine("Replace match " + match.Id);
                        Singleton.Instance.MatchDao.ReplaceMatch(findMatch.Id, match);
                    }
                }
                Console.WriteLine("     ----- End Fetch matches ----- ");
                Console.WriteLine("----- End Fetch football data ----- ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}