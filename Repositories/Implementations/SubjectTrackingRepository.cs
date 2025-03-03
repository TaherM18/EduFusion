using System.Data;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class SubjectTrackingRepository : ISubjectTrackingInterface
    {
        private readonly NpgsqlConnection _connection;

        public SubjectTrackingRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #region Add(SubjectTracking)
        public async Task<int> Add(SubjectTracking data)
        {
            const string query = @"
            INSERT INTO t_subject_tracking
                (c_subjectID, c_percentage, c_created_at, c_updated_at) 
            VALUES
                (@SubjectID, @Percentage, NOW(), NOW()) RETURNING c_trackingID;";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@SubjectID", data.SubjectID);
            cmd.Parameters.AddWithValue("@Percentage", (object?)data.Percentage ?? DBNull.Value);

            await _connection.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return Convert.ToInt32(result);
        }
        #endregion


        #region Delete(id)
        public async Task<int> Delete(int id)
        {
            const string query = "DELETE FROM t_subject_tracking WHERE c_trackingID = @TrackingID";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@TrackingID", id);

            await _connection.OpenAsync();
            var result = await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();

            return result;
        }
        #endregion


        #region GetAll
        public async Task<List<SubjectTracking>?> GetAll()
        {
            const string query = @"
            SELECT 
                st.c_trackingID, st.c_subjectID, st.c_percentage, st.c_created_at, st.c_updated_at,
                s.c_subject_name,
                std.c_standard_name,
                u.c_first_name, u.c_last_name
            FROM 
                t_subject_tracking st
            INNER JOIN
                t_subject s ON s.c_subjectID = st.c_subjectID
            INNER JOIN
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID";

            var list = new List<SubjectTracking>();

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                await _connection.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new SubjectTracking
                    {
                        TrackingID = reader.IsDBNull("c_trackingID") ? 0 : reader.GetInt32("c_trackingID"),
                        SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                        Percentage = reader.IsDBNull("c_percentage") ? 0 : reader.GetDecimal("c_percentage"),
                        CreatedAt = reader.GetDateTime("c_created_at"),
                        UpdatedAt = reader.GetDateTime("c_updated_at"),
                        Subject = new Subject()
                        {
                            SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                            SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name"),
                            StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                            Standard = new Standard()
                            {
                                StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                                StandardName = reader.IsDBNull("c_standard_name") ? string.Empty : reader.GetString("c_standard_name"),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.IsDBNull("c_teacherID") ? 0 : reader.GetInt32("c_teacherID"),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull("c_first_name") ? string.Empty : reader.GetString("c_first_name"),
                                    LastName = reader.IsDBNull("c_last_name") ? string.Empty : reader.GetString("c_last_name"),
                                }
                            }
                        }
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubjectTrackingRepository - GetAll():\n{ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetAllByStandard(id)
        public async Task<List<SubjectTracking>?> GetAllByStandard(int standardID)
        {
            const string query = @"
            SELECT 
                st.c_trackingID, st.c_subjectID, st.c_percentage, st.c_created_at, st.c_updated_at,
                s.c_subject_name,
                std.c_standard_name,
                u.c_first_name, u.c_last_name
            FROM 
                t_subject_tracking st
            INNER JOIN
                t_subject s ON s.c_subjectID = st.c_subjectID
            INNER JOIN
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE 
                s.c_standardID = @StandardID";

            var list = new List<SubjectTracking>();

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@StandardID", standardID);

                await _connection.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new SubjectTracking
                    {
                        TrackingID = reader.IsDBNull("c_trackingID") ? 0 : reader.GetInt32("c_trackingID"),
                        SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                        Percentage = reader.IsDBNull("c_percentage") ? 0 : reader.GetDecimal("c_percentage"),
                        CreatedAt = reader.GetDateTime("c_created_at"),
                        UpdatedAt = reader.GetDateTime("c_updated_at"),
                        Subject = new Subject()
                        {
                            SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                            SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name"),
                            StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                            Standard = new Standard()
                            {
                                StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                                StandardName = reader.IsDBNull("c_standard_name") ? string.Empty : reader.GetString("c_standard_name"),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.IsDBNull("c_teacherID") ? 0 : reader.GetInt32("c_teacherID"),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull("c_first_name") ? string.Empty : reader.GetString("c_first_name"),
                                    LastName = reader.IsDBNull("c_last_name") ? string.Empty : reader.GetString("c_last_name"),
                                }
                            }
                        }
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubjectTrackingRepository - GetAllByStandard():\n{ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetAllByTeacher(id)
        public async Task<List<SubjectTracking>?> GetAllByTeacher(int teacherID)
        {
            const string query = @"
            SELECT 
                st.c_trackingID, st.c_subjectID, st.c_percentage, st.c_created_at, st.c_updated_at,
                s.c_subject_name,
                std.c_standard_name,
                u.c_first_name, u.c_last_name
            FROM 
                t_subject_tracking st
            INNER JOIN
                t_subject s ON s.c_subjectID = st.c_subjectID
            INNER JOIN
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE 
                s.c_teacherID = @TeacherID";

            var list = new List<SubjectTracking>();

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);

                await _connection.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new SubjectTracking
                    {
                        TrackingID = reader.IsDBNull("c_trackingID") ? 0 : reader.GetInt32("c_trackingID"),
                        SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                        Percentage = reader.IsDBNull("c_percentage") ? 0 : reader.GetDecimal("c_percentage"),
                        CreatedAt = reader.GetDateTime("c_created_at"),
                        UpdatedAt = reader.GetDateTime("c_updated_at"),
                        Subject = new Subject()
                        {
                            SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                            SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name"),
                            StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                            Standard = new Standard()
                            {
                                StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                                StandardName = reader.IsDBNull("c_standard_name") ? string.Empty : reader.GetString("c_standard_name"),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.IsDBNull("c_teacherID") ? 0 : reader.GetInt32("c_teacherID"),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull("c_first_name") ? string.Empty : reader.GetString("c_first_name"),
                                    LastName = reader.IsDBNull("c_last_name") ? string.Empty : reader.GetString("c_last_name"),
                                }
                            }
                        }
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubjectTrackingRepository - GetAllByTeacher():\n{ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetOne(id)
        public async Task<SubjectTracking?> GetOne(int id)
        {
            const string query = "SELECT * FROM t_subject_tracking WHERE c_trackingID = @TrackingID";

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@TrackingID", id);

                await _connection.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var subjectTracking = new SubjectTracking
                    {
                        TrackingID = reader.IsDBNull("c_trackingID") ? 0 : reader.GetInt32("c_trackingID"),
                        SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                        Percentage = reader.IsDBNull("c_percentage") ? 0 : reader.GetDecimal("c_percentage"),
                        CreatedAt = reader.GetDateTime("c_created_at"),
                        UpdatedAt = reader.GetDateTime("c_updated_at"),
                        Subject = new Subject()
                        {
                            SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                            SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name"),
                            StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                            Standard = new Standard()
                            {
                                StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                                StandardName = reader.IsDBNull("c_standard_name") ? string.Empty : reader.GetString("c_standard_name"),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.IsDBNull("c_teacherID") ? 0 : reader.GetInt32("c_teacherID"),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull("c_first_name") ? string.Empty : reader.GetString("c_first_name"),
                                    LastName = reader.IsDBNull("c_last_name") ? string.Empty : reader.GetString("c_last_name"),
                                }
                            }
                        }
                    };

                    return subjectTracking;
                }
                else
                {
                    Console.WriteLine($"SubjectTrackingRepository - GetOne(): No Data");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubjectTrackingRepository - GetOne():\n{ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region Update(SubjectTracking)
        public async Task<int> Update(SubjectTracking data)
        {
            const string query = @"
            UPDATE 
                t_subject_tracking 
            SET 
                c_subjectID = @SubjectID, 
                c_percentage = @Percentage, 
                c_updated_at = NOW() 
            WHERE
                c_trackingID = @TrackingID";

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@TrackingID", data.TrackingID);
                cmd.Parameters.AddWithValue("@SubjectID", data.SubjectID);
                cmd.Parameters.AddWithValue("@Percentage", (object?)data.Percentage ?? DBNull.Value);

                await _connection.OpenAsync();
                var result = await cmd.ExecuteNonQueryAsync();
                await _connection.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubjectTrackingRepository - Update():\n{ex.Message}");
                return 0;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion
    }
}