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
    public class StandardRepository : IStandardInterface
    {
        private readonly NpgsqlConnection _connection;

        public StandardRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        #region Add
        public async Task<int> Add(Standard data)
        {
            const string query = @"
            INSERT INTO t_standard 
                (c_standard_name)
            VALUES
                (@StandardName)
            RETURNING
                c_standardID;";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@StandardName", data.StandardName);

            await _connection.OpenAsync();
            int newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await _connection.CloseAsync();

            return newId;
        }
        #endregion


        #region  Delete
        public async Task<int> Delete(int id)
        {
            const string query = "DELETE FROM t_standard WHERE c_standardID = @StandardID;";

            await using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@StandardID", id);

            await _connection.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();

            return rowsAffected;
        }
        #endregion


        #region GetAll
        public async Task<List<Standard>> GetAll()
        {
            const string query = @"
            SELECT 
                std.c_standardID, std.c_standard_name, sub.c_subjectID, sub.c_subject_name, sub.c_marks
            FROM 
                t_standard std
            LEFT JOIN 
                t_subject sub ON std.c_standardID = sub.c_standardID
            ORDER BY 
                std.c_standardID;";

            var standards = new Dictionary<int, Standard>();
            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int standardId = reader.GetInt32("c_standardID");
                    string standardName = reader.GetString("c_standard_name");
                    int? subjectId = reader.IsDBNull("c_subjectID") ? null : reader.GetInt32("c_subjectID");
                    string? subjectName = reader.IsDBNull("c_subject_name") ? null : reader.GetString("c_subject_name");
                    float? marks = reader.IsDBNull("c_marks") ? 0 : reader.GetFloat("c_marks");

                    if (!standards.ContainsKey(standardId))
                    {
                        standards[standardId] = new Standard
                        {
                            StandardID = standardId,
                            StandardName = standardName,
                            Subjects = new List<Subject>()
                        };
                    }

                    if (subjectId.HasValue)
                    {
                        standards[standardId].Subjects?.Add(new Subject
                        {
                            SubjectID = subjectId.Value,
                            SubjectName = subjectName ?? "N/A",
                            Marks = marks
                        });
                    }
                }
                return new List<Standard>(standards.Values);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StandardRepository - GetAll() - {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region GetOne
        public async Task<Standard> GetOne(int id)
        {
            const string query = @"
            SELECT s.c_standardID, s.c_standard_name, sub.c_subjectID, sub.c_subject_name
            FROM t_standard s
            LEFT JOIN t_subject sub ON s.c_standardID = sub.c_standardID
            WHERE s.c_standardID = @StandardID;";

            Standard? standard = null;

            try
            {
                await _connection.OpenAsync();
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@StandardID", id);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int standardId = reader.GetInt32(0);
                    string standardName = reader.GetString(1);
                    int? subjectId = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                    string? subjectName = reader.IsDBNull(3) ? null : reader.GetString(3);

                    if (standard == null)
                    {
                        standard = new Standard()
                        {
                            StandardID = standardId,
                            StandardName = standardName,
                            Subjects = new List<Subject>()
                        };
                    }

                    if (subjectId.HasValue)
                    {
                        standard.Subjects.Add(new Subject()
                        {
                            SubjectID = subjectId.Value,
                            SubjectName = subjectName ?? "N/A"
                        });
                    }
                }

                return standard ?? throw new KeyNotFoundException("Standard not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StandardRepository - GetOne() - {ex.Message}");
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        #endregion


        #region Update
        public async Task<int> Update(Standard data)
        {
            const string query = @"
            UPDATE t_standard 
            SET c_standard_name = @StandardName 
            WHERE c_standardID = @StandardID;";

            try
            {
                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@StandardName", data.StandardName);
                cmd.Parameters.AddWithValue("@StandardID", data.StandardID ?? (object)DBNull.Value);

                await _connection.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                await _connection.CloseAsync();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StandardRepository - Update() - {ex.Message}");
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