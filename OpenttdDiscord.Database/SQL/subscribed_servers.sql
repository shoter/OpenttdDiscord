CREATE TABLE `subscribed_servers` (
  `server_id` bigint(20) unsigned NOT NULL,
  `last_update` datetime NOT NULL,
  `channel_id` bigint(20) unsigned NOT NULL,
  `message_id` bigint(20) unsigned DEFAULT NULL,
  UNIQUE KEY `UK_subscribed_servers` (`server_id`,`channel_id`),
  CONSTRAINT `FK_subscribed_servers_server_id` FOREIGN KEY (`server_id`) REFERENCES `servers` (`id`) ON DELETE NO ACTION
) 