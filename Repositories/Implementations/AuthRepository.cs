using System.Data;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using Helpers.Databases;
using Helpers.Extensions;

namespace Repositories.Implementations
{
    public class AuthRepository : IUserInterface
    {

        private readonly DatabaseHelper _helper;

        private readonly NpgsqlConnection _con;

        public AuthRepository(NpgsqlConnection con)
        {
            _helper = new DatabaseHelper(con);
            _con = con;
        }

        #region GetOne
        public async Task<User?> GetOne(int id)
        {
            const string query = @"
            SELECT 
                u.c_userid, u.c_first_name, u.c_last_name, u.c_birth_date, 
                u.c_contact, u.c_email, u.c_gender, u.c_image, u.c_address, u.c_pincode, u.c_role
            FROM t_user u
            WHERE u.c_userid = @id;";

            try
            {
                await _con.CloseAsync();
                await _con.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _con);
                cmd.Parameters.AddWithValue("@id", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // ✅ Fixed ternary operation
                    System.Console.WriteLine("Firstname: " + (reader.IsDBNull("c_first_name") ? "" : reader.GetString("c_first_name")));
                    System.Console.WriteLine("Lastname: " + (reader.IsDBNull("c_last_name") ? "" : reader.GetString("c_last_name")));

                    return new User
                    {
                        UserID = reader.GetInt32("c_userid"),
                        FirstName = reader.IsDBNull("c_first_name") ? "" : reader.GetString("c_first_name"),
                        LastName = reader.IsDBNull("c_last_name") ? "" : reader.GetString("c_last_name"),
                        BirthDate = reader.IsDBNull("c_birth_date") ? null : reader.GetDateTime("c_birth_date"),
                        Contact = reader.IsDBNull("c_contact") ? "" : reader.GetString("c_contact"),
                        Email = reader.IsDBNull("c_email") ? "" : reader.GetString("c_email"),
                        Gender = reader.IsDBNull("c_gender") ? "" : reader.GetString("c_gender"),
                        Image = reader.IsDBNull("c_image") ? null : reader.GetString("c_image"),
                        Address = reader.IsDBNull("c_address") ? "" : reader.GetString("c_address"),
                        Pincode = reader.IsDBNull("c_pincode") ? "" : reader.GetString("c_pincode"),
                        Role = reader.GetString("c_role")
                    };
                }

                return null; // Student not found
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a LoggerHelper or any logging mechanism)
                Console.WriteLine($"StudentRepository - GetOne() : {ex.Message}");
                return null;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion

        public async Task<Student> GetStudent(User user, int sid)
        {
            Student student = null;

            DataTable dt = await _helper.GetTableCustom(@$"
                SELECT 
                    t1.*, t2.*
                FROM t_student AS t1
                JOIN
                    t_standard AS t2
                ON
                 t1.c_standardid = t2.c_standardid
                WHERE
                    t1.c_studentid = {sid}
            ");

            if (dt.Rows.Count > 0)
            {
                student = new Student
                {
                    StudentID = dt.Rows[0]["c_studentid"].ToString().ToInt(),
                    RollNumber = dt.Rows[0]["c_roll_number"].ToString(),
                    GuardianContact = dt.Rows[0]["c_guardian_contact"].ToString(),
                    GuardianName = dt.Rows[0]["c_guardian_name"].ToString(),
                    Section = dt.Rows[0]["c_section"].ToString(),
                    IsApproved = Convert.ToBoolean(dt.Rows[0]["c_is_approved"]),
                    Standard = new Standard
                    {
                        StandardID = dt.Rows[0]["c_standardid"].ToString().ToInt(),
                        StandardName = dt.Rows[0]["c_standard_name"].ToString(),
                    },
                    User = user
                };
            }

            return student;
        }

        public async Task<Teacher> GetTeacher(User user, int tid)
        {
            Teacher teacher = new Teacher();

            DataTable dt = await _helper.GetTableCustom(@$"
                SELECT 
                    t1.*
                FROM t_teacher AS t1
                WHERE
                    t1.c_teacherid = {tid}
            ");

            if (dt.Rows.Count > 0)
            {
                teacher = new Teacher
                {
                    TeacherID = dt.Rows[0]["c_teacherid"].ToString().ToInt(),
                    Salary = dt.Rows[0]["c_salary"].ToString().ToInt(),
                    ExperienceYears = dt.Rows[0]["c_experience_years"].ToString().ToInt(),
                    Qualification = dt.Rows[0]["c_qualification"].ToString(),
                    Expertise = dt.Rows[0]["c_expertise"].ToString(),
                    User = user
                };
            }

            return teacher;
        }

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

        public async Task<List<User>> GetUsers(int adminId)
        {
            List<User> users = new List<User>();
            DataTable dt = await _helper.GetTableWithCondition("t_user", new Dictionary<string, object>
    {
        { "c_userid", adminId }
    });

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["c_role"].ToString() != "A") //role a for admin only adming can retrive users
                {
                    return null;
                }
                else
                {
                    DataTable dtt = await _helper.GetTableAll("t_user");

                    if (dtt.Rows.Count > 0)
                    {
                        users = (
                            from DataRow row in dtt.Rows
                            select new User
                            {
                                UserID = row["c_userid"].ToString().ToInt(),
                                FirstName = row["c_first_name"].ToString(),
                                LastName = row["c_last_name"].ToString(),
                                BirthDate = row["c_birth_date"] != DBNull.Value ? Convert.ToDateTime(row["c_birth_date"]) : null,
                                Gender = row["c_gender"].ToString(),
                                Image = row["c_image"].ToString(),
                                Email = row["c_email"].ToString(),
                                Contact = row["c_contact"].ToString(),
                                Role = row["c_role"].ToString(),
                                Address = row["c_address"].ToString(),
                                Pincode = row["c_pincode"].ToString(),
                                IsActive = row["c_is_active"].ToString() == "1",
                                CreatedAt = row["c_created_at"] != DBNull.Value ? Convert.ToDateTime(row["c_created_at"]) : DateTime.UtcNow,
                                UpdatedAt = row["c_updated_at"] != DBNull.Value ? Convert.ToDateTime(row["c_updated_at"]) : DateTime.UtcNow
                            }
                        ).ToList();
                    }
                }
            }

            return users;
        }
    }
}