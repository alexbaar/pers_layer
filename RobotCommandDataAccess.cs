
// import the elements of the PostgreSQL data provider
using Npgsql;
using robot_controller_api;

namespace robot_controller_api.Persistence;


public class RobotCommandDataAccess
{
    private const string TABLE_NAME = "sit331";

    // This is the connection string. It is used by the data provider to establish a connection
    // to the database.We specify the host name, user name, password and a database name
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=alexolbert;Database=sit331";

    public static List <RobotCommand> GetRobotCommands()
    {
        // RobotCommand = This class is used to perform operations of reading and writing
        // into the database
        var RobotCommands = new List<RobotCommand>();

        //  NpgsqlConnection object is created. This object is used to open a connection to a database.
        //  The using statement releases the database connection resource when the variable goes
        //  out of scope
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        // This line opens the database connection
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);

        // The DataReader object is used to get all the data specified by the SQL query.
        // We can then read all the table rows one by one using the data reader.
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            // read values off the data reader and create a new robotCommand here
            // and then add it to the result list

            var RobotCommand = new RobotCommand();
            RobotCommand.ID = (int)(dr["ID"]);
            RobotCommand.Name = dr["Name"].ToString();
            RobotCommand.IsMoveCommand = (bool)dr["IsMoveCommand"];
            RobotCommand.CreatedDate = (DateTime)dr["CreatedDate"];
            RobotCommand.ModifiedDate = (DateTime)dr["ModifiedDate"];
            RobotCommand.Description = dr["Description"].ToString();

            RobotCommands.Add(RobotCommand);

        }
        return RobotCommands;
    }


    public async Task Add(RobotCommand command)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        string commandText = $"INSERT INTO {TABLE_NAME} (ID, Name, Description, IsMoveCommand, CreatedDate, ModifiedDate) VALUES(@id, @name, @description, @ismovecommand, @createddate, @modifieddate)";
        await using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            cmd.Parameters.AddWithValue("id", command.ID);
            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", command.Description);
            cmd.Parameters.AddWithValue("ismovecommand", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("createddate", command.CreatedDate);
            cmd.Parameters.AddWithValue("modifieddate", command.ModifiedDate);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task <RobotCommand> Get(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        string commandText = $"SELECT * FROM {TABLE_NAME} WHERE ID = @id";
        await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conn))
        {
            cmd.Parameters.AddWithValue("id", id);

            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    RobotCommand command = ReadRobotCommand(reader);
                    return command;
                }
        }
        return null;
    }

    private static RobotCommand ReadRobotCommand(NpgsqlDataReader reader)
    {
        int? id = reader["id"] as int?;
        string name = reader["name"] as string;
        string description = reader["description"] as string;
        bool ismovecommand = (bool)reader["ismovecommand"];
        DateTime createddate = (DateTime)reader["createddate"];
        DateTime modifieddate = (DateTime)reader["modifieddate"];

        RobotCommand command = new RobotCommand
        {
            ID = id.Value,
            Name = name,

            IsMoveCommand = ismovecommand,
            CreatedDate = createddate,
            ModifiedDate = modifieddate,
            Description = description
        };
        return command;
    }

    public async Task Update(int id, RobotCommand command)
    {
        var commandText = $@"UPDATE {TABLE_NAME}
                SET Name = @name, Description =@description, IsMoveCommand=@ismovecommand, CreatedDate=@createddate, ModifiedDate=@modifieddate
                WHERE ID = @id";

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        await using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            cmd.Parameters.AddWithValue("id", command.ID);
            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", command.Description);
            cmd.Parameters.AddWithValue("ismovecommand", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("createddate", command.CreatedDate);
            cmd.Parameters.AddWithValue("modifieddate", command.ModifiedDate);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task Delete(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        string commandText = $"DELETE FROM {TABLE_NAME} WHERE ID=(@p)";
        await using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            cmd.Parameters.AddWithValue("p", id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
