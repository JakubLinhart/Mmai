﻿using Microsoft.AspNetCore.Hosting;
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
                { "littleowl", new SpeciesStub("littleowl", @"\sounds\littleowl\", "Little Owl", 16, 4, CreateSpecies,
                    "In this game, you will hear the territorial calls of Little owl (Athene noctua).") },

                { "10vars", new SpeciesStub("10vars", @"\sounds\10vars\", "Drunken UFO Owl", 16, 4, CreateSpecies,
                    "In this game, you will hear the territorial calls of Drunken UFO Owl.") },

                { "3vars", new SpeciesStub("3vars", @"\sounds\3vars\", "Sleepy UFO Owl", 16, 4, CreateSpecies,
                    "In this game, you will hear the territorial calls of Sleepy UFO Owl.")},

                { "hs33-10vars", new SpeciesStub("hs33-10vars", @"\sounds\hs33_10vars\", "Common UFO Owl", 16, 4, CreateSpecies,
                    "In this game, you will hear the territorial calls of Common UFO Owl.")},

                { "hs33-10vars-long", new SpeciesStub("hs33-10vars-long", @"\sounds\hs33_10vars_long\", "Lazy UFO Owl", 16, 4, CreateSpecies,
                    "In this game, you will hear the territorial calls of Lazy UFO Owl.")}
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