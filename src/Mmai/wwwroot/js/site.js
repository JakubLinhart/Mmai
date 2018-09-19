function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

(function () {
    var gameId = null;

    $("#nickname").on("change", function () { updateGameContact(); });
    $("#email").on("change", function () { updateGameContact(); });

    function updateGameContact() {
        var data = JSON.stringify({
            id: gameId,
            nickName: $("#nickname").val(),
            email: $("#email").val()
        });
        $.ajax({
            url: "/api/games/contact",
            type: "post",
            data: data,
            dataType: "json",
            accept: 'application/json',
            contentType: "application/json"
        });
    }

    function startGame(name) {
        $("#cards").empty();
        $("#leaderboard").addClass("hidden");
        $("#nickname").empty();
        $("#email").empty();

        $.getJSON("/api/species/" + name, function (data) {
            speciesName = data.name;
            game(data);
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
                url: "/api/games/finish",
                type: "post",
                data: data,
                dataType: "json",
                accept: 'application/json',
                contentType: "application/json"
            });
        }

        function postGameStarted() {
            var data = JSON.stringify({
                duration: null,
                finishedTime: null,
                movesCount: null,
                speciesName: speciesName
            });

            $.ajax({
                url: "/api/games/start",
                type: "post",
                data: data,
                dataType: "json",
                accept: 'application/json',
                contentType: "application/json",
                success: function (data) {
                    gameId = data.id;
                },
                error: function (result) {
                    console.log("postGameEvent error: " + result.description);
                }
            });

            postGameEvent("started", null, null);
        }

        function postGameEvent(label, card, cardIndex) {
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
                speciesName: speciesName,
                row: Math.floor(cardIndex / columnCount),
                column: cardIndex % columnCount
            });
            lastEventTime = now;
            $.ajax({
                url: "/api/events/",
                type: "post",
                data: data,
                dataType: "json",
                accept: 'application/json',
                contentType: "application/json",

                error: function (result) {
                    console.log("postGameEvent error: " + result.description);
                }
            });
        }

        function fillLeaderBoard() {
            var url = "/api/leaderboard/top10/" + speciesName;
            $.getJSON(url, function (leaderboard) {
                $("#leaderboard-table").find("tr:gt(0)").remove();
                $.each(leaderboard.items, function (i, item) {
                    $("#leaderboard-table")
                        .append("<tr><td>" + item.nickName + "</td><td>" + item.movesCount + "</td></tr>");
                });
                $('#leaderboard-name').text(leaderboard.name);
                $('#leaderboard').removeClass('hidden');
            });
        }

        function finishGame() {
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
        var columnCount = species.columnCount;
        var setsCount = voiceSets.length;
        var firstSelectedCard = null;
        var matchCount = 0;
        var lastEventTime = null;
        var gameStartedTime = null;
        var movesCount = 0;

        $('#game-description').text(species.description);

        $('#next-game')
            .off("click")
            .addClass("hidden");

        var colIdx = 0;
        var rowIdx = 0;
        for (var i = 0; i < cardCount; i++) {
            if (colIdx == 0) {
                rowId = "row" + rowIdx;

                $("#cards").append("<div class='row' id='" + rowId + "'></div>");
            }

            cardId = "card" + i;
            $('#' + rowId).append("<div class='card cardCovered' id='" + cardId + "'></div>")

            colIdx++;
            if (colIdx == columnCount) {
                rowIdx++;
                colIdx = 0;
            }
        }

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
        postGameStarted();
        for (var i = 0; i < cardCount; i++) {
            cards[i].index = i;
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

                    var alreadyUncovered = $(cardId).hasClass("cardUncovered");

                    $(cardId)
                        .addClass("cardPlaying")
                        .removeClass("cardCovered")
                        .addClass("cardUncovered");
                    var audio = new Audio(url);
                    audio.onended = function () {
                        $(card.cardId).removeClass("cardPlaying");
                        soundPlaying = false;

                        if (alreadyUncovered) {
                            console.log("Sound finished for already uncovered card: ");
                            console.log(card);
                            return;
                        }

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
                                    postGameEvent("mismatch", card.url, card.index);
                                }
                                else {
                                    matchCount += 2;
                                    console.log(matchCount + ", " + cardCount)
                                    if (matchCount >= cardCount) {
                                        postGameEvent("finished", card.url, card.index);
                                        finishGame();
                                    }
                                    else {
                                        postGameEvent("match", card.url, card.index);
                                    }

                                }
                                firstSelectedCard = null;
                            }
                            else {
                                postGameEvent("first", card.url, card.index);
                                firstSelectedCard = card;
                            }
                        }
                        else {
                            postGameEvent("after the win", card.url, card.index);
                        }
                    }
                    audio.play();
                    console.log("Card selected: ");
                    console.log(card)
                });
        }
    }
})();