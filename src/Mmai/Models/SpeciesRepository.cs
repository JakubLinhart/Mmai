using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Mmai.Models
{
    public class SpeciesRepository : ISpeciesRepository
    {
        private readonly IHostingEnvironment environment;

        public SpeciesRepository(IHostingEnvironment environment)
        {
            this.environment = environment;
            speciesStubs = new Dictionary<string, SpeciesStub>(StringComparer.OrdinalIgnoreCase)
            {
                { "littleowl", new SpeciesStub("littleowl", @"\sounds\littleowl\", "Little Owl", 20, 5, CreateSpecies,
                    "In this game, you will hear the territorial calls of Little owl (Athene noctua).") },
                { "10vars", new SpeciesStub("10vars", @"\sounds\10vars\", "10vars", 20, 5, CreateSpecies,
                    "Now, you will hear synthetic acoustic signatures from the two non-existing species which we made up for purposes of this pilot experiment.") },
                { "3vars", new SpeciesStub("3vars", @"\sounds\3vars\", "3vars", 20, 5, CreateSpecies,
                    "Now, you will hear synthetic acoustic signatures from the two non-existing species which we made up for purposes of this pilot experiment. ")}
            };
        }

        private class SpeciesStub
        {
            public string Path { get; }
            public int CardCount { get; }
            public int ColumnCount { get; }
            public string Name { get; }
            public string Id { get; }
            public string Description { get; set; }
            public Lazy<Species> Species { get; }

            public SpeciesStub(string id, string path, string name, int cardCount, int columnCount, Func<SpeciesStub, Species> speciesFactory,
                string description)
            {
                Id = id;
                Path = path;
                Name = name;
                CardCount = cardCount;
                ColumnCount = columnCount;
                Species = new Lazy<Species>(() => speciesFactory(this), true);
                Description = description;
            }
        }

        private readonly Dictionary<string, SpeciesStub> speciesStubs;
        private readonly string[] nextGameSpecies = { "10vars", "3vars" };

        private Species CreateSpecies(SpeciesStub stub)
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

            var species = new Species();
            species.Id = stub.Id;
            species.Name = stub.Name;
            species.CardCount = stub.CardCount;
            species.ColumnCount = stub.ColumnCount;
            species.Sets = setList.ToArray();
            species.Description = stub.Description;

            return species;
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
            return Task.FromResult(stub.Species.Value);
        }
    }
}