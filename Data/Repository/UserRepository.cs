﻿using Library.Data.Interfaces;
using Library.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private static UserRepository instance;

        private readonly DbContext _dbContext;

        private UserRepository ()
        {
            _dbContext = DbContext.GetContext();
        }

        public static UserRepository GetInstance()
        {
            if (instance == null)
                instance = new UserRepository();
            return instance;
        }

        public async Task AddUser(UserModel userModel)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"INSERT INTO Users (first_name, last_name, email, phone_number, address) " +
                    $"VALUES (@firstName, @lastName, @email, @phoneNumber, @address)";

                _dbContext.GetCommand().Parameters.AddWithValue("@firstName", userModel.FirstName);
                _dbContext.GetCommand().Parameters.AddWithValue("@lastName", userModel.LastName);
                _dbContext.GetCommand().Parameters.AddWithValue("@email", userModel.Email);
                _dbContext.GetCommand().Parameters.AddWithValue("@phoneNumber", userModel.PhoneNumber);
                _dbContext.GetCommand().Parameters.AddWithValue("@address", userModel.Address);

                await _dbContext.GetCommand().ExecuteNonQueryAsync();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
        }

        public async Task DeleteUser(int id)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"DELETE FROM Users WHERE id={id}";

                await _dbContext.GetCommand().ExecuteNonQueryAsync();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }         
        }

        public async Task<UserModel> GetUserById(int id)
        {
            UserModel user;
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT * FROM Users WHERE id = {id}";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

                await reader.ReadAsync();
                user = new UserModel(
                        (int)reader["id"],
                        reader["first_name"] as string,
                        reader["last_name"] as string,
                        reader["email"] as string,
                        reader["phone_number"] as string,
                        reader["address"] as string);
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
            return user;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT * FROM Users";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new UserModel(
                        (int)reader["id"],
                        reader["first_name"] as string,
                        reader["last_name"] as string,
                        reader["email"] as string,
                        reader["phone_number"] as string,
                        reader["address"] as string));
                }
            }
            finally
            { 
                _dbContext.GetCommand().Connection.Close();
            }
            return users;
        }

        public async Task UpdateUser(int id, UserModel userModel)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"UPDATE Users " +
                    $"SET first_name='{userModel.FirstName}'," +
                    $"last_name='{userModel.LastName}'," +
                    $"email='{userModel.Email}'," +
                    $"phone_number='{userModel.PhoneNumber}'," +
                    $"address='{userModel.Address}' " +
                    $"WHERE id={id}";

                await _dbContext.GetCommand().ExecuteNonQueryAsync();
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
        }

        public UserModel GetEmptyUser()
        {
            return new UserModel();
        }

        public async Task<bool> ExistUserWithId(int id)
        {
            try
            {
                _dbContext.GetCommand().Connection.Open();

                _dbContext.GetCommand().CommandText = $"SELECT * FROM Users WHERE id={id}";
                NpgsqlDataReader reader = await _dbContext.GetCommand().ExecuteReaderAsync();

                if (reader.HasRows)
                    return true;
                return false;
            }
            finally
            {
                _dbContext.GetCommand().Connection.Close();
            }
        }
    }
}