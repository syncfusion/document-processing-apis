using Npgsql;
using System.Data;

namespace BackgroundWorker
{
    public class JobStorageServiceSql : IJobStorageService
    {
        readonly string _connectionString;
        string _tableName = "Jobs";
        string _tableNameCompleted = "JobsCompleted";
        public JobStorageServiceSql(string connectionString, string tableName)
        {
            _connectionString = connectionString;

            _tableName = tableName;
            _tableNameCompleted = tableName + "_completed";
            if (IsConnected())
            {
                CreateJobQueueTable();
                CreateJobCompletedTable();
            }
        }


        private void CreateJobQueueTable()
        {
            string sql = $@"
                CREATE TABLE IF NOT EXISTS {_tableName} (
                    id VARCHAR(255) PRIMARY KEY,
                    Type VARCHAR(255),
                    Status VARCHAR(255),
                    CreatedTime TIMESTAMP,
                    UpdatedTime TIMESTAMP,
                    Message TEXT,
                    OutputFileName VARCHAR(255),
                    OutputFile TEXT,
                    ErrorMessage TEXT,
                    ErrorStatusCode INT
                );
            ";
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void CreateJobCompletedTable()
        {
            string sql = $@"
                CREATE TABLE IF NOT EXISTS {_tableNameCompleted} (
                    id VARCHAR(255) PRIMARY KEY,
                    Type VARCHAR(255),
                    Status VARCHAR(255),
                    CreatedTime TIMESTAMP,
                    UpdatedTime TIMESTAMP,
                    Message TEXT,
                    OutputFileName VARCHAR(255),
                    OutputFile TEXT,
                    ErrorMessage TEXT,
                    ErrorStatusCode INT
                );
            ";
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }


        public async Task CompleteJobAsync(Job job)
        {
            job.UpdatedTime = DateTime.UtcNow;
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"UPDATE {_tableNameCompleted} SET Status = @status, UpdatedTime = @UpdatedTime, OutputFileName = @OutputFileName, OutputFile = @OutputFile WHERE id = @id";
            cmd.Parameters.AddWithValue("status", "completed");
            cmd.Parameters.AddWithValue("UpdatedTime", job.UpdatedTime);
            cmd.Parameters.AddWithValue("OutputFileName", job.OutputFileName);
            cmd.Parameters.AddWithValue("OutputFile", job.OutputFile);
            cmd.Parameters.AddWithValue("id", job.ID);
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task CompleteJobWithErrorAsync(Job job)
        {
            job.UpdatedTime = DateTime.UtcNow;
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"UPDATE {_tableNameCompleted} SET Status = @status, UpdatedTime = @UpdatedTime, ErrorMessage = @ErrorMessage, ErrorStatusCode = @ErrorStatusCode WHERE id = @id";
            cmd.Parameters.AddWithValue("status", "error");
            cmd.Parameters.AddWithValue("UpdatedTime", job.UpdatedTime);
            cmd.Parameters.AddWithValue("ErrorMessage", job.ErrorMessage);
            cmd.Parameters.AddWithValue("ErrorStatusCode", job.ErrorStatusCode);
            cmd.Parameters.AddWithValue("id", job.ID);
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task<Job?> DequeueJobAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand($@"SELECT * FROM {_tableName} WHERE status = @status LIMIT 1 FOR UPDATE SKIP LOCKED");
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("status", "queued");
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                Job job = new Job
                {
                    ID = reader.GetString(0),
                    Type = reader.GetString(1),
                    Status = reader.GetString(2),
                    CreatedTime = reader.GetDateTime(3),
                    UpdatedTime = reader.GetDateTime(4),
                    Message = reader.GetString(5),
                    OutputFileName = reader.GetString(6),
                    OutputFile = reader.GetString(7),
                    ErrorMessage = reader.GetString(8),
                    ErrorStatusCode = reader.GetInt32(9)
                };
                reader.Close();
                await DeleteJob(connection, job);
                await AddJobToCompletd(connection, job);
                connection.Close();
                return job;
            }
            else
            {
                reader.Close();
                connection.Close();
                return null;
            }
        }



        private async Task DeleteJob(NpgsqlConnection connection, Job job)
        {
            using var cmd = new NpgsqlCommand($@"
                DELETE FROM {_tableName} WHERE id = @id;");
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("id", job.ID);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task AddJobToCompletd(NpgsqlConnection connection, Job job)
        {

            using var cmd = new NpgsqlCommand($@"
                INSERT INTO {_tableNameCompleted} (id, Type, Status, CreatedTime, UpdatedTime, Message, OutputFileName, OutputFile, ErrorMessage, ErrorStatusCode)
                VALUES (@id, @Type, @Status, @CreatedTime, @UpdatedTime, @Message, @OutputFileName, @OutputFile, @ErrorMessage, @ErrorStatusCode);");
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("id", job.ID);
            cmd.Parameters.AddWithValue("Type", job.Type);
            cmd.Parameters.AddWithValue("Status", job.Status);
            cmd.Parameters.AddWithValue("CreatedTime", job.CreatedTime);
            cmd.Parameters.AddWithValue("UpdatedTime", job.UpdatedTime);
            cmd.Parameters.AddWithValue("Message", job.Message);
            cmd.Parameters.AddWithValue("OutputFileName", job.OutputFileName);
            cmd.Parameters.AddWithValue("OutputFile", job.OutputFile);
            cmd.Parameters.AddWithValue("ErrorMessage", job.ErrorMessage);
            cmd.Parameters.AddWithValue("ErrorStatusCode", job.ErrorStatusCode);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EnqueueJobAsync(Job job)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand($@"
                INSERT INTO {_tableName} (id, Type, Status, CreatedTime, UpdatedTime, Message, OutputFileName, OutputFile, ErrorMessage, ErrorStatusCode)
                VALUES (@id, @Type, @Status, @CreatedTime, @UpdatedTime, @Message, @OutputFileName, @OutputFile, @ErrorMessage, @ErrorStatusCode);");
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("id", job.ID);
            cmd.Parameters.AddWithValue("Type", job.Type);
            cmd.Parameters.AddWithValue("Status", "queued");
            cmd.Parameters.AddWithValue("CreatedTime", job.CreatedTime);
            cmd.Parameters.AddWithValue("UpdatedTime", job.UpdatedTime);
            cmd.Parameters.AddWithValue("Message", job.Message);
            cmd.Parameters.AddWithValue("OutputFileName", job.OutputFileName);
            cmd.Parameters.AddWithValue("OutputFile", job.OutputFile);
            cmd.Parameters.AddWithValue("ErrorMessage", job.ErrorMessage);
            cmd.Parameters.AddWithValue("ErrorStatusCode", job.ErrorStatusCode);
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task<Job?> GetJobByID(string jobID)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"SELECT * FROM {_tableNameCompleted} WHERE id = @id";
            cmd.Parameters.AddWithValue("id", jobID);
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                Job job = new Job
                {
                    ID = reader.GetString(0),
                    Type = reader.GetString(1),
                    Status = reader.GetString(2),
                    CreatedTime = reader.GetDateTime(3),
                    UpdatedTime = reader.GetDateTime(4),
                    Message = reader.GetString(5),
                    OutputFileName = reader.GetString(6),
                    OutputFile = reader.GetString(7),
                    ErrorMessage = reader.GetString(8),
                    ErrorStatusCode = reader.GetInt32(9)
                };
                reader.Close();
                connection.Close();
                return job;
            }
            else
            {
                reader.Close();
                connection.Close();
                return null;
            }
        }

        public async Task<string> GetJobStatus(string jobID)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"SELECT Status FROM {_tableNameCompleted} WHERE id = @id";
            cmd.Parameters.AddWithValue("id", jobID);
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {

                string status = reader.GetString(0);
                reader.Close();
                connection.Close();
                return status;
            }
            else
            {
                reader.Close();
                string status = await GetStatusFromQueue(connection, jobID);
                connection.Close();
                return status;
            }
        }

        private async Task<string> GetStatusFromQueue(NpgsqlConnection connection, string jobID)
        {
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"SELECT Status FROM {_tableName} WHERE id = @id";
            cmd.Parameters.AddWithValue("status", "queued");
            cmd.Parameters.AddWithValue("id", jobID);
            var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                string status = reader.GetString(0);
                reader.Close();
                return status;
            }
            else
            {
                reader.Close();
                return "";
            }
        }
        public bool IsConnected()
        {
            bool connected = false;
            int retries = 0;
            while (!connected && retries < 3) // Retry a few times
            {
                try
                {
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        conn.Open();
                        connected = true;
                    }
                }
                catch (PostgresException ex)
                {
                    // Check if the exception indicates a transient error (e.g., database starting up)
                    if (ex.SqlState == "57P03")
                    {
                        retries++;
                        System.Threading.Thread.Sleep(15000); // Wait for 5 seconds before retrying
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            return connected;
        }


        public async Task SetStatus(string jobID, string status)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = $"UPDATE {_tableNameCompleted} SET Status = @status WHERE id = @id";
            command.Parameters.AddWithValue("status", status);
            command.Parameters.AddWithValue("id", jobID);
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task DeleteJob(string ID)
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand($@"
                DELETE FROM {_tableNameCompleted} WHERE id = @id;");
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("id", ID);
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task<List<Job>> GetCompletedJobs()
        {
            string time = Environment.GetEnvironmentVariable("RESOURCE_EXPIRATION_TIME") ?? "30";
            var ids = new List<Job>();
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"SELECT * FROM {_tableNameCompleted} WHERE Status = @status AND UpdatedTime < NOW() - INTERVAL '{time} minutes' LIMIT 10";
            cmd.Parameters.AddWithValue("status", "completed");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    Job job = new Job
                    {
                        ID = reader.GetString(0),
                        Type = reader.GetString(1),
                        Status = reader.GetString(2),
                        CreatedTime = reader.GetDateTime(3),
                        UpdatedTime = reader.GetDateTime(4),
                        Message = reader.GetString(5),
                        OutputFileName = reader.GetString(6),
                        OutputFile = reader.GetString(7),
                        ErrorMessage = reader.GetString(8),
                        ErrorStatusCode = reader.GetInt32(9)
                    };
                    ids.Add(job);
                }
            }
            connection.Close();
            return ids;
        }
    }
}
