CREATE TABLE trusted_ips (
  ip_address NVARCHAR(50) UNSIGNED NOT NULL,
  playing_time_minutes INT UNSIGNED NOT NULL,
  PRIMARY KEY(ip_address)
);
