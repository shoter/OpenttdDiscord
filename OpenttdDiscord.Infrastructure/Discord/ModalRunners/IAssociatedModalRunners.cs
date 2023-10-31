namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public interface IAssociatedModalRunners
    {
        Dictionary<string, Type> AssociatedModalRunners { get; }
    }
}