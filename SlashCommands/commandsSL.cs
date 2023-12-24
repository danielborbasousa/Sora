using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Sora.SlashCommands
{
    public class commandsSL : ApplicationCommandModule
    {
        [SlashCommand("test", "este é um teste de slash!")]
        public async Task TestSlashCommand(InteractionContext ctx)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Test"
            };
            
            await ctx.Channel.SendMessageAsync(embed : embedMessage);
        }
    }
}
