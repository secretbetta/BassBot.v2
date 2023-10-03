using Discord.Net;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace BassBotDotNET.SlashCommandModules;

public class SettingSlashCommandModule
{
    public string FieldA { get; set; } = "test";
    public int FieldB { get; set; } = 10;
    public bool FieldC { get; set; } = true;

    public SlashCommandBuilder Command()
    {
        var guildCommand = new SlashCommandBuilder()
            .WithName("settings")
            .WithDescription("Changes some settings within the bot.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("field-a")
                .WithDescription("Gets or sets the field A")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field A")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.String, "the value to set the field", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field A.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            ).AddOption(new SlashCommandOptionBuilder()
                .WithName("field-b")
                .WithDescription("Gets or sets the field B")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field B")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.Integer, "the value to set the fie to.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field B.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            ).AddOption(new SlashCommandOptionBuilder()
                .WithName("field-c")
                .WithDescription("Gets or sets the field C")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field C")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.Boolean, "the value to set the fie to.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field C.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            );
        return guildCommand;
    }

    public async Task HandleSettingsCommand(SocketSlashCommand command)
    {
        // First lets extract our variables
        var fieldName = command.Data.Options.First().Name;
        var getOrSet = command.Data.Options.First().Options.First().Name;
        // Since there is no value on a get command, we use the ? operator because "Options" can be null.
        var value = command.Data.Options.First().Options.First().Options?.FirstOrDefault().Value;

        switch (fieldName)
        {
            case "field-a":
                {
                    if (getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-a` is `{FieldA}`");
                    }
                    else if (getOrSet == "set")
                    {
                        this.FieldA = value == null ? "" : (string)value;
                        await command.RespondAsync($"`field-a` has been set to `{FieldA}`");
                    }
                }
                break;
            case "field-b":
                {
                    if (getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-b` is `{FieldB}`");
                    }
                    else if (getOrSet == "set")
                    {
                        this.FieldB = value == null ? 0 : (int)value;
                        await command.RespondAsync($"`field-b` has been set to `{FieldB}`");
                    }
                }
                break;
            case "field-c":
                {
                    if (getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-c` is `{FieldC}`");
                    }
                    else if (getOrSet == "set")
                    {
                        this.FieldC = value == null ? false : (bool)value;
                        await command.RespondAsync($"`field-c` has been set to `{FieldC}`");
                    }
                }
                break;
        }
    }
}
