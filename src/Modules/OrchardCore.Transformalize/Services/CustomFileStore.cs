using OrchardCore.FileStorage.FileSystem;
using TransformalizeModule.Services.Contracts;

namespace TransformalizeModule.Services {

   public class CustomFileStore : FileSystemStore, ICustomFileStore {

      public string Path { get; set; }
      public CustomFileStore(string path) : base(path) {
         Path = path;
      }
     
   }
   
}
