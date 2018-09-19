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
                { "littleowl", new SpeciesStub(@"\sounds\littleowl\", "Little Owl", 2, 5, CreateSpecies,
                    "In the first game, you will hear the territorial calls of Little owl (Athene noctua).") },
                { "10vars", new SpeciesStub(@"\sounds\10vars\", "10vars", 2, 5, CreateSpecies,
                    "In subsequent games, you will hear synthetic acoustic signatures from the two non-existing species which we made up for purposes of this pilot experiment.") },
                { "3vars", new SpeciesStub(@"\sounds\3vars\", "3vars", 2, 5, CreateSpecies,
                    "In subsequent games, you will hear synthetic acoustic signatures from the two non-existing species which we made up for purposes of this pilot experiment. ")}
            };
        }

        private class SpeciesStub
        {
            public string Path { get; }
            public int CardCount { get; }
            public int ColumnCount { get; }
            public string Name { get; }
            public string Description { get; set; }
            public Lazy<Species> Species { get; }

            public SpeciesStub(string path, string name, int cardCount, int columnCount, Func<SpeciesStub, Species> speciesFactory,
                string description)
            {
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
            string path = stub.Path;

            Microsoft.Extensions.FileProviders.IDirectoryContents setsDirectories = environment.WebRootFileProvider.GetDirectoryContents(path);
            List<SpeciesVoiceSet> setList = new List<SpeciesVoiceSet>();
            foreach (Microsoft.Extensions.FileProviders.IFileInfo setDirectory in setsDirectories)
            {
                if (setDirectory.IsDirectory)
                {
                    SpeciesVoiceSet voiceSet = new SpeciesVoiceSet();
                    voiceSet.Name = setDirectory.Name;
                    List<string[]> subSets = new List<string[]>();

                    string setPath = Path.Combine(path, setDirectory.Name);

                    Microsoft.Extensions.FileProviders.IDirectoryContents subSetDirectories = environment.WebRootFileProvider.GetDirectoryContents(setPath);
                    foreach (Microsoft.Extensions.FileProviders.IFileInfo subSetDirectory in subSetDirectories)
                    {
                        if (subSetDirectory.IsDirectory)
                        {
                            List<string> subSet = new List<string>();
                            string subSetPath = Path.Combine(setPath, subSetDirectory.Name);
                            Microsoft.Extensions.FileProviders.IDirectoryContents subSetFiles = environment.WebRootFileProvider.GetDirectoryContents(subSetPath);

                            foreach (Microsoft.Extensions.FileProviders.IFileInfo file in subSetFiles)
                            {
                                if (!file.IsDirectory)
                                {
                                    string fileName = Path.Combine(subSetPath, file.Name).Replace('\\', '/');
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

            Species species = new Species();
            species.Name = stub.Name;
            species.CardCount = stub.CardCount;
            species.ColumnCount = stub.ColumnCount;
            species.Sets = setList.ToArray();
            species.Description = stub.Description;

            return species;
        }

        private Random random = new Random();

        public Species Get(string name)
        {
            if (name.Equals("nextrandom", StringComparison.OrdinalIgnoreCase))
            {
                int idx = random.Next(0, nextGameSpecies.Length);
                name = nextGameSpecies[idx];
            }

            SpeciesStub stub = speciesStubs[name];
            return stub.Species.Value;
        }
    }
}