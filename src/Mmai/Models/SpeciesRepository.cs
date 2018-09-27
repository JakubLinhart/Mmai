using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class SpeciesRepository : ISpeciesRepository
    {
        private readonly IHostingEnvironment environment;
        private readonly IStringLocalizer<SharedResource> localizer;

        public SpeciesRepository(IHostingEnvironment environment, IStringLocalizer<SharedResource> localizer)
        {
            this.environment = environment;
            this.localizer = localizer;

            speciesStubs = new Dictionary<string, SpeciesStub>(StringComparer.OrdinalIgnoreCase)
            {
                { "littleowl", new SpeciesStub("littleowl", @"\sounds\littleowl\", "Little Owl", 16, 4, CreateSpecies) },

                { "10vars", new SpeciesStub("10vars", @"\sounds\10vars\", "Drunken UFO Owl", 16, 4, CreateSpecies) },

                { "3vars", new SpeciesStub("3vars", @"\sounds\3vars\", "Sleepy UFO Owl", 16, 4, CreateSpecies)},

                { "hs33-10vars", new SpeciesStub("hs33-10vars", @"\sounds\hs33_10vars\", "Common UFO Owl", 16, 4, CreateSpecies)},

                { "hs33-10vars-long", new SpeciesStub("hs33-10vars-long", @"\sounds\hs33_10vars_long\", "Lazy UFO Owl", 16, 4, CreateSpecies)}
            };
        }

        private class SpeciesStub
        {
            public string Path { get; }
            public int CardCount { get; }
            public int ColumnCount { get; }
            public string Name { get; }
            public string Id { get; }
            public Lazy<SpeciesVoiceSet[]> Sets { get; set; }

            public SpeciesStub(string id, string path, string name, int cardCount, int columnCount, Func<SpeciesStub, SpeciesVoiceSet[]> setsFactory)
            {
                Id = id;
                Path = path;
                Name = name;
                CardCount = cardCount;
                ColumnCount = columnCount;
                Sets = new Lazy<SpeciesVoiceSet[]>(() => setsFactory(this), true);
            }
        }

        private readonly Dictionary<string, SpeciesStub> speciesStubs;
        private readonly string[] nextGameSpecies = { "littleowl", "10vars", "3vars", "hs33-10vars", "hs33-10vars-long" };

        private SpeciesVoiceSet[] CreateSpecies(SpeciesStub stub)
        {
            var path = stub.Path;

            var setsDirectories = environment.WebRootFileProvider.GetDirectoryContents(path);
            var setList = new List<SpeciesVoiceSet>();
            foreach (var setDirectory in setsDirectories)
            {
                if (setDirectory.IsDirectory)
                {
                    var voiceSet = new SpeciesVoiceSet();
                    voiceSet.Name = setDirectory.Name;
                    var subSets = new List<string[]>();

                    var setPath = Path.Combine(path, setDirectory.Name);

                    var subSetDirectories = environment.WebRootFileProvider.GetDirectoryContents(setPath);
                    foreach (var subSetDirectory in subSetDirectories)
                    {
                        if (subSetDirectory.IsDirectory)
                        {
                            var subSet = new List<string>();
                            var subSetPath = Path.Combine(setPath, subSetDirectory.Name);
                            var subSetFiles = environment.WebRootFileProvider.GetDirectoryContents(subSetPath);

                            foreach (var file in subSetFiles)
                            {
                                if (!file.IsDirectory)
                                {
                                    var fileName = Path.Combine(subSetPath, file.Name).Replace('\\', '/');
                                    subSet.Add(fileName);
                                }
                            }

                            subSets.Add(subSet.ToArray());
                        }
                        voiceSet.SubSets = subSets.ToArray();
                    }
                    setList.Add(voiceSet);
                }
            }

            return setList.ToArray();
        }

        private Random random = new Random();

        public Task<Species> Get(string name)
        {
            if (name.Equals("nextrandom", StringComparison.OrdinalIgnoreCase))
            {
                var idx = random.Next(0, nextGameSpecies.Length);
                name = nextGameSpecies[idx];
            }

            var stub = speciesStubs[name];

            var species = new Species();
            species.Id = stub.Id;
            species.Name = localizer[stub.Name];
            species.CardCount = stub.CardCount;
            species.ColumnCount = stub.ColumnCount;
            species.Sets = stub.Sets.Value;

            return Task.FromResult(species);
        }
    }
}