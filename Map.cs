namespace robot_controller_api;

public class Map
{
    /// Implement <see cref="Map"> here following the task sheet requirements
    /// 

    public int ID { get; set; }
    //public Guid Id { get; set; } // give a random one for now
    public string Name { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public string? Description { get; set; } // ? means it can be null
    public DateTime CreatedDate { get; set; } // D&T of command creation
    public DateTime ModifiedDate { get; set; } // D&T of command modifications

    // constructor

    public Map(int id, string name,  int columns , int rows,  DateTime createdDate,
                        DateTime modifiedDate, string? description = null)
    {
        ID = id;
        Name = name;
        Description = description;
        Columns = columns;
        Rows = rows;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }
}
