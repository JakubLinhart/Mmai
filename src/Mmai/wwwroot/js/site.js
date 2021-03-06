﻿function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function fillLeaderBoard(speciesId, targetElementId) {
    var url = "/api/statistics/top10/" + speciesId;
    var leaderBoardTableElementId = targetElementId + "-table"
    $.getJSON(url, function (leaderboard) {
        $(leaderBoardTableElementId).find("tr:gt(0)").remove();
        $.each(leaderboard.items, function (i, item) {
            $(leaderBoardTableElementId)
                .append("<tr><td>" + item.nickName + "</td><td>" + item.movesCount + "</td></tr>");
        });
        $(targetElementId + "-name").text(leaderboard.name);
        $(targetElementId).removeClass('hidden');
    });
}

function fillAllGameStatistics(targetElementId, speciesId) {
    var url = "/api/statistics/allgames/" + speciesId;
    $.getJSON(url, function (data) {
        $(targetElementId).text(data.meanTurnsCount);
    });
}

var startGame = (function () {
    var gameId = null;
    var currentSpeciesId = null;

    function game(species) {
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

        function postGameFinished() {
            var now = new Date();
            var gameDuration = now - gameStartedTime;

            var data = JSON.stringify({
                id: gameId,
                duration: gameDuration,
                finishedTime: now,
                movesCount: currentMovesCount,
                turnsCount: currentTurnsCount,
                speciesId: currentSpeciesId
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
                speciesId: currentSpeciesId
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
                speciesId: currentSpeciesId,
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

        function finishGame() {
            postGameFinished();
            if (matchCount >= cardCount) {
                for (var i = 0; i < cardCount; i++) {
                    console.log(cards[i]);
                    $(cards[i].cardId).text(cards[i].name);
                }
            }

            loadGameStatistics();
        }

        function loadGameStatistics() {
            $.getJSON("/api/statistics/game/" + currentTurnsCount + "/" + gameId + "/" + currentSpeciesId, function (data) {
                $("#stats-current-turns").text(currentTurnsCount);
                $("#stats-best-turns").text(data.bestTurnsCount);
                $("#stats-better-than-percentage").text(data.betterThanPercentage);

                $("#try-again")
                    .removeClass("hidden");
                $("#finished-game").modal();
            });
        }

        function delayedClearMatchClass(divId) {
            $(divId).removeClass("cardMatch")
                .removeClass("cardMismatch");
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
        var currentMovesCount = 0;
        var currentTurnsCount = 0;

        $('.species-name')
            .text(currentSpeciesName);
        $("#try-again").addClass("hidden");
        $("#nickname")
            .off("change")
            .on("change", function () { updateGameContact(); });

        $('.next-game')
            .off("click")
            .on("click", function () {
                startGame("nextrandom");
            });

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

                    var alreadyUncovered = $(cardId).hasClass("cardUncovered") || $(cardId).hasClass("cardPeeking");

                    $(cardId)
                        .addClass("cardPlaying")
                        .removeClass("cardCovered");

                    if (!alreadyUncovered)
                        $(cardId).addClass("cardPeeking");
                    else
                        $(cardId).addClass("cardUncovered");


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
                            currentMovesCount++;
                            console.log("matchCount " + matchCount + "; cardCount " + cardCount);
                            if (firstSelectedCard != null && firstSelectedCard.cardId != card.cardId) {
                                if (firstSelectedCard.setIndex != card.setIndex) {
                                    $(card.cardId).removeClass("cardUncovered")
                                        .removeClass("cardPeeking")
                                        .addClass("cardCovered")
                                        .addClass("cardMismatch");

                                    setTimeout(delayedClearMatchClass, 1000, card.cardId);

                                    $(firstSelectedCard.cardId)
                                        .removeClass("cardPeeking")
                                        .addClass("cardCovered")
                                        .addClass("cardMismatch")
                                        .removeClass("cardUncovered");
                                    currentTurnsCount++;
                                    setTimeout(delayedClearMatchClass, 1000, firstSelectedCard.cardId);

                                    postGameEvent("mismatch", card.url, card.index);
                                }
                                else {
                                    $(firstSelectedCard.cardId)
                                        .removeClass("cardPeeking")
                                        .addClass("cardMatch")
                                        .addClass("cardUncovered");
                                    $(card.cardId)
                                        .removeClass("cardPeeking")
                                        .addClass("cardMatch")
                                        .addClass("cardUncovered");
                                    matchCount += 2;
                                    currentTurnsCount++;
                                    console.log(matchCount + ", " + cardCount)
                                    if (matchCount >= cardCount) {
                                        postGameEvent("finished", card.url, card.index);
                                        finishGame();
                                    }
                                    else {
                                        setTimeout(delayedClearMatchClass, 1000, card.cardId);
                                        setTimeout(delayedClearMatchClass, 1000, firstSelectedCard.cardId);

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

    return function (id) {
        $("#cards").empty();
        $("#leaderboard").addClass("hidden");
        $("#nickname").empty();
        $("#email").empty();
        currentSpeciesId = id;

        $.getJSON("/api/species/" + id, function (data) {
            currentSpeciesId = data.result.id;
            currentSpeciesName = data.result.name;
            game(data.result);
        });
    }
})();