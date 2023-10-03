using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private DiscordSocketClient? _client;

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;
        _client.Ready += Client_Ready;

        _client.SlashCommandExecuted += SlashCommandHandler;

        // Reading token from token.txt for testing.
        // TODO set up secret management for production.
        var token = File.ReadAllText("token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task Client_Ready()
    {
        ulong guildId = 738870706191728772; // Testing Server

        // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
        var guild = _client.GetGuild(guildId);

        // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
        var firstGuildCommand = new SlashCommandBuilder();

        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        firstGuildCommand.WithName("first-command");

        // Descriptions can have a max length of 100.
        firstGuildCommand.WithDescription("This is my first guild slash command!");

        var listRolesGuild = new Discord.SlashCommandBuilder()
        .WithName("list-roles")
        .WithDescription("Lists all roles of a user.")
        .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true) // Apparently these options cannot have uppercase letters @_@
        .AddOption("isprivate", ApplicationCommandOptionType.Boolean, "Whether the response should be private", isRequired: true);

        // Let's do our global command
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("first-global-command");
        globalCommand.WithDescription("This is my first global slash command");

        try
        {
            // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            await guild.CreateApplicationCommandAsync(firstGuildCommand.Build());
            
            await _client.Rest.CreateGuildCommand(listRolesGuild.Build(), guildId);
            // With global commands we don't need the guild.
            await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
        }
        catch (HttpException exception)
        {
            // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine(json);
        }
    }
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "list-roles":
                await HandleListRoleCommand(command);
                break;
            case "list-roles-private":
                await HandleListRoleCommand(command);
                break;
            // all other commands
            default:
                await command.RespondAsync($"You executed {command.Data.Name}");
                break;
        }
    }

    private async Task HandleListRoleCommand(SocketSlashCommand command)
    {
        // We need to extract the user parameter from the command.
        var parameters = command.Data.Options.ToArray();
        var guildUser = (SocketGuildUser)parameters.First().Value;
        var isPrivate = parameters.Length > 1 && parameters[1].Value.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase);

        // We remove the everyone role and select the mention of each role.
        var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

        var embedBuilder = new EmbedBuilder()
            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            .WithTitle("Roles")
            .WithDescription(roleList)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();

        // Now, Let's respond with the embed.
        await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: isPrivate);
        //await command.RespondAsync(embed: embedBuilder.Build());
    }
}