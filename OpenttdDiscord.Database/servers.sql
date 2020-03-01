CREATE TABLE `servers` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `server_ip` varchar(255) NOT NULL,
  `server_port` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UK_servers` (`server_ip`,`server_port`)
)