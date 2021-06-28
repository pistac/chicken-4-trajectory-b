mergeInto(LibraryManager.library, {

  ShowDialog: function(completionCode) {
    window.alert(UTF8ToString(completionCode));
  },
});