using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

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
                { "sycek", new SpeciesStub(@"\sounds\sycek\", "sycek", 10, 3, CreateSpecies) },
                { "10vars", new SpeciesStub(@"\sounds\10vars\", "10vars", 20, 5, CreateSpecies) },
                { "3vars", new SpeciesStub(@"\sounds\3vars\", "3vars", 20, 5, CreateSpecies)}
            };
        }

        private class SpeciesStub
        {
            public string Path { get; }
            public int CardCount { get; }
            public int ColumnCount { get; }
            public string Name { get; }
            public Lazy<Species> Species { get; }

            public SpeciesStub(string path, string name, int cardCount, int columnCount, Func<SpeciesStub, Species> speciesFactory)
            {
                Path = path;
                Name = name;
                CardCount = cardCount;
                ColumnCount = columnCount;
                Species = new Lazy<Species>(() => speciesFactory(this), true);
            }
        }

        private readonly Dictionary<string, SpeciesStub> speciesStubs;
        private readonly string[] nextGameSpecies = { "10vars", "3vars" };

        private Species CreateSpecies(SpeciesStub stub)
        {
            string path = stub.Path;

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
            species.Name = stub.Name;
            species.CardCount = stub.CardCount;
            species.ColumnCount = stub.ColumnCount;
            species.Sets = setList.ToArray();

            return species;
        }

        private Random random = new Random();

        public Species Get(string name)
        {
            if (name.Equals("nextrandom", StringComparison.OrdinalIgnoreCase))
            {
                var idx = random.Next(0, nextGameSpecies.Length);
                name = nextGameSpecies[idx];
            }

            var stub = speciesStubs[name];
            return stub.Species.Value;
        }
    }
}