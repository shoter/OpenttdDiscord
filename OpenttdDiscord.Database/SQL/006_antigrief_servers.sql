CREATE TABLE antigrief_servers (
  server_id BIGINT(20) UNSIGNED  NOT NULL,
  required_mins_to_play int(8) UNSIGNED NOT NULL DEFAULT 15,
  reason VARCHAR(500) DEFAULT NULL,
  PRIMARY KEY(server_id) 
);



ALTER TABLE antigrief_servers 
  ADD CONSTRAINT FK_antigrief_servers_server_id FOREIGN KEY (server_id)
    REFERENCES servers(id) ON DELETE NO ACTION;