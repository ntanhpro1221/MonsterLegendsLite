namespace MonsterLegendsLite.Data {
    public readonly struct UserInsDataWithId {
        public readonly string Id;
        public readonly UserInsData Data;

        public UserInsDataWithId(string id, UserInsData data) {
            Id   = id;
            Data = data;
        }
    }
}