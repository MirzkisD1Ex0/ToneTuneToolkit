调用
[DllImport("__Internal")] private static extern void puzzle_ready();  
[DllImport("__Internal")] private static extern void puzzle_debug(string value);
[DllImport("__Internal")] private static extern void puzzle_finished();

被调用
public void StartGame(string message)

空实现
Plugins/xxx.jslib
// Assets/Plugins/WebGL/JigsawPuzzleLib.jslib
mergeInto(LibraryManager.library, {
    puzzle_ready: function () {
        // 空实现
    },

    puzzle_debug: function (value_ptr) {
        // 空实现
    },

    puzzle_finished: function () {
        // 空实现
    }
});


