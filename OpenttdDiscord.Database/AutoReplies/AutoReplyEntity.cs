using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Database.AutoReplies
{
    public record AutoReplyEntity(
        ulong GuildId,
        Guid ServerId,
        string TriggerMessage,
        string ResponseMessage,
        string AdditionalAction)
    {
        public string TriggerMessage { get; set; } = TriggerMessage;
        public string ResponseMessage { get; set; } = ResponseMessage;
        public string AdditionalAction { get; set; } = AdditionalAction;

        public AutoReply ToDomain() => new(
            TriggerMessage,
            ResponseMessage,
            Enum.Parse<AutoReplyAction>(AdditionalAction));

        [ExcludeFromCodeCoverage]
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutoReplyEntity>()
                .HasKey(
                    x => new
                    {
                        x.ServerId,
                        x.TriggerMessage,
                    });

            modelBuilder.Entity<AutoReplyEntity>()
                .HasOne<OttdServerEntity>()
                .WithMany()
                .HasForeignKey(x => x.ServerId);
        }
    }
}