

using System.Data;
using Helpers.Databases;
using Helpers.Extensions;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class AuthRepository : IUserInterface
    {

        private readonly DatabaseHelper _helper;

        public AuthRepository(NpgsqlConnection con) => _helper = new DatabaseHelper(con);

        public async Task<Student> GetStudent(User user, int sid)
        {
            Student student = new Student();

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
    }
}