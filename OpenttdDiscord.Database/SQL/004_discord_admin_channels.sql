CREATE TABLE discord_admin_channels (
  server_id BIGINT(20) UNSIGNED NOT NULL,
  channel_id BIGINT(20) UNSIGNED NOT NULL,
  prefix VARCHAR(10) NOT NULL
);