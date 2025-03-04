using System.Data;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class MaterialRepository : IMaterialInterface
    {
        private readonly NpgsqlConnection _connection;

        public MaterialRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        
        #region Add
        public async Task<int> Add(Material material)
        {
            string query = @"
            INSERT INTO t_material (c_file_name, c_userID, c_subjectID) 
            VALUES (@FileName, @UserID, @SubjectID);";

            try
            {   
                await _connection.OpenAsync();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@FileName", material.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserID", material.UserID);
                    cmd.Parameters.AddWithValue("@SubjectID", material.SubjectID ?? (object)DBNull.Value);

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] MaterialRepository - Add() - {ex.Message}");
                return -1;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion


        #region Delete
        public async Task<int> Delete(int id)
        {
            string query = "DELETE FROM t_material WHERE c_materialID = @Id";

            try
            {
                _connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] MaterialRepository - Delete() - {ex.Message}");
                return -1;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion


        #region GetAll
        public async Task<List<Material>?> GetAll()
        {
            string query = @"
            SELECT 
                m.*,
                s.c_subject_name, s.c_marks,
                std.c_standardID, std.c_standard_name,
                u.c_userID, u.c_first_name, u.c_last_name
            FROM 
                t_material m
            INNER JOIN 
                t_subject s ON m.c_subjectID = s.c_subjectID
            INNER JOIN 
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID;";

            List<Material> materials = new List<Material>();
            try
            {
                _connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Material material = new Material
                            {
                                MaterialID = reader.GetInt32("c_materialID"),
                                FileName = reader.IsDBNull("c_file_name") ? string.Empty : reader.GetString("c_file_name"),
                                UserID = reader.GetInt32("c_userID"),
                                SubjectID = reader.GetInt32("c_subjectID"),
                                Subject = new Subject()
                                {
                                    SubjectID = reader.GetInt32("c_subjectID"),
                                    SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name"),
                                    TeacherID = reader.GetInt32("c_userID"),
                                    Standard = new Standard()
                                    {
                                        StandardID = reader.GetInt32("c_standardID"),
                                        StandardName = reader.GetString("c_standard_name"),
                                    },
                                    Teacher = new Teacher()
                                    {
                                        TeacherID = reader.GetInt32("c_userID"),
                                        User = new User()
                                        {
                                            FirstName = reader.GetString("c_first_name"),
                                            LastName = reader.GetString("c_last_name"),
                                        }
                                    }
                                }
                            };
                            materials.Add(material);
                        }
                    }
                }
                return materials;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] MaterialRepository - GetAll() - {ex.Message}");
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion


        #region GetAllByStandard
        public async Task<List<Material>> GetAllByStandard(int standardID)
        {
            string query = @"
            SELECT 
                m.*,
                s.c_subject_name, s.c_marks,
                std.c_standardID, std.c_standard_name,
                u.c_userID, u.c_first_name, u.c_last_name
            FROM 
                t_material m
            INNER JOIN 
                t_subject s ON m.c_subjectID = s.c_subjectID
            INNER JOIN 
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE 
                s.c_standardID = @StandardID";

            List<Material> materials = new List<Material>();
            try
            {
                _connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@StandardID", standardID);

                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Material material = new Material
                            {
                                MaterialID = reader.GetInt32("c_materialID"),
                                FileName = reader.IsDBNull("c_file_name") ? string.Empty : reader.GetString("c_file_name"),
                                UserID = reader.GetInt32("c_userID"),
                                SubjectID = reader.GetInt32("c_subjectID"),
                                Subject = new Subject()
                                {
                                    SubjectID = reader.GetInt32("c_subjectID"),
                                    SubjectName = reader.IsDBNull("c_subject_name") ? string.Empty : reader.GetString("c_subject_name")
                                }
                            };
                            materials.Add(material);
                        }
                    }
                }
                return materials;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] MaterialRepository - GetAll() - {ex.Message}");
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion


        #region GetOne
        public async Task<Material?> GetOne(int id)
        {
            const string query = @"
            SELECT 
                m.*,
                s.c_subject_name, s.c_marks,
                std.c_standardID, std.c_standard_name,
                u.c_userID, u.c_first_name, u.c_last_name
            FROM 
                t_material m
            INNER JOIN 
                t_subject s ON m.c_subjectID = s.c_subjectID
            INNER JOIN 
                t_standard std ON s.c_standardID = std.c_standardID
            INNER JOIN
                t_user u ON s.c_teacherID = u.c_userID
            WHERE
                m.c_materialID = @Id";

            try
            {
                _connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Material
                            {
                                MaterialID = Convert.ToInt32(reader["c_materialID"]),
                                FileName = reader["c_file_name"].ToString(),
                                UserID = Convert.ToInt32(reader["c_userID"]),
                                SubjectID = Convert.ToInt32(reader["c_subjectID"])
                            };
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR] MaterialRepository - GetOne() - No Data");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] MaterialRepository - GetOne() - {ex.Message}");
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion


        #region Update
        public async Task<int> Update(Material material)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}