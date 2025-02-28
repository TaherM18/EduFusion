using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class TeacherRepository : ITeacherInterface
    {
        #region Register
        public async Task<int> Register(Teacher teacher)
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
                    "c_address",
                    "c_role"
                },
                new List<object> {
                    teacher.user.FirstName,
                    teacher.user.LastName,
                    teacher.user.BirthDate,
                    teacher.user.Contact,
                    teacher.user.Email,
                    teacher.user.Password,
                    teacher.user.Gender,
                    teacher.user.Image ?? (object)DBNull.Value,
                    teacher.user.Address,
                    teacher.user.Role
                },
                "c_userid"
            );

            Console.WriteLine("TeacherRepository - Register - userId=" + userId);

            if (userId > 0) // Ensure a valid userId before inserting teacher data
            {
                await _helper.InsertOne(
                    "t_teachers",
                    new string[] { "c_userID", "c_salary", "c_qualification", "c_experience", "c_expertise" },
                    new List<object> { userId, teacher.Salary, teacher.Qualification, teacher.Experience, teacher.SubjectExpertise }
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