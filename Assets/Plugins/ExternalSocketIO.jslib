var plugin = {
  ConnectWebSocket: function(endpoint)
  {
      ConnectWebSocket(Pointer_stringify(endpoint));
  },
  
  EmitMessage: function (title, message) {
    EmitMessage(Pointer_stringify(title), Pointer_stringify(message))
  },
}

mergeInto(LibraryManager.library, plugin);
