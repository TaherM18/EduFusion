using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class TimeTableRepository : ITimeTableInterface
    {
        private readonly NpgsqlConnection _connection;

        public TimeTableRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #region Add
        public async Task<int> Add(TimeTable data)
        {
            const string query = @"
            INSERT INTO t_timetable (c_subjectID, c_classID, c_day_of_week, c_start_time, c_end_time)
            VALUES (@subjectID, @classID, @dayOfWeek, @startTime, @endTime)
            RETURNING c_timetableID;";

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@subjectID", data.SubjectID);
                cmd.Parameters.AddWithValue("@classID", data.ClassID);
                cmd.Parameters.AddWithValue("@dayOfWeek", (object?)data.DayOfWeek ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@startTime", data.StartTime);
                cmd.Parameters.AddWithValue("@endTime", data.EndTime);

                int? affectedRows = (int?)await cmd.ExecuteScalarAsync();

                return affectedRows.HasValue ? affectedRows.Value : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TimeTableRepository - Add() : {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region Delete
        public async Task<int> Delete(int id)
        {
            const string query = "DELETE FROM t_timetable WHERE c_timetableID = @id;";

            try
            {
                await _connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TimeTableRepository - Delete() : {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetAll
        public Task<List<TimeTable>> GetAll()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region GetAllGroupByDayOfWeek
        public async Task<List<TimeTable>> GetAllByStandardGroupByDayOfWeek(int standardID)
        {
            const string query = @"
            SELECT 
                t.c_timetableID, t.c_subjectID, t.c_classID, t.c_day_of_week, t.c_start_time, t.c_end_time,
                s.c_standardID, s.c_teacherID, s.c_subject_name, s.c_marks,
                c.c_class_name, 
                std.c_standard_name, 
                u.c_first_name, u.c_last_name
            FROM 
                t_timetable t
            LEFT JOIN 
                t_subject s ON t.c_subjectID = s.c_subjectID
            LEFT JOIN 
                t_class c ON t.c_classID = c.c_classID
            LEFT JOIN 
                t_standard std ON s.c_standardID = std.c_standardID
            LEFT JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE
                std.c_standardID = @StandardID
            ORDER BY 
                c_day_of_week, c_start_time;";

            var result = new List<TimeTable>();

            try
            {
                await _connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@StandardID", standardID);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var timeTable = new TimeTable
                    {
                        TimetableID = reader.GetInt32(reader.GetOrdinal("c_timetableID")),
                        SubjectID = reader.GetInt32(reader.GetOrdinal("c_subjectID")),
                        ClassID = reader.GetInt32(reader.GetOrdinal("c_classID")),
                        DayOfWeek = reader.IsDBNull(reader.GetOrdinal("c_day_of_week")) ? 0 : reader.GetInt32(reader.GetOrdinal("c_day_of_week")),
                        StartTime = reader.GetTimeSpan(reader.GetOrdinal("c_start_time")),
                        EndTime = reader.GetTimeSpan(reader.GetOrdinal("c_end_time")),
                        ClassModel = new ClassModel()
                        {
                            ClassID = reader.GetInt32(reader.GetOrdinal("c_classID")),
                            ClassName = reader.IsDBNull(reader.GetOrdinal("c_class_name")) ? "" :reader.GetString(reader.GetOrdinal("c_class_name"))
                        },
                        Subject = new Subject()
                        {
                            SubjectID = reader.GetInt32(reader.GetOrdinal("c_subjectID")),
                            SubjectName = reader.IsDBNull(reader.GetOrdinal("c_subject_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_subject_name")),
                            Standard = new Standard()
                            {
                                StandardID = reader.GetInt32(reader.GetOrdinal("c_standardID")),
                                StandardName = reader.IsDBNull(reader.GetOrdinal("c_standard_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_standard_name")),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.GetInt32(reader.GetOrdinal("c_teacherID")),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull(reader.GetOrdinal("c_first_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_first_name")),
                                    LastName = reader.IsDBNull(reader.GetOrdinal("c_last_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_last_name"))
                                }
                            }
                        }
                    };
                    // Console.WriteLine("TimeTableRepository - GetAllByStandardGroupByDayOfWeek() - TimeTableId="+timeTable.TimetableID);

                    string dayName = GetDayOfWeekName(timeTable.DayOfWeek);
                    timeTable.DayName = dayName;
                    
                    result.Add(timeTable);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] TimeTableRepository - GetAllByStandardGroupByDayOfWeek() :\n{ex.Message}");
                // return new Dictionary<string, List<TimeTable>>(); // Returning empty dictionary instead of null
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetOne
        public async Task<TimeTable?> GetOne(int id)
        {
            const string query = @"
            SELECT 
                t.c_timetableID, t.c_subjectID, t.c_classID, t.c_day_of_week, t.c_start_time, t.c_end_time,
                s.c_standardID, s.c_teacherID, s.c_subject_name, s.c_marks,
                c.c_class_name, 
                std.c_standard_name, 
                u.c_first_name, u.c_last_name
            FROM 
                t_timetable t
            LEFT JOIN 
                t_subject s ON t.c_subjectID = s.c_subjectID
            LEFT JOIN 
                t_class c ON t.c_classID = c.c_classID
            LEFT JOIN 
                t_standard std ON s.c_standardID = std.c_standardID
            LEFT JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE
                t.c_timetableID = @id";

            try
            {
                await _connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TimeTable
                    {
                        TimetableID = reader.GetInt32(reader.GetOrdinal("c_timetableID")),
                        SubjectID = reader.GetInt32(reader.GetOrdinal("c_subjectID")),
                        ClassID = reader.GetInt32(reader.GetOrdinal("c_classID")),
                        DayOfWeek = reader.IsDBNull(reader.GetOrdinal("c_day_of_week")) ? 0 : reader.GetInt32(reader.GetOrdinal("c_day_of_week")),
                        StartTime = reader.GetTimeSpan(reader.GetOrdinal("c_start_time")),
                        EndTime = reader.GetTimeSpan(reader.GetOrdinal("c_end_time")),
                        Subject = new Subject()
                        {
                            SubjectID = reader.GetInt32(reader.GetOrdinal("c_subjectID")),
                            SubjectName = reader.IsDBNull(reader.GetOrdinal("c_subject_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_subject_name")),
                            Standard = new Standard()
                            {
                                StandardID = reader.GetInt32(reader.GetOrdinal("c_standardID")),
                                StandardName = reader.IsDBNull(reader.GetOrdinal("c_standard_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_standard_name")),
                            },
                            Teacher = new Teacher()
                            {
                                TeacherID = reader.GetInt32(reader.GetOrdinal("c_teacherID")),
                                User = new User()
                                {
                                    FirstName = reader.IsDBNull(reader.GetOrdinal("c_first_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_first_name")),
                                    LastName = reader.IsDBNull(reader.GetOrdinal("c_last_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("c_last_name"))
                                }
                            }
                        }
                    };
                }

                Console.WriteLine($"TimeTableRepository - GetOne() : No Data");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TimeTableRepository - GetOne() : {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region Update
        public async Task<int> Update(TimeTable data)
        {
            const string query = @"
            UPDATE t_timetable
            SET c_subjectID = @subjectID, c_classID = @classID, c_day_of_week = @dayOfWeek,
                c_start_time = @startTime, c_end_time = @endTime
            WHERE c_timetableID = @timetableID;";

            try
            {
                await _connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@subjectID", data.SubjectID);
                cmd.Parameters.AddWithValue("@classID", data.ClassID);
                cmd.Parameters.AddWithValue("@dayOfWeek", (object?)data.DayOfWeek ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@startTime", data.StartTime);
                cmd.Parameters.AddWithValue("@endTime", data.EndTime);
                cmd.Parameters.AddWithValue("@timetableID", data.TimetableID);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TimeTableRepository - Update() : {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion

        #region GetDayOfWeekName
        private string GetDayOfWeekName(int? dayOfWeek)
        {
            return dayOfWeek switch
            {
                1 => "Sunday",
                2 => "Monday",
                3 => "Tuesday",
                4 => "Wednesday",
                5 => "Thursday",
                6 => "Friday",
                7 => "Saturday",
                _ => "Unknown"
            };
        }
        #endregion
    }
}