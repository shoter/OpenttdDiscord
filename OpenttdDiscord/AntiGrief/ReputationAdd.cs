namespace OpenttdDiscord.AntiGrief
{
    public class ReputationAdd
    {
        public string IpAddres { get; }

        public int Minutes { get; }

        public ReputationAdd(string ipAddress, int minutes)
        {
            this.IpAddres = ipAddress;
            this.Minutes = minutes;
        }
    }
}
