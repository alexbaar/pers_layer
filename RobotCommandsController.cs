using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{

    // order
    // RobotCommand(int id, string name, bool isMoveCommand, DateTime createdDate, 
    //DateTime modifiedDate, string? description = null)
    private static readonly List<RobotCommand> _commands = new List<RobotCommand>
    {
       //new() {ID = 100,Name ="LEFT", IsMoveCommand = true, CreatedDate = DateTime.Now, ModifiedDate = DateTime.UtcNow, Description = "turn left"},
       new(100,"LEFT",true,DateTime.Now, DateTime.UtcNow,"turn left"),
       new(101,"RIGHT",true,DateTime.Now, DateTime.UtcNow,"turn right"),
       new(102,"MOVE",true,DateTime.Now, DateTime.UtcNow),
       new(103,"PLACE",false,DateTime.Now, DateTime.UtcNow),
       new(104,"REPORT",false,DateTime.Now, DateTime.UtcNow)
    };

    private static readonly List<RobotCommand> _commands2 = new List<RobotCommand>();

    // Robot commands endpoints here

    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return RobotCommandDataAccess.GetRobotCommands();;
    }
    // same as
    /* public IEnumerable<RobotCommand> GetAllRobotCommands() => _commands; */

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        // OPTION 1

        //foreach(var val in _commands)
        //{
        //    if (val.IsMoveCommand == true)
        //        _commands2.Add(val);
        //}
        //return _commands2;

        // OPTION 2

        // filter according to a condition
        // LINQ = filtering operator that uses "Where" and "OfType"
        var res = from c in _commands where c.IsMoveCommand == true select c;
        foreach(var val in res)
        {
            _commands2.Add(val);
        }
        return _commands2;
    }

    // search by ID

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var cmd = _commands.FirstOrDefault(c => c.ID == id);
    // return cmd   if not null with OK   , else if null return not found
        return cmd is not null ? Ok(cmd) : NotFound();
    }


    // add new CMD

    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if(newCommand is null)
            return BadRequest();
        // is name taken?
        var res = _commands.FirstOrDefault(c => c.Name == newCommand.Name);
        // is ID taken?
        var res2 = _commands.FirstOrDefault(c => c.ID == newCommand.ID);
        
        if (res is not null && res2 is not null)
            return Conflict("The command with this name and/or ID already exists ! Use unique values");

        newCommand.CreatedDate = DateTime.Now;
        newCommand.ModifiedDate = DateTime.Now;
        
        // add new command to the list
        _commands.Add(newCommand);

        return CreatedAtRoute("GetRobotCommand", new { id = newCommand.ID }, newCommand);
    }


    // modify existing
    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        //find it                              check list IDs against the input id
        var oldCommand = _commands.FirstOrDefault(c => c.ID == id);

        // check
        // assume that name should be unique
        var checkName = _commands.FirstOrDefault(c => c.Name == updatedCommand.Name);

        // check 2
        // if new ID does not collide with already existing ID in the list
        var checkID = _commands.FirstOrDefault(c => c.ID == updatedCommand.ID);



        if (oldCommand is null)  // if not found
            return NotFound();

        // we can decide to change only one of the parameters not all, so the below
        // keeps the unchanged values      |    name is unique in our list
        if (updatedCommand.Name is not null && checkName is null)
            oldCommand.Name = updatedCommand.Name;
        // if the updated ID is unique
        if (checkID is null)
            oldCommand.ID = updatedCommand.ID;
        if (updatedCommand.Description is not null)
            oldCommand.Description = updatedCommand.Description;
        oldCommand.IsMoveCommand = updatedCommand.IsMoveCommand;
        oldCommand.ModifiedDate = DateTime.Now; // mark the update

        return NoContent();
    }


    // DELETE 

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        // found?
        var findCommand = _commands.FirstOrDefault(c => c.ID == id);
        // if not found
        if (findCommand is null)
            return NotFound();

        // remove from our cmd list
        _commands.Remove(findCommand);

        return NoContent();
    }
        
}
