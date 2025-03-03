using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using Helpers.Files;

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
            string query = "SELECT * FROM t_material";
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
                                MaterialID = Convert.ToInt32(reader["c_materialID"]),
                                FileName = reader["c_file_name"].ToString(),
                                UserID = Convert.ToInt32(reader["c_userID"]),
                                SubjectID = Convert.ToInt32(reader["c_subjectID"])
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
            const string query = "SELECT * FROM t_material WHERE c_materialID = @Id";

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