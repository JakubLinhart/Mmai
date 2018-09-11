using System;
using System.Threading.Tasks;
using Xunit;

namespace Mmai.Data.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var now = DateTime.UtcNow;
            var playerId = Repository.NewGuid();

            //await Repository.AddGameEvent(game.PlayerId, game.Id, "first", "satan klausuv", now);
            //await Repository.AddGameEvent(game.PlayerId, game.Id, "nonmatch", "jiricka smrtonosna", now.AddSeconds(1));
            //await Repository.AddGameEvent(game.PlayerId, game.Id, "first", "konipasek monstrozni", now.AddSeconds(2));
            //await Repository.AddGameEvent(game.PlayerId, game.Id, "match", "cervenka krhava", now.AddSeconds(3));
        }
    }
}
