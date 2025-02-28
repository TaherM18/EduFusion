using Npgsql;
using EduFusion.Helpers.DB;
using EduFusion.Repositories.Interfaces;
using EduFusion.Repositories.Models;
using EduFusion.Helpers.DB;
using System.Data;
using System.Collections;

namespace EduFusion.Repositories.Implementations
{
    public class UserRepository : IUserInterface
    {
        private readonly DBHelper _helper;

        public UserRepository(NpgsqlConnection connection)
        {
            _helper = new DBHelper(connection);
        }

        /// <summary>
        /// Login user by email and password
        /// </summary>
        public async Task<User> Login(LoginVM vm)
        {
            User data = null;
            DataTable dt = await _helper.GetTableWithCondition("t_users", new Dictionary<string, object> {
                {"c_email", vm.Email},
                {"c_password", vm.Password}
            });

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No user found");
                return null;
            }

            DataRow row = dt.Rows[0];

            data = new User
            {
                UserId = Convert.ToInt32(row["c_userid"]),
                FirstName = row["c_firstname"].ToString(),
                LastName = row["c_lastname"].ToString(),
                Email = row["c_email"].ToString(),
                Password = row["c_password"].ToString(),
                Gender = row["c_gender"].ToString(),
                Image = row["c_image"]?.ToString()
            };

            return data;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<int> Register(User user)
        {
            return await _helper.InsertOne("t_users",
                new string[] { "c_firstname", "c_lastname", "c_email", "c_password", "c_gender", "c_image" },
                new ArrayList { user.FirstName, user.LastName, user.Email, user.Password, user.Gender, user.Image ?? (object)DBNull.Value }
            );
        }

        public Task<int> TeacherRegister(Teacher data)
        {
            throw new NotImplementedException();
        }
    }
}
