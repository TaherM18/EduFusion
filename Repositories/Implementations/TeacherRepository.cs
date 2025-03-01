using Repositories.Interfaces;
using Repositories.Models;
using Npgsql;
using Helpers.Databases;

namespace Repositories.Implementations
{
    public class TeacherRepository : ITeacherInterface
    {
        private readonly DatabaseHelper _helper;
        private readonly NpgsqlConnection _con;

        public TeacherRepository(NpgsqlConnection connection)
        {
            _con = connection;
            _helper = new DatabaseHelper(_con);
        }

        #region Register
        public async Task<int> Register(Teacher teacher)
        {
            int userId = await _helper.InsertGetId(
                "t_user",
                new string[] {
                    "c_first_name",
                    "c_last_name",
                    "c_birth_date",
                    "c_contact",
                    "c_email",
                    "c_password",
                    "c_gender",
                    "c_image",
                    "c_address",
                    "c_role"
                },
                new List<object> {
                    teacher.User.FirstName,
                    teacher.User.LastName,
                    teacher.User.BirthDate,
                    teacher.User.Contact,
                    teacher.User.Email,
                    teacher.User.Password,
                    teacher.User.Gender,
                    teacher.User.Image ?? (object)DBNull.Value,
                    teacher.User.Address,
                    "T"
                },
                "c_userid"
            );

            Console.WriteLine("TeacherRepository - Register - userId = " + userId);

            if (userId > 0) // Ensure a valid userId before inserting teacher data
            {
                await _helper.InsertOne(
                    "t_teacher",
                    new string[] { 
                        "c_teacherid", 
                        "c_salary", 
                        "c_experience_years",
                        "c_qualification", 
                        "c_expertise" 
                    },
                    new List<object> { 
                        userId, 
                        teacher.Salary, 
                        teacher.ExperienceYears, 
                        teacher.Qualification, 
                        teacher.Expertise 
                    }
                );
            }
            else
            {
                Console.WriteLine("Failed to insert user, skipping teacher entry.");
            }

            return userId;
        }
        #endregion


        #region GetOne
        public Task<Teacher> GetOne(int id)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region GetAll
        public Task<List<Teacher>> GetAll()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Add
        public Task<int> Add(Teacher data)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Update
        public Task<int> Update(Teacher data)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Delete
        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}