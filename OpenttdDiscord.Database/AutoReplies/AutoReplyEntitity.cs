using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;

namespace OpenttdDiscord.Database.AutoReplies
{
    public record AutoReplyEntitity(
        Guid GuildId,
        ulong ServerId,
        string TriggerMessage,
        string ResponseMessage,
        string AdditionalAction)
    {
        public string TriggerMessage { get; set; } = TriggerMessage;
        public string ResponseMessage { get; set; } = ResponseMessage;
        public string AdditionalAction { get; set; } = AdditionalAction;

        [ExcludeFromCodeCoverage]
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutoReplyEntitity>()
                .HasKey(
                    x => new
                    {
                        x.ServerId,
                        x.TriggerMessage,
                    });

            modelBuilder.Entity<AutoReplyEntitity>()
                .HasOne<OttdServerEntity>()
                .WithMany()
                .HasForeignKey(x => x.ServerId);
        }
    }
}