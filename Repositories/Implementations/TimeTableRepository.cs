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
    public class TimeTableRepository : ITImeTableInterface
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

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@subjectID", data.SubjectID);
            cmd.Parameters.AddWithValue("@classID", data.ClassID);
            cmd.Parameters.AddWithValue("@dayOfWeek", (object?)data.DayOfWeek ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@startTime", data.StartTime);
            cmd.Parameters.AddWithValue("@endTime", data.EndTime);

            int? affectedRows = (int?)await cmd.ExecuteScalarAsync();

            return affectedRows.HasValue ? affectedRows.Value : 0;
        }
        #endregion


        #region Delete
        public async Task<int> Delete(int id)
        {
            const string query = "DELETE FROM t_timetable WHERE c_timetableID = @id;";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
        #endregion


        #region GetAll
        public Task<List<TimeTable>> GetAll()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region GetAllGroupByDayOfWeek
        public async Task<Dictionary<string, List<TimeTable>>> GetAllGroupByDayOfWeek()
        {
            const string query = "SELECT * FROM t_timetable ORDER BY c_day_of_week, c_start_time;";

            var result = new Dictionary<string, List<TimeTable>>();

            await using var cmd = new NpgsqlCommand(query, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var timeTable = new TimeTable
                {
                    TimetableID = reader.GetInt32("c_timetableID"),
                    SubjectID = reader.GetInt32("c_subjectID"),
                    ClassID = reader.GetInt32("c_classID"),
                    DayOfWeek = reader.IsDBNull("c_day_of_week") ? 0 : reader.GetInt32("c_day_of_week"),
                    StartTime = reader.GetTimeSpan(reader.GetOrdinal("c_start_time")),
                    EndTime = reader.GetTimeSpan(reader.GetOrdinal("c_end_time"))
                };

                string dayName = GetDayOfWeekName(timeTable.DayOfWeek);
                if (!result.ContainsKey(dayName))
                {
                    result[dayName] = new List<TimeTable>();
                }
                result[dayName].Add(timeTable);
            }

            return result;
        }
        #endregion


        #region GetOne
        public async Task<TimeTable?> GetOne(int id)
        {
            const string query = "SELECT * FROM t_timetable WHERE c_timetableID = @id;";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", id);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new TimeTable
                {
                    TimetableID = reader.GetInt32("c_timetableID"),
                    SubjectID = reader.GetInt32("c_subjectID"),
                    ClassID = reader.GetInt32("c_classID"),
                    DayOfWeek = reader.IsDBNull("c_day_of_week") ? 0 : reader.GetInt32("c_day_of_week"),
                    StartTime = reader.GetTimeSpan(reader.GetOrdinal("c_start_time")),
                    EndTime = reader.GetTimeSpan(reader.GetOrdinal("c_end_time"))
                };
            }

            return null;
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

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@subjectID", data.SubjectID);
            cmd.Parameters.AddWithValue("@classID", data.ClassID);
            cmd.Parameters.AddWithValue("@dayOfWeek", (object?)data.DayOfWeek ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@startTime", data.StartTime);
            cmd.Parameters.AddWithValue("@endTime", data.EndTime);
            cmd.Parameters.AddWithValue("@timetableID", data.TimetableID);

            return await cmd.ExecuteNonQueryAsync();
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