var plugin = {
  EmitMessage: function (title, message) {
    EmitMessageJS(Pointer_stringify(title), Pointer_stringify(message))
  },

    CopyToClipboard: function (content) {
    CopyToClipboardJS(Pointer_stringify(content))
  },
}

mergeInto(LibraryManager.library, plugin);
