# Register Rcon Channel

## Syntax

`/register-rcon-channel server-name:{name} prefix:{prefix}`

## Privilege required

- Admin


## Description

Registers channel on which command was executed as rcon channel for given server.  
After registration all messages prefixed with {prefix} are going to be executed as rcon commands on the server.

## Example

Example of rcon channel which has `!` as {prefix}

![Rcon channel example](image.png)