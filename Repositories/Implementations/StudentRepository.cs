using Npgsql;
using Repositories.Interfaces;
using Helpers.Databases;
using Repositories.Models;
using System.Data;

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

        private static object GetDbValue(object? value) => value ?? DBNull.Value;

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
        public async Task<Student?> GetOne(int id)
        {
            const string query = @"
            SELECT 
                u.c_userid, u.c_first_name, u.c_last_name, u.c_birth_date, 
                u.c_contact, u.c_email, u.c_gender, u.c_image, u.c_address, u.c_pincode, u.c_role,
                s.c_studentID, s.c_standardID, s.c_roll_number, s.c_guardian_name, s.c_guardian_contact, s.c_section
            FROM t_user u
            INNER JOIN t_student s ON u.c_userid = s.c_studentID
            WHERE u.c_userid = @id AND u.c_is_active = TRUE;";

            try
            {
                await _con.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _con);
                cmd.Parameters.AddWithValue("@id", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Student
                    {
                        User = new User
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
                        },
                        StudentID = reader.GetInt32("c_studentID"),
                        StandardID = reader.GetInt32("c_standardID"),
                        RollNumber = reader.IsDBNull("c_roll_number") ? "" : reader.GetString("c_roll_number"),
                        GuardianName = reader.IsDBNull("c_guardian_name") ? "" : reader.GetString("c_guardian_name"),
                        GuardianContact = reader.IsDBNull("c_guardian_contact") ? "" : reader.GetString("c_guardian_contact"),
                        Section = reader.GetString("c_section") ?? ""
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

        #region GetAll
        public async Task<List<Student>?> GetAll()
        {
            const string query = @"
            SELECT 
                u.c_userid, u.c_first_name, u.c_last_name, u.c_birth_date, 
                u.c_contact, u.c_email, u.c_gender, u.c_image, u.c_address, u.c_pincode, u.c_role,
                s.c_studentID, s.c_standardID, s.c_roll_number, s.c_guardian_name, s.c_guardian_contact, s.c_section
            FROM t_user u
            INNER JOIN t_student s ON u.c_userid = s.c_studentID";

            List<Student> studentList = new List<Student>();
            try
            {
                await _con.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _con);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Student student = new Student()
                    {
                        User = new User
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
                        },
                        StudentID = reader.GetInt32("c_studentID"),
                        StandardID = reader.GetInt32("c_standardID"),
                        RollNumber = reader.IsDBNull("c_roll_number") ? "" : reader.GetString("c_roll_number"),
                        GuardianName = reader.IsDBNull("c_guardian_name") ? "" : reader.GetString("c_guardian_name"),
                        GuardianContact = reader.IsDBNull("c_guardian_contact") ? "" : reader.GetString("c_guardian_contact"),
                        Section = reader.GetString("c_section") ?? ""
                    };

                    studentList.Add(student);
                }

                return studentList; // Student not found
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a LoggerHelper or any logging mechanism)
                Console.WriteLine($"StudentRepository - GetAll() : {ex.Message}");
                return null;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion

        #region Update
        public async Task<int> Update(Student data)
        {
            const string query = @"
            UPDATE t_user
            SET 
                c_first_name = @FirstName, 
                c_last_name = @LastName, 
                c_birth_date = @BirthDate, 
                c_contact = @Contact, 
                c_email = @Email, 
                c_gender = @Gender, 
                c_image = @Image, 
                c_address = @Address
            WHERE c_userid = @UserId;

            UPDATE t_student
            SET 
                c_roll_number = @RollNumber, 
                c_guardian_name = @GuardianName, 
                c_guardian_contact = @GuardianContact, 
                c_section = @Section
            WHERE c_studentID = @UserId;";

            try
            {
                await _con.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _con);

                if (data.StudentID is null)
                    throw new ArgumentException("User data is required for updating a student.");

                // Parameters for t_users table
                cmd.Parameters.AddWithValue("@UserId", data.StudentID);
                cmd.Parameters.AddWithValue("@FirstName", GetDbValue(data.User.FirstName));
                cmd.Parameters.AddWithValue("@LastName", GetDbValue(data.User.LastName));
                cmd.Parameters.AddWithValue("@BirthDate", GetDbValue(data.User.BirthDate));
                cmd.Parameters.AddWithValue("@Contact", GetDbValue(data.User.Contact));
                cmd.Parameters.AddWithValue("@Email", GetDbValue(data.User.Email));
                cmd.Parameters.AddWithValue("@Gender", GetDbValue(data.User.Gender));
                cmd.Parameters.AddWithValue("@Image", GetDbValue(data.User.Image));
                cmd.Parameters.AddWithValue("@Address", GetDbValue(data.User.Address));

                // Parameters for t_student table
                cmd.Parameters.AddWithValue("@RollNumber", GetDbValue(data.RollNumber));
                cmd.Parameters.AddWithValue("@GuardianName", GetDbValue(data.GuardianName));
                cmd.Parameters.AddWithValue("@GuardianContact", GetDbValue(data.GuardianContact));
                cmd.Parameters.AddWithValue("@Section", GetDbValue(data.Section));

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StudentRepository - Update() : {ex.Message}");
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion

        #region Add
        public async Task<int> Add(Student data)
        {
            const string userQuery = @"
            INSERT INTO t_user
                (c_first_name, c_last_name, c_birth_date, c_contact, c_email, c_password, c_gender, c_image, c_address, c_role) 
            VALUES 
                (@FirstName, @LastName, @BirthDate, @Contact, @Email, @Password, @Gender, @Image, @Address, @Role)
            RETURNING c_userid;";

            const string studentQuery = @"
            INSERT INTO t_student 
                (c_studentID, c_standardID, c_roll_number, c_guardian_name, c_guardian_contact, c_section) 
            VALUES 
                (@UserID, @StandardID, @RollNumber, @GuardianName, @GuardianContact, @Section);";

            await _con.OpenAsync();
            await using var transaction = await _con.BeginTransactionAsync();

            try
            {
                int userId;

                // Insert into t_users
                await using (var userCmd = new NpgsqlCommand(userQuery, _con, transaction))
                {
                    if (data.User is null)
                        throw new ArgumentException("User data is required for adding a student.");

                    userCmd.Parameters.AddWithValue("@FirstName", GetDbValue(data.User.FirstName));
                    userCmd.Parameters.AddWithValue("@LastName", GetDbValue(data.User.LastName));
                    userCmd.Parameters.AddWithValue("@BirthDate", GetDbValue(data.User.BirthDate));
                    userCmd.Parameters.AddWithValue("@Contact", GetDbValue(data.User.Contact));
                    userCmd.Parameters.AddWithValue("@Email", GetDbValue(data.User.Email));
                    userCmd.Parameters.AddWithValue("@Password", GetDbValue(data.User.Password));
                    userCmd.Parameters.AddWithValue("@Gender", GetDbValue(data.User.Gender));
                    userCmd.Parameters.AddWithValue("@Image", GetDbValue(data.User.Image));
                    userCmd.Parameters.AddWithValue("@Address", GetDbValue(data.User.Address));
                    userCmd.Parameters.AddWithValue("@Role", 'S');

                    userId = Convert.ToInt32(await userCmd.ExecuteScalarAsync());
                }

                // Insert into t_student
                await using (var studentCmd = new NpgsqlCommand(studentQuery, _con, transaction))
                {
                    studentCmd.Parameters.AddWithValue("@UserID", userId);
                    studentCmd.Parameters.AddWithValue("@StandardID", data.StandardID);
                    studentCmd.Parameters.AddWithValue("@RollNumber", GetDbValue(data.RollNumber));
                    studentCmd.Parameters.AddWithValue("@GuardianName", GetDbValue(data.GuardianName));
                    studentCmd.Parameters.AddWithValue("@GuardianContact", GetDbValue(data.GuardianContact));
                    studentCmd.Parameters.AddWithValue("@Section", GetDbValue(data.Section));

                    await studentCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return userId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"StudentRepository - Add() : {ex.Message}");
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion

        #region Delete
        public async Task<int> Delete(int id)
        {
            const string softDeleteQuery = "UPDATE t_user SET c_is_active = FALSE WHERE c_userid = @Id;";

            await _con.OpenAsync();

            try
            {
                await using var command = new NpgsqlCommand(softDeleteQuery, _con);
                command.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] StudentRepository - Delete() :\n{ex.Message}");
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion
    }
}