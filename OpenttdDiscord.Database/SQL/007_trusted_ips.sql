CREATE TABLE trusted_ips (
  ip_address VARCHAR(50) NOT NULL,
  playing_time_minutes INT(8) UNSIGNED NOT NULL,
  PRIMARY KEY(ip_address)
);
