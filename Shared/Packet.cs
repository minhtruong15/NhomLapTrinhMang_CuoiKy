using Newtonsoft.Json;

namespace Shared
{
    public class Packet
    {
        public Command Cmd { get; set; }
        public object Data { get; set; }

        public Packet() { }
        public Packet(Command cmd, object data = null)
        {
            Cmd = cmd; Data = data;
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
        public static Packet FromJson(string json) => JsonConvert.DeserializeObject<Packet>(json);
    }
}
