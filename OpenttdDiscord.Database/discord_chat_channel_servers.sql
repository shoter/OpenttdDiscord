CREATE TABLE openttd_test.discord_chat_channel_servers (
  server_id BIGINT(20) NOT NULL AUTO_INCREMENT,
  channel_id BIGINT(20) NOT NULL AUTO_INCREMENT,
  server_name VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (channel_id, server_id)
)
ENGINE = INNODB;

ALTER TABLE openttd_test.discord_chat_channel_servers 
  ADD CONSTRAINT FK_discord_chat_channel_servers_server_id FOREIGN KEY (server_id)
    REFERENCES openttd_test.() ON DELETE NO ACTION;