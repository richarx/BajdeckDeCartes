mergeInto(LibraryManager.library, {
    CopyToClipboardWebGL: function(strPtr) {
        var str = UTF8ToString(strPtr);
        navigator.clipboard.writeText(str).then(
            function() { console.log("Copied to clipboard"); },
            function(err) { console.error("Clipboard error: ", err); }
        );
    }
});
