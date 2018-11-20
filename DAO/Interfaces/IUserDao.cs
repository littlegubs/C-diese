﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace DAO.Interfaces
{
    public interface IUserDao
    {
        Task<List<User>> FindAllUser();
        Task<User> FindUser(string uid);
        Task<User> FindUserByEmail(string email);
        Task<User> FindUserByPseudo(string pseudo);
        void AddUser(User user);
        void DeleteUser(string uId);
        void UpdateUser(string uId, User userParam);
    }
}
