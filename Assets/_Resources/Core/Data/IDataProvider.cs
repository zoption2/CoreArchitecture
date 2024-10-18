namespace Data
{
    public interface IDataProvider<TData>
    {
        void Init();
        TData GetData();
        void Save(TData data);
        void Clear();

        Cysharp.Threading.Tasks.UniTask InitAsync();
        Cysharp.Threading.Tasks.UniTask ClearAsync();
    }
}

