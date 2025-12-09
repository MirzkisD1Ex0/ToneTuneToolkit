mergeInto(LibraryManager.library, {
    PuzzleReady: function () {
      puzzle_ready();
    },

    PuzzleDebug: function (value) {
      var message = UTF8ToString(value);
      puzzle_debug(message);
    },

    PuzzleFinished: function () {
      puzzle_finished();
    }
});