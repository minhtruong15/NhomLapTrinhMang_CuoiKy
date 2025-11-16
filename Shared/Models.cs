using System;
using System.Collections.Generic;

namespace Shared
{
    public class UserDto { public string Username { get; set; } public string Role { get; set; } = "user"; }
    public class MessageDto { public string From { get; set; } public string Text { get; set; } public DateTime At { get; set; } }
    public class RoomDto { public string Name { get; set; } public string Code { get; set; } public List<string> Users { get; set; } = new List<string>(); }
}
