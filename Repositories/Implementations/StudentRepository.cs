using Npgsql;
using Repositories.Interfaces;
using Helpers.Databases;
using Repositories.Models;
using System.Collections;

namespace Repositories.Implementations
{
    public class StudentRepository : IStudentInterface
    {
        private readonly DatabaseHelper _helper;
        private readonly NpgsqlConnection _con;

        public StudentRepository(NpgsqlConnection connection)
        {
            _con = connection;
            _helper = new DatabaseHelper(_con);
        }

        #region Register
        public async Task<int> Register(Student student)
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
                    student.User.FirstName,
                    student.User.LastName,
                    student.User.BirthDate,
                    student.User.Contact,
                    student.User.Email,
                    student.User.Password,
                    student.User.Gender,
                    student.User.Image ?? (object)DBNull.Value,
                    student.User.Address,
                    "S"
                },
                "c_userID"
            );

            Console.WriteLine("StudentRepository - Register - userId = " + userId);

            if (userId > 0) // Ensure valid userId before inserting student record
            {
                await _helper.InsertOne(
                    "t_student",
                    new string[] {
                        "c_studentID",
                        "c_standardID",
                        "c_roll_number",
                        "c_guardian_name",
                        "c_guardian_contact",
                        "c_section"
                    },
                    new List<object> {
                        userId,
                        student.StandardID,
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