using TonSdk.Connect;
class InMemoryRemoteStorage : RemoteStorage {
    private InMemoryRemoteStorage(Dictionary<string, string> storage, RemoteStorageGetItem getItem, RemoteStorageSetItem setItem, RemoteStorageRemoveItem removeItem, RemoteStorageHasItem hasItem) : base(getItem, setItem, removeItem, hasItem)
    {
        _storage = storage;
    }

    private readonly Dictionary<string, string> _storage;

    public void ClearStorage(){
        _storage.Clear();
    }

    public static InMemoryRemoteStorage Build(){
         Dictionary<string, string> _storage = [];
         return new InMemoryRemoteStorage(
            _storage, 
            (string key, string defaultValue) => {
                return _storage.TryGetValue(key, out string value) ? value : defaultValue;
            },
            (string key, string value) => {
                _storage[key] = value;
            },
            (string key) => {
                _storage.Remove(key);
            },
            _storage.ContainsKey
         );
    }
}