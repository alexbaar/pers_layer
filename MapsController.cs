using System; // math class
using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private static readonly List<Map> _maps = new List<Map>
    {
        // maps here
        new(15, "Germany", 17, 18 ,DateTime.Now, DateTime.UtcNow, "a map of Germany"),
        new(16, "Poland", 15, 15 ,DateTime.Now, DateTime.UtcNow, "a map of Poland"),
        new(17, "Austraila", 67, 88 ,DateTime.Now, DateTime.UtcNow),
        new(18, "Canada", 107, 107 ,DateTime.Now, DateTime.UtcNow),
        new(19, "Hungary", 13, 15 ,DateTime.Now, DateTime.UtcNow, "a map of Hungary"),
    };

    // Endpoints here


    private static readonly List<Map> _maps2 = new List<Map>();

    // Robot commands endpoints here

    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        return _maps;
    }
    // same as
    /* public IEnumerable<RobotCommand> GetAllRobotCommands() => _commands; */

    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {

        foreach (var val in _maps)
        {
            if (val.Columns == val.Rows)
                _maps2.Add(val);
        }

        // return a new array where we placed the matches for our condition above
        return _maps2;
    }

    // search by ID

    [HttpGet("{id}", Name = "GetMapByID")]
    public IActionResult GetMapByID(int id)
    {
        var map = _maps.FirstOrDefault(m => m.ID == id);
        // return cmd   if not null with OK   , else if null return not found
        return map is not null ? Ok(map) : NotFound();
    }


    // add a new map

    [HttpPost()]
    public IActionResult AddNewMap(Map newMap)
    {
        if (newMap is null)
            return BadRequest();

        // Like with the commands, I will filter the Names and IDs among the ones that
        // already exist, as those values should be unique to each map instance
        // stuff like columns, description, date can be duplicated

        // is name taken?
        var res = _maps.FirstOrDefault(c => c.Name == newMap.Name);
        // is ID taken?
        var res2 = _maps.FirstOrDefault(c => c.ID == newMap.ID);

        if (res is not null && res2 is not null)
            return Conflict("A map with this name and/or ID already exists ! Use unique values");

        newMap.CreatedDate = DateTime.Now;
        newMap.ModifiedDate = DateTime.Now;

        // add new command to the list
        _maps.Add(newMap);

        return CreatedAtRoute("GetRobotCommand", new { id = newMap.ID }, newMap);
    }


    // modify existing
    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        //find it                     check list IDs against the input id
        var oldMap = _maps.FirstOrDefault(c => c.ID == id);

        // check
        // assume that name should be unique
        var checkName = _maps.FirstOrDefault(c => c.Name == updatedMap.Name);

        // check 2
        // if new ID does not collide with already existing ID in the list
        var checkID = _maps.FirstOrDefault(c => c.ID == updatedMap.ID);



        if (oldMap is null)  // if not found
            return NotFound();

        // we can decide to change only one of the parameters not all, so the below
        // keeps the unchanged values      |    name is unique in our list
        if (updatedMap.Name is not null && checkName is null)
            oldMap.Name = updatedMap.Name;
        // if the updated ID is unique
        if (checkID is null)
            oldMap.ID = updatedMap.ID;
        if (updatedMap.Description is not null)
            oldMap.Description = updatedMap.Description;
        oldMap.Columns = updatedMap.Columns;
        oldMap.Rows = updatedMap.Rows;
        oldMap.ModifiedDate = DateTime.Now;  // mark the update

        return NoContent();
    }


    // DELETE 

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        // found?
        var findMap = _maps.FirstOrDefault(c => c.ID == id);
        // if not found
        if (findMap is null)
            return NotFound();

        // remove from our cmd list
        _maps.Remove(findMap);

        return NoContent();
    }

    // x, y belong?
    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        bool isOnMap = false;

        // negative coordinates
        if (Math.Sign(x) == -1 || Math.Sign(y) == -1)
            return BadRequest();

        // found?
        var findMap = _maps.FirstOrDefault(c => c.ID == id);
        // if not found
        if (findMap is null)
            return NotFound();

        // so the way I understand is that on a coordinate map we start at 0,0 being x,y
        // and then we specify how many x 'blocks' to the right, and y 'blocks' down
        // so I use LinQ to check if the provided x and y can be found in ranges starting 0,0 
        // and ending at the Columns/Rows values of the selected by ID map 

        else if(Enumerable.Range(0,findMap.Columns).Contains(x) && Enumerable.Range(0,findMap.Rows).Contains(y))
            isOnMap = true;  // point belongs to map

        return Ok(isOnMap); // returns true or false
    }

}
