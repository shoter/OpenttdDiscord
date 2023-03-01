CREATE TABLE IF NOT EXISTS "__MigrationHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___MigrationHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__MigrationHistory" WHERE "MigrationId" = '20230226001458_initial') THEN
    CREATE TABLE "ChatChannels" (
        "ServerId" uuid NOT NULL,
        "ChannelId" numeric(20,0) NOT NULL,
        "GuildId" numeric(20,0) NOT NULL,
        CONSTRAINT "PK_ChatChannels" PRIMARY KEY ("ServerId", "ChannelId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__MigrationHistory" WHERE "MigrationId" = '20230226001458_initial') THEN
    CREATE TABLE "Servers" (
        "Id" uuid NOT NULL,
        "GuildId" numeric(20,0) NOT NULL,
        "Ip" text NOT NULL,
        "Name" text NOT NULL,
        "AdminPort" integer NOT NULL,
        "AdminPortPassword" text NOT NULL,
        CONSTRAINT "PK_Servers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__MigrationHistory" WHERE "MigrationId" = '20230226001458_initial') THEN
    CREATE TABLE "Monitors" (
        "ServerId" uuid NOT NULL,
        "ChannelId" numeric(20,0) NOT NULL,
        "GuildId" numeric(20,0) NOT NULL,
        "MessageId" numeric(20,0) NOT NULL,
        "LastUpdateTime" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Monitors" PRIMARY KEY ("ServerId", "ChannelId"),
        CONSTRAINT "FK_Monitors_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__MigrationHistory" WHERE "MigrationId" = '20230226001458_initial') THEN
    CREATE UNIQUE INDEX "IX_Servers_Name" ON "Servers" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__MigrationHistory" WHERE "MigrationId" = '20230226001458_initial') THEN
    INSERT INTO "__MigrationHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230226001458_initial', '7.0.2');
    END IF;
END $EF$;
COMMIT;

