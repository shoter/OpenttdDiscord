CREATE TABLE report_servers (
  server_id BIGINT(20) UNSIGNED NOT NULL,
  channel_id BIGINT(20) UNSIGNED NOT NULL
);

ALTER TABLE report_servers 
  ADD UNIQUE INDEX UK_report_servers(server_id, channel_id);

ALTER TABLE report_servers 
  ADD CONSTRAINT FK_report_servers_server_id FOREIGN KEY (server_id)
    REFERENCES servers(id) ON DELETE NO ACTION;