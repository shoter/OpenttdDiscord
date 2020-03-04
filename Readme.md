# Openttd Discord

Openttd Discord bot.


## Compilation/Installation

* run build.sh to build docker image
* load discord bot token into environment variable `ottd_discord_token`
* Load MySQL connection string into environment variable `ottd_discord_connectionstring`
	* Database creation process is described inside `OpenttdDiscord.Database`
	* example: `Server=1.2.3.256;User ID=openttd;Password=yoursupersecretpassword;Database=openttd`
* run run.sh to launch docker with openttd bot loaded. It will automatically take your environment variables.


Debugging process is the same excluding the fact that you need to launch your project instead of docker instance. It should work out of the box with environment variables set.


