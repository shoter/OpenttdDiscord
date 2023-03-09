﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OpenttdDiscord.Database;

#nullable disable

namespace OpenttdDiscord.Database.Migrations
{
    [DbContext(typeof(OttdContext))]
    partial class OttdContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OpenttdDiscord.Database.Chatting.ChatChannelEntity", b =>
                {
                    b.Property<Guid>("ServerId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("ServerId", "ChannelId");

                    b.ToTable("ChatChannels");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Ottd.Servers.OttdServerEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AdminPort")
                        .HasColumnType("integer");

                    b.Property<string>("AdminPortPassword")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Rcon.RconChannelEntity", b =>
                {
                    b.Property<Guid>("ServerId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ServerId", "ChannelId");

                    b.ToTable("RconChannels");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Reporting.ReportChannelEntity", b =>
                {
                    b.Property<Guid>("ServerId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("ServerId", "ChannelId");

                    b.ToTable("ReportChannels");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Statuses.StatusMonitorEntity", b =>
                {
                    b.Property<Guid>("ServerId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("ServerId", "ChannelId");

                    b.ToTable("Monitors");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Statuses.StatusMonitorEntity", b =>
                {
                    b.HasOne("OpenttdDiscord.Database.Ottd.Servers.OttdServerEntity", "Server")
                        .WithMany("Monitors")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("OpenttdDiscord.Database.Ottd.Servers.OttdServerEntity", b =>
                {
                    b.Navigation("Monitors");
                });
#pragma warning restore 612, 618
        }
    }
}
