using Repositories.Interfaces;
using Repositories.Models;
using Npgsql;
using Helpers.Databases;
using System.Data;
using System.Collections;

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

        private static object GetDbValue(object? value) => value ?? DBNull.Value;

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
                    teacher.User?.FirstName ?? (object)DBNull.Value,
                    teacher.User?.LastName ?? (object)DBNull.Value,
                    teacher.User?.BirthDate ?? (object)DBNull.Value,
                    teacher.User?.Contact ?? (object)DBNull.Value,
                    teacher.User?.Email ?? (object)DBNull.Value,
                    teacher.User?.Password ?? (object)DBNull.Value,
                    teacher.User?.Gender ?? (object)DBNull.Value,
                    teacher.User?.Image ?? (object)DBNull.Value,
                    teacher.User?.Address ?? (object)DBNull.Value,
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

        #region Approve

        public async Task<int> Approve(int sid)
        {
            return await _helper.UpdateOne("t_teacher", new string[]{
                "c_is_approved"
            },
            new ArrayList{
                true
            }, "c_teacherid", sid.ToString());
        }

        #endregion

        #region UnApprove

        public async Task<int> UnApprove(int sid)
        {
            return await _helper.UpdateOne("t_teacher", new string[]{
                "c_is_approved"
            },
            new ArrayList{
                false
            }, "c_teacherid", sid.ToString());
        }

        #endregion


        #region GetOne
        public async Task<Teacher?> GetOne(int id)
        {
            const string query = @"
            SELECT 
                u.c_userid, u.c_first_name, u.c_last_name, u.c_birth_date, 
                u.c_contact, u.c_email, u.c_gender, u.c_image, u.c_address, 
                u.c_is_active, u.c_role, u.c_pincode, 
                t.c_salary, t.c_qualification, t.c_experience_years, t.c_expertise,
                t.c_is_approved
            FROM t_user u
            INNER JOIN t_teacher t ON u.c_userid = t.c_teacherid
            WHERE u.c_userid = @Id AND u.c_is_active = TRUE;";


            try
            {
                await _con.OpenAsync();

                await using var command = new NpgsqlCommand(query, _con);
                command.Parameters.AddWithValue("@Id", id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Teacher
                    {
                        User = new User()
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
                        TeacherID = reader.GetInt32("c_userid"),
                        Salary = reader.IsDBNull("c_salary") ? 0f : reader.GetFloat("c_salary"),
                        Qualification = reader.IsDBNull("c_qualification") ? "" : reader.GetString("c_qualification"),
                        ExperienceYears = reader.IsDBNull("c_experience_years") ? 0 : reader.GetInt32("c_experience_years"),
                        Expertise = reader.IsDBNull("c_expertise") ? "" : reader.GetString("c_expertise"),
                        IsApproved = reader.GetBoolean("c_is_approved"),
                    };
                }
                return null; // Teacher not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] TeacherRepository - GetOne():\n{ex.Message}");
                return null;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }

        #endregion


        #region GetAll
        public async Task<List<Teacher>> GetAll()
        {
            const string query = @"
            SELECT 
                u.c_userid, u.c_first_name, u.c_last_name, u.c_birth_date, 
                u.c_contact, u.c_email, u.c_gender, u.c_image, u.c_address, 
                u.c_is_active, u.c_role, u.c_pincode, 
                t.c_salary, t.c_qualification, t.c_experience_years, t.c_expertise,
                t.c_is_approved
            FROM t_user u
            INNER JOIN t_teacher t ON u.c_userid = t.c_teacherid
            WHERE u.c_is_active = TRUE;";

            var teachers = new List<Teacher>();
            await _con.OpenAsync();

            try
            {
                await using var command = new NpgsqlCommand(query, _con);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    teachers.Add(new Teacher
                    {
                        User = new User()
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
                        TeacherID = reader.GetInt32("c_userid"),
                        Salary = reader.IsDBNull("c_salary") ? 0f : reader.GetFloat("c_salary"),
                        Qualification = reader.IsDBNull("c_qualification") ? "" : reader.GetString("c_qualification"),
                        ExperienceYears = reader.IsDBNull("c_experience_years") ? 0 : reader.GetInt32("c_experience_years"),
                        Expertise = reader.IsDBNull("c_expertise") ? "" : reader.GetString("c_expertise"),
                        IsApproved = reader.GetBoolean("c_is_approved"),
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] TeacherRepository - GetAll():\n{ex.Message}");
            }
            finally
            {
                await _con.CloseAsync();
            }

            return teachers;
        }

        #endregion


        #region Add
        public async Task<int> Add(Teacher data)
        {
            const string userQuery = @"
            INSERT INTO t_user (c_first_name, c_last_name, c_birth_date, c_contact, c_email, 
                                c_gender, c_image, c_address, c_pincode, c_role)
            VALUES (@FirstName, @LastName, @BirthDate, @Contact, @Email, @Gender, 
                    @Image, @Address, @Pincode, @Role)
            RETURNING c_userid;";

            const string teacherQuery = @"
            INSERT INTO t_teacher (c_teacherid, c_salary, c_qualification, c_experience, c_expertise)
            VALUES (@UserId, @Salary, @Qualification, @Experience, @Expertise);";

            await _con.OpenAsync();
            await using var transaction = await _con.BeginTransactionAsync();

            try
            {
                int userId;
                await using (var userCmd = new NpgsqlCommand(userQuery, _con, transaction))
                {
                    if (data.User is null)
                        throw new ArgumentException("User data is required for adding a teacher.");

                    userCmd.Parameters.AddWithValue("@FirstName", GetDbValue(data.User.FirstName));
                    userCmd.Parameters.AddWithValue("@LastName", GetDbValue(data.User.LastName));
                    userCmd.Parameters.AddWithValue("@BirthDate", GetDbValue(data.User.BirthDate));
                    userCmd.Parameters.AddWithValue("@Contact", GetDbValue(data.User.Contact));
                    userCmd.Parameters.AddWithValue("@Email", GetDbValue(data.User.Email));
                    userCmd.Parameters.AddWithValue("@Gender", GetDbValue(data.User.Gender));
                    userCmd.Parameters.AddWithValue("@Image", GetDbValue(data.User.Image));
                    userCmd.Parameters.AddWithValue("@Address", GetDbValue(data.User.Address));
                    userCmd.Parameters.AddWithValue("@Pincode", GetDbValue(data.User.Pincode));
                    userCmd.Parameters.AddWithValue("@Role", 'T');  // Explicitly setting "T" for Teacher role

                    userId = Convert.ToInt32(await userCmd.ExecuteScalarAsync());
                }

                await using (var teacherCmd = new NpgsqlCommand(teacherQuery, _con, transaction))
                {
                    teacherCmd.Parameters.AddWithValue("@UserId", userId);
                    teacherCmd.Parameters.AddWithValue("@Salary", data.Salary);
                    teacherCmd.Parameters.AddWithValue("@Qualification", data.Qualification);
                    teacherCmd.Parameters.AddWithValue("@Experience", data.ExperienceYears);
                    teacherCmd.Parameters.AddWithValue("@Expertise", data.Expertise);

                    await teacherCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return userId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Error] TeacherRepository - Add():\n{ex.Message}");
                return -1;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }

        #endregion


        #region Update
        public async Task<int> Update(Teacher data)
        {
            const string userQuery = @"
            UPDATE t_user 
            SET c_first_name = @FirstName, c_last_name = @LastName, c_birth_date = @BirthDate, 
                c_contact = @Contact, c_email = @Email, c_gender = @Gender, 
                c_image = @Image, c_address = @Address, c_pincode = @Pincode
            WHERE c_userid = @UserId;";

            const string teacherQuery = @"
            UPDATE t_teacher
            SET c_salary = @Salary, c_qualification = @Qualification, 
                c_experience_years = @ExperienceYears, c_expertise = @Expertise
            WHERE c_teacherid = @UserId;";

            await _con.OpenAsync();
            await using var transaction = await _con.BeginTransactionAsync();

            try
            {
                await using (var userCmd = new NpgsqlCommand(userQuery, _con, transaction))
                {
                    userCmd.Parameters.AddWithValue("@FirstName", data.User.FirstName);
                    userCmd.Parameters.AddWithValue("@LastName", data.User.LastName);
                    userCmd.Parameters.AddWithValue("@BirthDate", data.User.BirthDate);
                    userCmd.Parameters.AddWithValue("@Contact", data.User.Contact);
                    userCmd.Parameters.AddWithValue("@Email", data.User.Email);
                    userCmd.Parameters.AddWithValue("@Gender", data.User.Gender);
                    userCmd.Parameters.AddWithValue("@Image", (object?)data.User.Image ?? DBNull.Value);
                    userCmd.Parameters.AddWithValue("@Address", data.User.Address);
                    userCmd.Parameters.AddWithValue("@Pincode", data.User.Pincode);
                    userCmd.Parameters.AddWithValue("@UserId", data.TeacherID);

                    await userCmd.ExecuteNonQueryAsync();
                }

                await using (var teacherCmd = new NpgsqlCommand(teacherQuery, _con, transaction))
                {
                    teacherCmd.Parameters.AddWithValue("@UserId", data.TeacherID);
                    teacherCmd.Parameters.AddWithValue("@Salary", data.Salary);
                    teacherCmd.Parameters.AddWithValue("@Qualification", data.Qualification);
                    teacherCmd.Parameters.AddWithValue("@ExperienceYears", data.ExperienceYears);
                    teacherCmd.Parameters.AddWithValue("@Expertise", data.Expertise);

                    await teacherCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return 1;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Error] TeacherRepository - Update():\n{ex.Message}");
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
            const string query = "UPDATE t_user SET c_is_active = FALSE WHERE c_userid = @UserId;";

            await _con.OpenAsync();

            try
            {
                await using var command = new NpgsqlCommand(query, _con);
                command.Parameters.AddWithValue("@UserId", id);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0 ? 1 : 0;  // 1 if successful, 0 if not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] TeacherRepository - Delete():\n{ex.Message}");
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion


        #region GetStudentProgress
        public async Task<List<StudentProgress>> GetStudentProgress()
        {
            string query = @"
        SELECT 
            s.c_studentID, 
            u.c_first_name || ' ' || u.c_last_name AS student_name,
            std.c_standard_name,
            sub.c_subject_name,
            st.c_trackingID,
            st.c_subjectID,
            st.c_percentage AS progress,
            st.c_created_at,
            st.c_updated_at
        FROM t_student s
        JOIN t_user u ON s.c_studentID = u.c_userID
        JOIN t_standard std ON s.c_standardID = std.c_standardID
        JOIN t_subject sub ON std.c_standardID = sub.c_standardID
        JOIN t_subject_tracking st ON sub.c_subjectID = st.c_subjectID
        ORDER BY s.c_studentID, sub.c_subjectID;
    ";

            List<StudentProgress> studentProgress = new List<StudentProgress>();

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        StudentProgress progress = new StudentProgress
                        {
                            TrackingID = Convert.ToInt32(reader["c_trackingID"]),
                            SubjectID = Convert.ToInt32(reader["c_subjectID"]),
                            Percentage = Convert.ToDecimal(reader["progress"]),
                            CreatedAt = Convert.ToDateTime(reader["c_created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["c_updated_at"]),
                            Subject = new Subject
                            {
                                SubjectID = Convert.ToInt32(reader["c_subjectID"]),
                                SubjectName = reader["c_subject_name"].ToString()
                            }
                        };
                        studentProgress.Add(progress);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _con.Close();
            }

            return studentProgress;
        }
        #endregion


        #region GetStudentRatings
        public async Task<List<StudentRating>> GetStudentRatings()
        {
            string query = @"
        SELECT 
            r.c_ratingID,
            r.c_studentID,
            r.c_teacherID,
            r.c_rating,
            r.c_created_at,
            u.c_first_name || ' ' || u.c_last_name AS student_name,
            t.c_first_name || ' ' || t.c_last_name AS teacher_name
        FROM t_student_rating r
        JOIN t_user u ON r.c_studentID = u.c_userID
        JOIN t_user t ON r.c_teacherID = t.c_userID
        ORDER BY r.c_studentID, r.c_teacherID;
    ";

            List<StudentRating> studentRatings = new List<StudentRating>();

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        StudentRating rating = new StudentRating
                        {
                            RatingID = Convert.ToInt32(reader["c_ratingID"]),
                            StudentID = Convert.ToInt32(reader["c_studentID"]),
                            TeacherID = Convert.ToInt32(reader["c_teacherID"]),
                            Rating = Convert.ToInt32(reader["c_rating"]),
                            CreatedAt = Convert.ToDateTime(reader["c_created_at"]),
                            Student = new Student
                            {
                                StudentID = Convert.ToInt32(reader["c_studentID"]),
                                User = new User
                                {
                                    FirstName = reader["student_name"].ToString()
                                }
                            }
                        };
                        studentRatings.Add(rating);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _con.Close();
            }

            return studentRatings;
        }
        #endregion


        #region GetSubjects
        public async Task<List<Subject>> GetSubjects(int standardID)
        {
            string query = @"
                SELECT 
                    *
                FROM t_subject
                WHERE c_standardID = @standardID
                ORDER BY c_subjectID;
            ";

            List<Subject> subjects = new List<Subject>();

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                {
                    cmd.Parameters.AddWithValue("@standardID", standardID);
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Subject subject = new Subject
                            {
                                SubjectID = Convert.ToInt32(reader["c_subjectID"]),
                                SubjectName = reader["c_subject_name"].ToString()
                            };
                            subjects.Add(subject);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _con.Close();
            }
            return subjects;
        }
        #endregion


        #region GetTeacherBySubject
        public async Task<Teacher> GetTeacherBySubject(int c_subjectID)
        {
            string query = @"
                SELECT u.c_first_name || ' ' || u.c_last_name AS student_name 
                FROM t_user u
                JOIN t_subject s ON  u.c_userID = s.c_teacherID
                WHERE s.c_subjectID = @c_subjectID;
            ";

            Teacher teacher = null;

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                {
                    cmd.Parameters.AddWithValue("@c_subjectID", c_subjectID);
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            teacher = new Teacher
                            {
                                User = new User
                                {
                                    FirstName = reader["student_name"].ToString()
                                }
                            };
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _con.Close();
            }

            return teacher;
        }
        #endregion


        #region GetTimeTable
        public async Task<List<TimeTable>> GetTimeTable()
        {
            string query = @"
            SELECT 
                t.c_timetableID AS Id, 
                sub.c_subject_name AS Title, 
                t.c_start_time AS Start, 
                t.c_end_time AS End, 
                t.c_day_of_week AS DayOfWeek, 
                t.c_subjectID AS SubjectId, 
                t.c_classID AS StandardId, 
                sub.c_standardID AS StandardId, 
                sub.c_teacherID AS TeacherId,
                tr.c_qualification AS TeacherQualification,
                tr.c_expertise AS TeacherExpertise,
                std.c_standard_name AS StandardName
            FROM
                t_timetable t
            JOIN
                t_subject sub ON t.c_subjectID = sub.c_subjectID
            JOIN
                t_teacher tr ON sub.c_teacherID = tr.c_teacherID
            JOIN
                t_standard std ON sub.c_standardID = std.c_standardID
            ";

            List<TimeTable> timeTable = new List<TimeTable>();

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        TimeTable timetable = new TimeTable
                        {
                            TimetableID = Convert.ToInt32(reader["Id"]),
                            StartTime = reader.GetTimeSpan(reader.GetOrdinal("Start")),
                            EndTime = reader.GetTimeSpan(reader.GetOrdinal("End")),
                            DayOfWeek = Convert.ToInt32(reader["DayOfWeek"]),
                            Subject = new Subject
                            {
                                SubjectID = Convert.ToInt32(reader["SubjectId"]),
                                SubjectName = reader["Title"].ToString()
                            },
                            Standard = new Standard
                            {
                                StandardID = Convert.ToInt32(reader["StandardId"]),
                                StandardName = reader["StandardName"].ToString()
                            },
                            Teacher = new Teacher
                            {
                                TeacherID = Convert.ToInt32(reader["TeacherId"]),
                                Qualification = reader["TeacherQualification"].ToString(),
                                Expertise = reader["TeacherExpertise"].ToString()
                            }
                        };
                        timeTable.Add(timetable);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                _con.Close();
            }

            return timeTable;
        }
        #endregion


        #region AddTimeTable
        public async Task<int> AddTimeTable(TimeTable timeTable)
        {
            string query = @"
                INSERT INTO t_timetable 
                (c_start_time, c_end_time, c_day_of_week, c_subjectID, c_classID) 
                VALUES 
                (@StartTime, @EndTime, @DayOfWeek, @SubjectID, @c_classID);
            ";

            try
            {
                _con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _con))
                {
                    cmd.Parameters.AddWithValue("@StartTime", timeTable.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", timeTable.EndTime);
                    cmd.Parameters.AddWithValue("@DayOfWeek", timeTable.DayOfWeek);
                    cmd.Parameters.AddWithValue("@SubjectID", timeTable.SubjectID);
                    cmd.Parameters.AddWithValue("@c_classID", timeTable.ClassID);

                    cmd.ExecuteNonQuery();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                _con.Close();
            }
        }
        #endregion
    }
}