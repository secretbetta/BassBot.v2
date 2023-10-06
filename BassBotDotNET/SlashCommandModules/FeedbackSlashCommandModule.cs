using Discord.Net;
using Discord;
using Newtonsoft.Json;
using Discord.WebSocket;

namespace BassBotDotNET.SlashCommandModules;

public class FeedbackSlashCommandModule
{

    /// <summary>
    /// The feedback command with preset rating options.
    /// </summary>
    /// <returns>The command builder to use.</returns>
    public SlashCommandBuilder Command()
    {
        var guildCommand = new SlashCommandBuilder()
            .WithName("feedback")
            .WithDescription("Tell us how much you are enjoying this bot!")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("rating")
                .WithDescription("The rating your willing to give our bot")
                .WithRequired(true)
                .AddChoice("Terrible", 1)
                .AddChoice("Meh", 2)
                .AddChoice("Good", 3)
                .AddChoice("Lovely", 4)
                .AddChoice("Excellent!", 5)
                .WithType(ApplicationCommandOptionType.Integer)
            );

        return guildCommand;
    }

    /// <summary>
    /// Handles the feedback command with the rating option.
    /// </summary>
    /// <param name="command">The command options from the user.</param>
    public async Task HandleFeedbackCommand(SocketSlashCommand command)
    {
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(command.User)
            .WithTitle("Feedback")
            .WithDescription($"Thanks for your feedback! You rated us {command.Data.Options.First().Value}/5")
            .WithColor(Color.Green)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuilder.Build());
    }
}
