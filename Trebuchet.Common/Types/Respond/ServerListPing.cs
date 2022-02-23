using Newtonsoft.Json;

namespace Trebuchet.Common.Types.Respond
{
    public record ServerListPing(VersionData version, PlayerData players, Description description)
    {
        public string Encode()
        {
            // encode data into json
            var json = JsonConvert.SerializeObject(this);
            return json;
        }
    }

    public record VersionData(string name, int protocol);

    public record SmallUserRecord(string name, string id);

    public record PlayerData(int max, int online, SmallUserRecord[] sample);

    public record Description(string text);
}