using Npgsql;
using EduFusion.Repositories.Interfaces;
using EduFusion.Helpers.DB;
using EduFusion.Repositories.Models;
using System.Collections;

namespace EduFusion.Repositories.Implementations
{
    public class StudentRepository : IStudentInterface
    {
        private readonly DBHelper _helper;
        private readonly NpgsqlConnection _con;

        public StudentRepository(NpgsqlConnection connection)
        {
            _con = connection;
            _helper = new DBHelper(_con);
        }

        #region Register
        public async Task<int> Register(Student student)
        {
            int userId = await _helper.InsertGetId(
                "t_users",
                new string[] {
                    "c_first_name",
                    "c_last_name",
                    "c_birth_date",
                    "c_contact",
                    "c_email",
                    "c_password",
                    "c_gender",
                    "c_image",
                    "c_address"
                },
                new List<object> {
                    student.user.FirstName,
                    student.user.LastName,
                    student.user.BirthDate,
                    student.user.Contact,
                    student.user.Email,
                    student.user.Password,
                    student.user.Gender,
                    student.user.Image ?? (object)DBNull.Value,
                    student.user.Address
                },
                "c_userid"
            );

            Console.WriteLine("StudentRepository - Register - userId=" + userId);

            if (userId > 0) // Ensure valid userId before inserting student record
            {
                await _helper.InsertOne(
                    "t_student",
                    new string[] {
                        "c_studentID",
                        "c_roll_number",
                        "c_guardian_name",
                        "c_guardian_contact",
                        "c_section"
                    },
                    new List<object> {
                        userId,
                        student.RollNumber,
                        student.GuardianName,
                        student.GuardianContact,
                        student.Section
                    }
                );
            }
            else
            {
                Console.WriteLine("Failed to insert user, skipping student entry.");
            }

            return userId;
        }
        #endregion

        #region GetOne
        public Task<Student> GetOne(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetAll
        public Task<List<Student>> GetAll()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update
        public Task<int> Update(Student data)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Add
        public Task<int> Add(Student data)
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