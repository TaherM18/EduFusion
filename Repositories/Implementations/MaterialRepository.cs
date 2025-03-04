using Repositories.Interfaces;
using Repositories.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace EduFusion.Repositories
{


    public class MaterialRepository : IMaterialInterface
    {
        private readonly NpgsqlConnection _dbConnection;

        public MaterialRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Material>> GetAllMaterialsAsync()
        {
            string query = "SELECT * FROM t_material";
            List<Material> materials = new List<Material>();
            try
            {
                _dbConnection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _dbConnection))
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
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error");
            }
            finally
            {
                _dbConnection.Close();
            }
            return materials;

        }

        public async Task<Material?> GetMaterialByIdAsync(int id) // Nullable return type
        {
            const string query = "SELECT * FROM t_material WHERE c_materialID = @Id";

            try
            {
                _dbConnection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _dbConnection))
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
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _dbConnection.Close();
            }

            return null; 
        }

        public async Task<int> AddMaterialAsync(Material material)
        {
            string query = @"
                INSERT INTO t_material (c_file_name, c_userID, c_subjectID) 
                VALUES (@FileName, @UserID, @SubjectID);";

            try
            {
                _dbConnection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _dbConnection))
                {
                    cmd.Parameters.AddWithValue("@FileName", material.FileName);
                    cmd.Parameters.AddWithValue("@UserID", material.UserID);
                    cmd.Parameters.AddWithValue("@SubjectID", material.SubjectID);

                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            string query = "DELETE FROM t_material WHERE c_materialID = @Id";

            try
            {
                _dbConnection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _dbConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

    }
}

