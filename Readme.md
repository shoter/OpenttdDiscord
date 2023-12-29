# Openttd Discord

![GitHub](https://img.shields.io/github/license/shoter/OpenttdDiscord?style=plastic)
![Test badge](https://github.com/shoter/OpenttdDiscord/actions/workflows/test.yml/badge.svg)
[![codecov](https://codecov.io/gh/shoter/OpenttdDiscord/branch/master/graph/badge.svg?token=1EC4AKIMD3)](https://codecov.io/gh/shoter/OpenttdDiscord)

Provides ability to integrate Discord with your OpenTTD servers.

## Documentation

https://shoter.github.io/OpenttdDiscord/

## Functionalities

* Connects Discord channel to the Openttd Servers enabling communication between multiple servers and Discord.
    * Automatically translates ASCII emojis into Discord emojis and vice-versa.
* Ability to execute rcon commands on server from Discord.
* Ability to gather short reports from players about malicious behavior of other players
    * Contains information about connected clients
    * Contains last chat messages
    * Contains reason for the report
    * Some additional information
* Creating auto-updated messages which can show status of current server.
    * It contains the same information as OpenTTD client on server browser screen.
* Auto reply system
    * Players joining server are going to be welcomed with pre-defined message


## Compilation/Installation

* load discord bot token into environment variable `ottd_discord_token`
* Use `run.sh` in order to run bot. (Use `run.pi.sh` if you are using raspberry pi.)
    * Bot automatically creates persistent SQL database on the computer where bot is being run.


* Debugging process is the same excluding the fact that you need to launch your project instead of docker instance. 
It should work out of the box with environment variables set. Remember to load MySql connection string into `ottd_discord_connectionstring` before debugging.
  * Database creation process is described inside [OpenttdDiscord.Database](https://github.com/shoter/OpenttdDiscord/tree/master/OpenttdDiscord.Database)
  * example: `Server=1.2.3.256;User ID=openttd;Password=yoursupersecretpassword;Database=openttd`


## Bots required permissions

- View channels
- Send Messages
- Embed files

Read more about permissions in [official Discord documentation](https://discord.com/developers/docs/topics/permissions)

## Development

### Conventional commits

This repository uses [Conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) to specify type of commits commited to the repository.

Used types:
- feat - feature
- fix
- ref - refactor
- docs - documentation
- nuget - commits connected with nuget upgrade/downgrade
- chore


## Architecture

### Actors hierarchy

![Actors hierarchy](https://github.com/shoter/OpenttdDiscord/obsidian/docs/diagrams/akka.drawio.png)

### Database diagram

![Database diagram](https://github.com/shoter/OpenttdDiscord/obsidian/docs/diagrams/database.drawio.png)

## Test coverage

[![Test coverage graph](https://codecov.io/gh/shoter/OpenttdDiscord/branch/master/graphs/tree.svg?token=1EC4AKIMD3)](https://app.codecov.io/gh/shoter/OpenttdDiscord)
