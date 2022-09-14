namespace robot_controller_api;

public class RobotCommand
{
    /// Implement <see cref="RobotCommand"> here following the task sheet requirements
    /// 

    public int ID { get; set; }
    //public Guid Id { get; set; } // give a random one for now
    public string Name { get; set; }
    public string? Description { get; set; } // ? means it can be null
    public bool IsMoveCommand { get; set; }
    public DateTime CreatedDate { get; set; } // D&T of command creation
    public DateTime ModifiedDate { get; set; } // D&T of command modifications

    // constructor
    
    public RobotCommand( int id,string name, bool isMoveCommand, DateTime createdDate, DateTime modifiedDate , string? description = null)
    {
        ID = id;
        Name = name;
        IsMoveCommand = isMoveCommand;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
        Description = description;
    }
    
}
