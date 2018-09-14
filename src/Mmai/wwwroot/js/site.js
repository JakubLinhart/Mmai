function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

(function () {
    var maxCardCount = 12;

    function startGame(name) {
        for (var i = 0; i < maxCardCount; i++) {
            cardId = "#card" + i;
            $(cardId).removeClass("cardCovered")
                .removeClass("cardUncovered")
                .removeClass("cardPlaying")
                .off("click")
                .empty()
        }

        $("#leaderboard").addClass("hidden");

        $.getJSON("/api/species/" + name, function (data) {
            game(data);
            speciesName = data.name;
        });
    }

    startGame(speciesName);

    function game(species) {
        function postGameFinished() {
            var now = new Date();
            var gameDuration = now - gameStartedTime;

            var data = JSON.stringify({
                id: gameId,
                duration: gameDuration,
                finishedTime: now,
                movesCount: movesCount,
                speciesName: speciesName
            });

            $.ajax({
                url: "/api/games/",
                type: "post",
                data: data,
                dataType: "json",
                accept: 'application/json',
                contentType: "application/json"
            });
        }

        function postGameEvent(label, card) {
            var now = new Date();
            var sinceLastEvent;

            if (lastEventTime != null) {
                sinceLastEvent = now - lastEventTime;
            }
            else
                sinceLastEvent = null;

            var data = JSON.stringify({
                label: label,
                card: card,
                time: now,
                millisecondsSinceLastEvent: sinceLastEvent,
                gameId: gameId,
                speciesName: speciesName
            });
            lastEventTime = now;
            $.ajax({
                url: "/api/events/",
                type: "post",
                data: data,
                dataType: "json",
                accept: 'application/json',
                contentType: "application/json",

                success: function (data) {
                    gameId = data;
                },
                error: function (result) {
                    alert(result);
                }
            });
        }

        function fillLeaderBoard() {
            var url = "/api/leaderboard/" + speciesName;
            $.getJSON(url, function (leaderboard) {
                $("#leaderboard-table").find("tr:gt(0)").remove();
                $.each(leaderboard.items, function (i, item) {
                    $("#leaderboard-table")
                        .append("<tr><td>" + item.name + "</td><td>" + item.movesCount + "</td></tr>");
                });
                $('#leaderboard-name').text(leaderboard.name);
                $('#leaderboard').removeClass('hidden');
            });
        }

        function finishGame() {
            postGameEvent("match", card.url);
            postGameFinished();
            if (matchCount >= cardCount) {
                for (var i = 0; i < cardCount; i++) {
                    console.log(cards[i]);
                    $(cards[i].cardId).text(cards[i].name);
                }
            }

            $('#next-game')
                .removeClass("hidden")
                .text("next game")
                .off("click")
                .on("click", function () {
                    startGame("nextrandom");
                });

            fillLeaderBoard();
        }

        var voiceSets = species.sets;
        var soundPlaying = false;
        var cardCount = species.cardCount;
        var setsCount = voiceSets.length;
        var firstSelectedCard = null;
        var matchCount = 0;
        var lastEventTime = null;
        var gameStartedTime = null;
        var gameId = null;
        var movesCount = 0;

        $('#next-game')
            .off("click")
            .addClass("hidden");

        var cards = [];
        var randomizedSpecies = [];
        for (var i = 0; i < setsCount; i++) {
            randomizedSpecies.push({ index: i, weight: getRandomInt(0, 1000), species: voiceSets[i] });
        }
        randomizedSpecies.sort(function (x, y) {
            return x.weight - y.weight;
        });

        speciesIndex = 0;
        for (var i = 0; i < cardCount; i += 2, speciesIndex++) {
            var randomizedSubSets = [];
            var subSets = randomizedSpecies[speciesIndex].species.subSets;

            for (var j = 0; j < subSets.length; j++) {
                var subSetUrlIndex = getRandomInt(0, subSets[j].length - 1);
                randomizedSubSets.push({ weight: getRandomInt(0, 1000), url: randomizedSpecies[speciesIndex].species.subSets[j][subSetUrlIndex] });
            }

            randomizedSubSets.sort(function (x, y) {
                return x.weight - y.weight;
            });

            cards.push({
                setIndex: randomizedSpecies[speciesIndex].index,
                weight: getRandomInt(0, 1000),
                url: randomizedSubSets[0].url,
                name: randomizedSpecies[speciesIndex].species.name
            });
            cards.push({
                setIndex: randomizedSpecies[speciesIndex].index,
                weight: getRandomInt(0, 1000),
                url: randomizedSubSets[1].url,
                name: randomizedSpecies[speciesIndex].species.name
            });
        }

        cards.sort(function (x, y) {
            return x.weight - y.weight;
        });

        gameStartedTime = new Date();
        postGameEvent("started", null);
        for (var i = 0; i < cardCount; i++) {
            cards[i].cardId = "#card" + i;
            var card = cards[i];
            console.log(card);

            $(card.cardId).addClass("cardCovered")
                .on("click", card, function (e) {
                    if (soundPlaying)
                        return;

                    soundPlaying = true;
                    var cardId = e.data.cardId;
                    var url = e.data.url;
                    var card = e.data;

                    $(cardId)
                        .addClass("cardPlaying")
                        .removeClass("cardCovered")
                        .addClass("cardUncovered");
                    var audio = new Audio(url);
                    audio.onended = function () {
                        $(card.cardId).removeClass("cardPlaying");
                        soundPlaying = false;

                        console.log("Sound finished for card: ");
                        console.log(card);
                        console.log("First selected card: " + firstSelectedCard);
                        console.log(firstSelectedCard);

                        if (matchCount < cardCount) {
                            movesCount++;
                            console.log("matchCount " + matchCount + "; cardCount " + cardCount);
                            if (firstSelectedCard != null && firstSelectedCard.cardId != card.cardId) {
                                if (firstSelectedCard.setIndex != card.setIndex) {
                                    $(card.cardId).removeClass("cardUncovered");
                                    $(card.cardId).addClass("cardCovered");
                                    $(firstSelectedCard.cardId).removeClass("cardUncovered");
                                    $(firstSelectedCard.cardId).addClass("cardCovered");
                                    postGameEvent("mismatch", card.url);
                                }
                                else {
                                    matchCount += 2;
                                    console.log(matchCount + ", " + cardCount)
                                    if (matchCount >= cardCount) {
                                        finishGame();
                                    }
                                }
                                firstSelectedCard = null;
                            }
                            else {
                                postGameEvent("first", card.url);
                                firstSelectedCard = card;
                            }
                        }
                        else {
                            postGameEvent("after the win", card.url);
                        }
                    }
                    audio.play();
                    console.log("Card selected: ");
                    console.log(card)
                });
        }
    }
})();