using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;


namespace Sora.commands
{
    public class TestCommands : BaseCommandModule
    {
        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Opa! Tudo bom?");
        }

        [Command("add")]
        public async Task Add(CommandContext ctx, int number1, int number2)
        {
            int result = number1 + number2;
            await ctx.Channel.SendMessageAsync(result.ToString());
        }

        [Command("hello")]
        public async Task Hello(CommandContext ctx)
        {
            await ctx.RespondAsync($"Olá, {ctx.User.Mention}!");
        }

        [Command("userinfo")]
        public async Task UserInfo(CommandContext ctx)
        {
            var user = ctx.User;
            await ctx.RespondAsync($"Você é {user.Username}#{user.Discriminator}, e seu ID é {user.Id}.");
        }


        [Command("serverinfo")]
        public async Task ServerInfo(CommandContext ctx)
        {
            var server = ctx.Guild;
            await ctx.RespondAsync($"Este servidor se chama {server.Name} e foi criado em {server.CreationTimestamp}. Ele tem {server.MemberCount} membros.");
        }

        [Command("roll")]
        public async Task Roll(CommandContext ctx, int sides = 6)
        {
            if (sides < 2)
            {
                await ctx.RespondAsync("Número de lados do dado deve ser pelo menos 2.");
                return;
            }

            var random = new Random();
            var result = random.Next(1, sides + 1);
            await ctx.RespondAsync($"Você rolou um dado de {sides} lados e obteve: {result}!");
        }

        [Command("avatar")]
        public async Task Avatar(CommandContext ctx, DiscordMember member = null)
        {
            var targetMember = member ?? ctx.Member;
            await ctx.RespondAsync($"Avatar de {targetMember.Username}: {targetMember.AvatarUrl}");
        }

        [Command("joke")]
        public async Task Joke(CommandContext ctx)
        {
            // Substitua este exemplo por um mecanismo para fornecer piadas aleatórias
            var jokes = new string[] { "Por que o pássaro não usa Facebook? Porque ele já tem Twitter!", "O que o zero falou para o oito? Gosto do seu cinto novo!" };
            var random = new Random();
            var joke = jokes[random.Next(jokes.Length)];

            await ctx.RespondAsync($"Piada do dia: {joke}");
        }

        [Command("infos")]
        public async Task EmbedMessage(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder()
            {
                Title = "Hayashi Sora",
                Description = "Prazer! Sou uma simples mortal que vaga pelo mundo em busca de conhecimento e aprendizado!",
                Color = DiscordColor.DarkRed
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }
        [Command("members")]
        public async Task Members(CommandContext ctx)
        {
            var memberCount = ctx.Guild.MemberCount;
            await ctx.RespondAsync($"O servidor tem {memberCount} membros.");
        }
        [Command("infousuario")]
        public async Task UserInfo(CommandContext ctx, DiscordUser user = null)
        {
            if (user == null)
            {
                user = ctx.User;
            }
            var embed = new DiscordEmbedBuilder
            {
                Title = "ℹ️ Informações do Usuário:",
                Description = $"Informações sobre {user.Mention}",
                Color = new DiscordColor(52, 152, 219),
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl }
            };

            if (user is DiscordMember member)
            {
                embed.AddField("ID", member.Id.ToString(), inline: true);
                embed.AddField("Apelido", member.Username, inline: true);
                embed.AddField("Conta Criada em", user.CreationTimestamp.ToString("yyyy-MM-dd HH:mm:ss"), inline: true);

                // Não é necessário verificar se JoinedAt é nulo, pois não é uma propriedade nullable
                embed.AddField("Conta Entrou em", member.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "Não disponível", inline: true);
            }

            await ctx.RespondAsync(embed: embed);
        }
        [Command("ban")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember member)
        {
            await member.BanAsync();
            await ctx.RespondAsync($"Usuário {member.Username} foi banido!");
        }
        [Command("kick")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, DiscordMember member)
        {
            await member.RemoveAsync();
            await ctx.RespondAsync($"Usuário {member.Username} foi expulso!");
        }
        [Command("clear")]
        [Description("Limpa mensagens no canal.")]
        public async Task Clear(CommandContext ctx, int amount = 5)
        {
            var messages = await ctx.Channel.GetMessagesAsync(amount);
            await ctx.Channel.DeleteMessagesAsync(messages);
        }
        [Command("contarcaracteres")]
        public async Task CountCharacters(CommandContext ctx, [RemainingText] string text)
        {
            int characterCount = text.Length;
            await ctx.RespondAsync($"A mensagem tem {characterCount} caracteres.");
        }
        [Command("escolher")]
        public async Task Choose(CommandContext ctx, params string[] options)
        {
            if (options.Length < 2)
            {
                await ctx.RespondAsync("Você precisa fornecer pelo menos duas opções para escolher!");
                return;
            }

            Random random = new Random();
            string chosenOption = options[random.Next(options.Length)];
            await ctx.RespondAsync($"Escolhi: {chosenOption}");
        }
        [Command("comandos")]
        public async Task AvailableCommands(CommandContext ctx)
        {
            var commands = ctx.CommandsNext.RegisteredCommands.Values
                .Where(c => !c.IsHidden)
                .Select(c => c.Name);

            await ctx.RespondAsync($"Comandos disponíveis: {string.Join(", ", commands)}");
        }
        [Command("enquete")]
        public async Task Poll(CommandContext ctx, string question, params string[] options)
        {
            if (options.Length < 2)
            {
                await ctx.RespondAsync("Você precisa fornecer pelo menos duas opções para a enquete!");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = "📊 Enquete:",
                Description = question,
                Color = new DiscordColor(139, 0, 0),
            };

            foreach (var (option, index) in options.Select((option, index) => (option, index)))
            {
                embed.AddField($"Opção {index + 1}", option, inline: true);
            }

            var message = await ctx.RespondAsync(embed: embed);

            for (int i = 0; i < options.Length; i++)
            {
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{(char)('a' + i)}:"));
            }
        }
        [Command("ship")]
        public async Task Ship(CommandContext ctx, DiscordUser user1, DiscordUser user2)
        {
            // Obtém os membros dos usuários mencionados
            var guild = await ctx.Client.GetGuildAsync(ctx.Guild.Id);
            var member1 = await guild.GetMemberAsync(user1.Id);
            var member2 = await guild.GetMemberAsync(user2.Id);

            // Gera um número aleatório para a porcentagem de ship
            Random random = new Random();
            int shipPercentage = random.Next(0, 101);

            // Cria um embed para a mensagem de ship
            var embed = new DiscordEmbedBuilder
            {
                Title = "💖 Ship!",
                Description = $"**{member1.DisplayName}** e **{member2.DisplayName}** foram shipados!",
                Color = new DiscordColor(139, 0, 0)
            };

            // Adiciona um campo para a porcentagem de ship
            embed.AddField("Porcentagem de Ship", $"{shipPercentage}%", inline: true);

            // Mensagens diferentes com base na porcentagem
            if (shipPercentage <= 20)
            {
                embed.AddField("Resultado", "Parece que não há muita química entre eles. 😢", inline: false);
            }
            else if (shipPercentage <= 50)
            {
                embed.AddField("Resultado", "Há um pouco de química, quem sabe o que o futuro reserva? 🤔", inline: false);
            }
            else if (shipPercentage <= 80)
            {
                embed.AddField("Resultado", "Eles têm uma boa química! 👏", inline: false);
            }
            else
            {
                embed.AddField("Resultado", "É uma combinação perfeita! 💖", inline: false);
            }

            // Envia o embed no canal
            await ctx.RespondAsync(embed: embed);
        }
        [Command("add")]
        [Description("Adiciona dois números.")]
        public async Task Add(CommandContext ctx, double num1, double num2)
        {
            await ctx.RespondAsync($"Resultado: {num1 + num2}");
        }

        [Command("subtract")]
        [Description("Subtrai dois números.")]
        public async Task Subtract(CommandContext ctx, double num1, double num2)
        {
            await ctx.RespondAsync($"Resultado: {num1 - num2}");
        }

        [Command("multiply")]
        [Description("Multiplica dois números.")]
        public async Task Multiply(CommandContext ctx, double num1, double num2)
        {
            await ctx.RespondAsync($"Resultado: {num1 * num2}");
        }

        [Command("divide")]
        [Description("Divide dois números.")]
        public async Task Divide(CommandContext ctx, double num1, double num2)
        {
            if (num2 == 0)
            {
                await ctx.RespondAsync("Não é possível dividir por zero.");
                return;
            }

            await ctx.RespondAsync($"Resultado: {num1 / num2}");
        }

        // Adicione outros comandos matemáticos conforme necessário

        // Exemplo: Potência
        [Command("power")]
        [Description("Calcula a potência de um número.")]
        public async Task Power(CommandContext ctx, double num, double exponent)
        {
            await ctx.RespondAsync($"Resultado: {Math.Pow(num, exponent)}");
        }

        // Exemplo: Raiz quadrada
        [Command("sqrt")]
        [Description("Calcula a raiz quadrada de um número.")]
        public async Task Sqrt(CommandContext ctx, double num)
        {
            if (num < 0)
            {
                await ctx.RespondAsync("Não é possível calcular a raiz quadrada de um número negativo.");
                return;
            }

            await ctx.RespondAsync($"Resultado: {Math.Sqrt(num)}");
        }
        public class RPCommands : BaseCommandModule
        {
            [Command("como-fazer-rp")]
            [Description("Fornece instruções sobre como fazer roleplay.")]
            public async Task ComoFazerRP(CommandContext ctx)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Como fazer Roleplay",
                    Description = "Para iniciar uma interação de roleplay (RP), você pode seguir as seguintes convenções:\n" +
                                  "`//` para fora do personagem (OOC - Out of Character)\n" +
                                  "`-` para fala\n" +
                                  "** ** para ações\n\n" +
                                  "Exemplo:\n" +
                                  "**Personagem 1**: - Olá, como você está?\n" +
                                  "**Personagem 2**: - Estou bem, obrigado!",
                    Color = DiscordColor.Green
                };

                await ctx.RespondAsync(embed);
            }

            [Command("clear-mensagens")]
            [Description("Limpa todas as mensagens que contêm //.")]
            [RequirePermissions(Permissions.ManageMessages)]
            public async Task ClearMensagens(CommandContext ctx)
            {
                var messagesToDelete = await ctx.Channel.GetMessagesAsync();
                var messagesToDeleteFiltered = messagesToDelete.Where(msg => msg.Content.Contains("//"));

                if (messagesToDeleteFiltered.Any())
                {
                    await ctx.Channel.DeleteMessagesAsync(messagesToDeleteFiltered);
                    await ctx.RespondAsync($"Mensagens com `//` foram removidas.");
                }
                else
                {
                    await ctx.RespondAsync("Não há mensagens com `//` para remover.");
                }
            }
        }
        public class CustomEmbedCommandModule : BaseCommandModule
        {
            [Command("customembed")]
            [Description("Cria um embed personalizado com título, descrição, imagem e cor vermelha escura.")]
            public async Task CreateCustomEmbed(CommandContext ctx,
                                                string title = "Mensagem Embed Personalizada",
                                                string description = "Conteúdo da mensagem embed.",
                                                string imageUrl = "")
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = title,
                    Description = description,
                    Color = new DiscordColor(139, 0, 0), // Código RGB para vermelho escuro
                };

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    embed.ImageUrl = imageUrl;
                }

                await ctx.RespondAsync(embed);
            }
            public class InsultCommandModule : BaseCommandModule
            {
                private static List<string> Insults = new List<string>
    {
        "Seu código é tão bagunçado que até o Garbage Collector desiste dele.",
        "Você é a razão pela qual não podemos ter coisas boas no código."
    };

                [Command("insult")]
                [Description("Fornecer um insulto criativo.")]
                public async Task Insult(CommandContext ctx, [RemainingText] string target = "")
                {
                    var random = new Random();
                    var insultIndex = random.Next(Insults.Count);
                    var insult = Insults[insultIndex];

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        insult = $"{target}, {insult.ToLower()}";
                    }

                    await ctx.RespondAsync($"Insulto do dia: {insult}");
                }

                [Command("addinsult")]
                [Description("Adiciona um insulto à lista.")]
                [RequireOwner]
                public async Task AddInsult(CommandContext ctx, [RemainingText] string newInsult)
                {
                    Insults.Add(newInsult);
                    await ctx.RespondAsync($"Insulto adicionado com sucesso: {newInsult}");
                }
                public class RandomFactCommandModule : BaseCommandModule
                {
                    private static List<string> Facts = new List<string>
    {
        "Os elefantes são os únicos animais que não conseguem pular.",
        "A melancia é um vegetal e uma fruta.",
        "O sol é 330.330 vezes mais massivo que a Terra."
    };

                    [Command("randomfact")]
                    [Description("Fornecer um fato aleatório interessante.")]
                    public async Task RandomFact(CommandContext ctx)
                    {
                        var random = new Random();
                        var factIndex = random.Next(Facts.Count);
                        var fact = Facts[factIndex];

                        await ctx.RespondAsync($"Fato aleatório: {fact}");
                    }

                    [Command("addfact")]
                    [Description("Adiciona um novo fato à lista.")]
                    [RequireOwner]
                    public async Task AddFact(CommandContext ctx, [RemainingText] string newFact)
                    {
                        Facts.Add(newFact);
                        await ctx.RespondAsync($"Fato adicionado com sucesso: {newFact}");
                    }
                    public class ComandosEmbed : BaseCommandModule
                    {
                        [Command("habilidade-aprovada")]
                        [Description("Comando para habilidades aprovadas.")]
                        public async Task HabilidadeAprovada(CommandContext ctx)
                        {
                            var embed = new DiscordEmbedBuilder
                            {
                                Title = "Habilidade Aprovada",
                                Description = "Parabéns, sua habilidade foi aprovada!",
                                Color = DiscordColor.Green
                            };

                            await ctx.RespondAsync(embed);
                        }

                        [Command("habilidade-obs")]
                        [Description("Comando para habilidades em observação.")]
                        public async Task HabilidadeObservacao(CommandContext ctx)
                        {
                            var embed = new DiscordEmbedBuilder
                            {
                                Title = "Habilidade em Observação",
                                Description = "Sua habilidade ainda precisa ser testada, por isso foi aprovada em observação! Marque um staff após 7 dias para a observação final.",
                                Color = DiscordColor.Yellow
                            };

                            await ctx.RespondAsync(embed);
                        }

                        [Command("habilidade-rpv")]
                        [Description("Comando para habilidades reprovadas.")]
                        public async Task HabilidadeReprovada(CommandContext ctx)
                        {
                            var embed = new DiscordEmbedBuilder
                            {
                                Title = "Habilidade Reprovada",
                                Description = "Sua habilidade, infelizmente, foi reprovada. Tente mudar algumas coisas e marque um avaliador após isso!",
                                Color = DiscordColor.Red
                            };

                            await ctx.RespondAsync(embed);
                        }
                    }
                }
                public class WeatherCommandModule : BaseCommandModule
                {
                    [Command("weather")]
                    [Description("Fornecer informações climáticas aleatórias.")]
                    public async Task Weather(CommandContext ctx, string city)
                    {
                        var temperature = new Random().Next(-20, 35);
                        var humidity = new Random().Next(0, 100);
                        var isRaining = new Random().Next(0, 2) == 1; // 50% chance de chuva

                        // Determinar o clima com base na temperatura
                        string climate;
                        DiscordColor embedColor;

                        if (temperature <= 20)
                        {
                            climate = "Frio";
                            embedColor = DiscordColor.CornflowerBlue; // Azul Claro
                        }
                        else if (temperature <= 29)
                        {
                            climate = "Nublado";
                            embedColor = DiscordColor.Orange; // Laranja
                        }
                        else
                        {
                            climate = "Calor";
                            embedColor = DiscordColor.Red; // Vermelho
                        }

                        // Adicionar aleatoriedade para chuva
                        if (isRaining)
                        {
                            climate += " com chuva";
                        }

                        var embed = new DiscordEmbedBuilder
                        {
                            Title = $"Condições climáticas em {city}",
                            Description = $"A temperatura é de {temperature}°C.",
                            Color = embedColor
                        };

                        embed.AddField("Clima", climate, true);
                        embed.AddField("Umidade", $"{humidity}%", true);
                        embed.AddField("Chuva", isRaining ? "Sim" : "Não", true);

                        await ctx.RespondAsync(embed);
                    }
                    public class NationModule : BaseCommandModule
                    {
                        private static readonly Dictionary<string, NationInfo> Nations = new Dictionary<string, NationInfo>();

                        [Command("create-nation")]
                        [Description("Cria uma nova nação.")]
                        public async Task CreateNation(CommandContext ctx, string name, string region, DiscordRole nationRole, params DiscordChannel[] channels)
                        {
                            if (Nations.ContainsKey(name.ToLower()))
                            {
                                await ctx.RespondAsync($"A nação '{name}' já existe.");
                                return;
                            }

                            var nationInfo = new NationInfo
                            {
                                Name = name,
                                Region = region,
                                NationRole = nationRole,
                                Channels = channels.ToList()
                            };

                            Nations[name.ToLower()] = nationInfo;

                            var embed = BuildNationEmbed(nationInfo);
                            await ctx.RespondAsync(embed);
                        }

                        [Command("view-nation")]
                        [Description("Visualiza informações sobre uma nação.")]
                        public async Task ViewNation(CommandContext ctx, string name)
                        {
                            if (Nations.TryGetValue(name.ToLower(), out var nationInfo))
                            {
                                var embed = BuildNationEmbed(nationInfo);
                                await ctx.RespondAsync(embed);
                            }
                            else
                            {
                                await ctx.RespondAsync($"A nação '{name}' não foi encontrada.");
                            }
                        }

                        private DiscordEmbedBuilder BuildNationEmbed(NationInfo nationInfo)
                        {
                            var embed = new DiscordEmbedBuilder
                            {
                                Title = $"Informações da Nação: {nationInfo.Name}",
                                Color = DiscordColor.DarkBlue,
                                Description = $"**Região:** {nationInfo.Region}\n" +
                                              $"**Cargo da Nação:** {nationInfo.NationRole.Mention}\n" +
                                              $"**Canais:** {string.Join(", ", nationInfo.Channels.Select(c => c.Mention))}",
                                Timestamp = DateTimeOffset.Now
                            };

                            return embed;
                        }

                        private class NationInfo
                        {
                            public string Name { get; set; }
                            public string Region { get; set; }
                            public DiscordRole NationRole { get; set; }
                            public List<DiscordChannel> Channels { get; set; }
                        }
                        public class FichaModule : BaseCommandModule
                        {
                            private static readonly Dictionary<string, Ficha> Fichas = new Dictionary<string, Ficha>();

                            [Command("create-ficha")]
                            [Description("Cria uma nova ficha de personagem.")]
                            public async Task CreateFicha(CommandContext ctx, string nome, int idade, string genero, string sexualidade, string raca, string classe, string classeSocial, string cidade, string emprego, string elemento, string lado, int altura, int peso, string aparencia)
                            {
                                var ficha = new Ficha(nome, idade, genero, sexualidade, raca, classe, classeSocial, cidade, emprego, elemento, lado, altura, peso, aparencia);
                                Fichas[nome.ToLower()] = ficha;

                                var embed = BuildFichaEmbed(ficha);
                                await ctx.RespondAsync(embed);
                            }

                            [Command("view-ficha")]
                            [Description("Visualiza a ficha de um personagem.")]
                            public async Task ViewFicha(CommandContext ctx, string nome)
                            {
                                if (Fichas.TryGetValue(nome.ToLower(), out var ficha))
                                {
                                    var embed = BuildFichaEmbed(ficha);
                                    await ctx.RespondAsync(embed);
                                }
                                else
                                {
                                    await ctx.RespondAsync($"Ficha para o personagem '{nome}' não encontrada.");
                                }
                            }

                            private DiscordEmbedBuilder BuildFichaEmbed(Ficha ficha)
                            {
                                var embed = new DiscordEmbedBuilder
                                {
                                    Title = $"Ficha de Personagem: {ficha.Nome}",
                                    Color = DiscordColor.DarkGreen,
                                    Description = $"**Nome:** {ficha.Nome}\n" +
                                                  $"**Idade:** {ficha.Idade}\n" +
                                                  $"**Gênero:** {ficha.Genero}\n" +
                                                  $"**Sexualidade:** {ficha.Sexualidade}\n" +
                                                  $"**Raça:** {ficha.Raca}\n" +
                                                  $"**Classe:** {ficha.Classe}\n" +
                                                  $"**Classe Social:** {ficha.ClasseSocial}\n" +
                                                  $"**Cidade:** {ficha.Cidade}\n" +
                                                  $"**Emprego:** {ficha.Emprego}\n" +
                                                  $"**Elemento:** {ficha.Elemento}\n" +
                                                  $"**Lado:** {ficha.Lado}\n" +
                                                  $"**Altura:** {ficha.Altura} cm\n" +
                                                  $"**Peso:** {ficha.Peso} kg\n" +
                                                  $"**Aparência:** {ficha.Aparencia}",
                                    Timestamp = DateTimeOffset.Now
                                };

                                return embed;
                            }
                        }

                        public class Ficha
                        {
                            public string Nome { get; set; }
                            public int Idade { get; set; }
                            public string Genero { get; set; }
                            public string Sexualidade { get; set; }
                            public string Raca { get; set; }
                            public string Classe { get; set; }
                            public string ClasseSocial { get; set; }
                            public string Cidade { get; set; }
                            public string Emprego { get; set; }
                            public string Elemento { get; set; }
                            public string Lado { get; set; }
                            public int Altura { get; set; }
                            public int Peso { get; set; }
                            public string Aparencia { get; set; }

                            public Ficha(string nome, int idade, string genero, string sexualidade, string raca, string classe, string classeSocial, string cidade, string emprego, string elemento, string lado, int altura, int peso, string aparencia)
                            {
                                Nome = nome;
                                Idade = idade;
                                Genero = genero;
                                Sexualidade = sexualidade;
                                Raca = raca;
                                Classe = classe;
                                ClasseSocial = classeSocial;
                                Cidade = cidade;
                                Emprego = emprego;
                                Elemento = elemento;
                                Lado = lado;
                                Altura = altura;
                                Peso = peso;
                                Aparencia = aparencia;
                            }
                        }
                    }
                }
            }
            public class ComandosEmbed : BaseCommandModule
            {
                [Command("pedir-narração")]
                [Description("Comando para pedir narração.")]
                public async Task PedirNarracao(CommandContext ctx, string tipoDeRank, DiscordChannel canalDesejado)
                {
                    // Obtenha o cargo que você deseja marcar
                    var cargoParaMarcar = ctx.Guild.GetRole(1185411390727536680); // Substitua pelo ID do cargo desejado

                    if (cargoParaMarcar == null)
                    {
                        // O cargo não foi encontrado, pode ser necessário ajustar o ID
                        await ctx.RespondAsync("Erro: Cargo não encontrado.");
                        return;
                    }

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Pedido de Narração",
                        Description = $"{ctx.User.Mention} pediu narração no canal {canalDesejado.Mention} com o rank {tipoDeRank} e marcou o cargo {cargoParaMarcar.Mention}.",
                        Color = DiscordColor.Gray // Pode personalizar a cor conforme desejado
                    };

                    // Marque o cargo no canal onde o comando foi chamado
                    await ctx.Channel.SendMessageAsync(embed: embed.Build());
                }
            }
        }
        public class WeatherCommandModule : BaseCommandModule
        {
            [Command("estacao")]
            [Description("Fornecer informações sobre a estação do ano.")]
            public async Task Estacao(CommandContext ctx)
            {
                var seasonArray = new[] { "Primavera", "Verão", "Outono", "Inverno" };
                var randomSeason = seasonArray[new Random().Next(seasonArray.Length)];

                // Mensagem base para cada estação
                string seasonMessage = "";
                switch (randomSeason)
                {
                    case "Primavera":
                        seasonMessage = "As flores estão desabrochando e o clima está ameno.";
                        break;
                    case "Verão":
                        seasonMessage = "É uma temporada quente e ensolarada.";
                        break;
                    case "Outono":
                        seasonMessage = "As folhas estão mudando de cor e o clima está fresco.";
                        break;
                    case "Inverno":
                        seasonMessage = "A neve está caindo e está frio lá fora.";
                        break;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Estação do Ano: {randomSeason}",
                    Description = seasonMessage,
                    Color = DiscordColor.Gold // Cor para representar as estações
                };

                await ctx.RespondAsync(embed);
            }

            [Command("horario")]
            [Description("Fornecer informações sobre o horário atual.")]
            public async Task Horario(CommandContext ctx)
            {
                var timeOfDayArray = new[] { "Manhã", "Tarde", "Noite" };
                var randomTimeOfDay = timeOfDayArray[new Random().Next(timeOfDayArray.Length)];

                // Mensagem base para cada horário
                string timeOfDayMessage = "";
                switch (randomTimeOfDay)
                {
                    case "Manhã":
                        timeOfDayMessage = "O sol está nascendo e um novo dia começa.";
                        break;
                    case "Tarde":
                        timeOfDayMessage = "É a tarde, o sol está alto no céu.";
                        break;
                    case "Noite":
                        timeOfDayMessage = "A noite chegou, as estrelas estão brilhando no céu.";
                        break;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Horário Atual: {randomTimeOfDay}",
                    Description = timeOfDayMessage,
                    Color = GetTimeOfDayEmbedColor(randomTimeOfDay)
                };

                await ctx.RespondAsync(embed);
            }

            // Método auxiliar para obter a cor da embed com base no horário do dia
            private DiscordColor GetTimeOfDayEmbedColor(string timeOfDay)
            {
                switch (timeOfDay)
                {
                    case "Manhã":
                        return DiscordColor.Gold; // Amarelo claro
                    case "Tarde":
                        return DiscordColor.Orange; // Laranja
                    case "Noite":
                        return DiscordColor.DarkBlue; // Azul escuro
                    default:
                        return DiscordColor.Gray; // Padrão para outros casos
                }
            }
        }
    }
}
 









