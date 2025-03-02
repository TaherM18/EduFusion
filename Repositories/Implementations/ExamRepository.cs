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
    public class ExamRepository : IExamInterface
    {
        private readonly NpgsqlConnection _connection;

        public ExamRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #region Add
        public async Task<int> Add(Exam data)
        {
            const string query = @"
            INSERT INTO t_exam (c_exam_name, c_subjectID, c_total_marks, c_exam_date, c_start_time, c_duration)
            VALUES (@ExamName, @SubjectID, @TotalMarks, @ExamDate, @StartTime, @Duration)
            RETURNING c_examID;";

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@ExamName", data.ExamName);
                cmd.Parameters.AddWithValue("@SubjectID", data.SubjectID);
                cmd.Parameters.AddWithValue("@TotalMarks", data.TotalMarks);
                cmd.Parameters.AddWithValue("@ExamDate", data.ExamDate);
                cmd.Parameters.AddWithValue("@StartTime", data.StartTime);
                cmd.Parameters.AddWithValue("@Duration", data.Duration);

                int? affectedRows = (int?)await cmd.ExecuteScalarAsync();

                return affectedRows.HasValue ? affectedRows.Value : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - Add() : {ex.Message}");
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
            const string query = "DELETE FROM t_exam WHERE c_examID = @ExamID;";

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@ExamID", id);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - Delete() : {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetAll
        public async Task<List<Exam>> GetAll()
        {
            const string query = @"
            SELECT 
                e.c_examID, e.c_exam_name, e.c_subjectID, e.c_classID, e.c_total_marks, e.c_exam_date, e.c_start_time, e.c_duration,
                s.c_subject_name, s.c_standardID,
                std.c_standard_name,
                c.c_class_name, c.c_wing, c.c_floor
            FROM 
                t_exam e
            INNER JOIN
                t_subject s ON e.c_subjectID = s.c_subjectID
            INNER JOIN
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_class c ON e.c_classID = c.c_classID
            ORDER BY 
                e.c_exam_date ASC;";

            var result = new List<Exam>();

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new Exam
                    {
                        ExamID = reader.GetInt32("c_examID"),
                        ExamName = reader.IsDBNull("c_exam_name") ? "N/A" : reader.GetString("c_exam_name"),
                        SubjectID = reader.GetInt32("c_subjectID"),
                        ClassID = reader.GetInt32("c_classID"),
                        TotalMarks = reader.GetInt32("c_total_marks"),
                        ExamDate = reader.GetDateTime("c_exam_date"),
                        Subject = new Subject()
                        {
                            SubjectID = reader.IsDBNull("c_subjectID") ? 0 : reader.GetInt32("c_subjectID"),
                            SubjectName = reader.IsDBNull("c_subject_name") ? "N/A" : reader.GetString("c_subject_name"),
                            Standard = new Standard()
                            {
                                StandardID = reader.IsDBNull("c_standardID") ? 0 : reader.GetInt32("c_standardID"),
                                StandardName = reader.IsDBNull("c_standard_name") ? "N/A" : reader.GetString("c_standard_name"),
                            }
                        },
                        ClassModel = new ClassModel()
                        {
                            ClassID = reader.IsDBNull("c_classID") ? 0 : reader.GetInt32("c_classID"),
                            ClassName = reader.IsDBNull("c_class_name") ? "N/A" : reader.GetString("c_class_name"),
                            Wing = reader.IsDBNull("c_wing") ? "N/A" : reader.GetString("c_wing"),
                            Floor = reader.IsDBNull("c_floor") ? -1 : reader.GetInt32("c_floor"),
                        }
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - GetAll() : {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetAllByStandard
        public async Task<List<Exam>> GetAllByStandard(int standardID)
        {
            const string query = @"
            SELECT 
                e.c_examID, e.c_exam_name, e.c_subjectID, e.c_classID, e.c_total_marks, e.c_exam_date, e.c_start_time, e.c_duration,
                s.c_subject_name, s.c_standardID,
                std.c_standard_name,
                c.c_class_name, c.c_wing, c.c_floor
            FROM 
                t_exam e
            INNER JOIN
                t_subject s ON e.c_subjectID = s.c_subjectID
            INNER JOIN
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_class c ON e.c_classID = c.c_classID
            WHERE 
                s.c_standardID = @StandardID
            ORDER BY 
                c_exam_date DESC;";

            var result = new List<Exam>();

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@StandardID", standardID);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new Exam
                    {
                        ExamID = reader.GetInt32(reader.GetOrdinal("c_examID")),
                        ExamName = reader.GetString(reader.GetOrdinal("c_exam_name")),
                        SubjectID = reader.GetInt32(reader.GetOrdinal("c_SubjectID")),
                        TotalMarks = reader.GetInt32(reader.GetOrdinal("c_total_marks")),
                        ExamDate = reader.GetDateTime(reader.GetOrdinal("c_exam_date"))
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - GetAllByStandard() : {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetOne
        public async Task<Exam> GetOne(int id)
        {
            const string query = @"
        SELECT c_examID, c_exam_name, c_SubjectID, c_total_marks, c_exam_date
        FROM t_exam
        WHERE c_examID = @ExamID;";

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@ExamID", id);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Exam
                    {
                        ExamID = reader.GetInt32(reader.GetOrdinal("c_examID")),
                        ExamName = reader.GetString(reader.GetOrdinal("c_exam_name")),
                        SubjectID = reader.GetInt32(reader.GetOrdinal("c_SubjectID")),
                        TotalMarks = reader.GetInt32(reader.GetOrdinal("c_total_marks")),
                        ExamDate = reader.GetDateTime(reader.GetOrdinal("c_exam_date"))
                    };
                }
                Console.WriteLine($"[ERROR] ExamRepository - GetOne() : No Data");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - GetOne() : {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region Update
        public async Task<int> Update(Exam data)
        {
            const string query = @"
            UPDATE t_exam 
            SET c_exam_name = @ExamName, c_SubjectID = @SubjectID, 
                c_total_marks = @TotalMarks, c_exam_date = @ExamDate
            WHERE c_examID = @ExamID;";

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@ExamName", data.ExamName);
                cmd.Parameters.AddWithValue("@SubjectID", data.SubjectID);
                cmd.Parameters.AddWithValue("@TotalMarks", data.TotalMarks);
                cmd.Parameters.AddWithValue("@ExamDate", data.ExamDate);
                cmd.Parameters.AddWithValue("@ExamID", data.ExamID);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ExamRepository - Update() : {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion
    }
}