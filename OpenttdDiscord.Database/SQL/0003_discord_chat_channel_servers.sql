CREATE TABLE discord_chat_channel_servers (
  server_id BIGINT(20) UNSIGNED NOT NULL,
  channel_id BIGINT(20) UNSIGNED NOT NULL,
  connect_message_enabled BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (channel_id, server_id)
);

ALTER TABLE discord_chat_channel_servers 
  ADD CONSTRAINT FK_discord_chat_channel_servers_server_id FOREIGN KEY (server_id)
    REFERENCES servers(id) ON DELETE NO ACTION;