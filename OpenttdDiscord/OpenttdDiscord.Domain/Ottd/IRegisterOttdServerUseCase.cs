namespace OpenttdDiscord.Domain.Ottd
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        Task Execute(OttdServer server);
    }
}
