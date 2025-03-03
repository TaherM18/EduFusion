using System.Data;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using Helpers.Databases;

namespace Repositories.Implementations
{
    public class AuthRepository : IUserInterface
    {

        private readonly DatabaseHelper _helper;

        public AuthRepository(NpgsqlConnection con) => _helper = new DatabaseHelper(con);

        public async Task<User> Login(LoginVM vm)
        {
            User data = null;
            DataTable dt = await _helper.GetTableWithCondition("t_user", new Dictionary<string, object> {
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
                UserID = Convert.ToInt32(row["c_userid"]),
                FirstName = row["c_first_name"].ToString(),
                LastName = row["c_last_name"].ToString(),
                BirthDate = row["c_birth_date"] != DBNull.Value ? Convert.ToDateTime(row["c_birth_date"]) : null,
                Gender = row["c_gender"].ToString(),
                Image = row["c_image"]?.ToString(),
                Email = row["c_email"].ToString(),
                Password = row["c_password"].ToString(),
                Contact = row["c_contact"].ToString(),
                Role = row["c_role"].ToString(),
                Address = row["c_address"].ToString(),
                Pincode = row["c_pincode"].ToString(),
                IsActive = row["c_is_active"] != DBNull.Value && Convert.ToBoolean(row["c_is_active"]),
                CreatedAt = row["c_created_at"] != DBNull.Value ? Convert.ToDateTime(row["c_created_at"]) : DateTime.UtcNow,
                UpdatedAt = row["c_updated_at"] != DBNull.Value ? Convert.ToDateTime(row["c_updated_at"]) : DateTime.UtcNow
            };

            return data;
        }
    }
}