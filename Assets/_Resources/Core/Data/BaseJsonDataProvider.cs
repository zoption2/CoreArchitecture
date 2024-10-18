using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Utilities;


namespace Data
{

    public abstract class BaseJsonDataProvider<T> : IDataProvider<T> where T : ICloneable, new()
    {
        private const string kJsonFileExtension = ".json";
        private const string kFullPathFormat = "{0}/{1}{2}";
        private const string kSavesDirectory = "Saves";


        private readonly Queue<object> _savingQueue = new Queue<object>();

        private string _fullFilePath;
        private bool _isSaving;
        private bool _isInited;
        protected T _deserializedData;
        protected virtual JsonSerializerSettings SerializerSettings { get => _baseSerializerSettings; }
        protected virtual JsonSerializerSettings DeserializerSettings { get => _baseDeserializerSettings; }
        protected abstract string JsonFileName { get; }


        protected readonly JsonSerializerSettings _baseSerializerSettings = new JsonSerializerSettings()
        {
#if UNITY_EDITOR
            Formatting = Formatting.Indented,
#endif
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        protected readonly JsonSerializerSettings _baseDeserializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };


        public void Init()
        {
            if (_isInited)
            {
                return;
            }

            if (JsonFileName is null || string.IsNullOrEmpty(JsonFileName))
            {
                Debug.LogError("Set json file name");
                return;
            }

            _fullFilePath = GetFullPathToJsonFile(JsonFileName);
            _deserializedData = LoadAndDeserializeData<T>();
            _isInited = true;
        }

        public virtual async UniTask InitAsync()
        {
            if (_isInited)
            {
                return;
            }

            if (JsonFileName is null || string.IsNullOrEmpty(JsonFileName))
            {
                Debug.LogError("Set json file name");
                return;
            }

            _fullFilePath = GetFullPathToJsonFile(JsonFileName);
            _deserializedData = await LoadAndDeserializeDataAsync<T>();
            _isInited = true;
        }

        public T GetData()
        {
            if (!_isInited)
            {
                Init();
            }

            var copy = (T)_deserializedData.Clone();
            return copy;
        }

        public virtual void Save(T data)
        {
            if (!_isInited)
            {
                Init();
            }

            var clonedData = data.Clone();
            _savingQueue.Enqueue(clonedData);
            _ = SaveQueueData();
        }

        public void Clear()
        {
            if (!_isInited)
            {
                Init();
            }

            DeleteFile();
        }

        public virtual async UniTask ClearAsync()
        {
            if (!_isInited)
            {
                Init();
            }

            DeleteFile();
            await UniTask.CompletedTask;
        }

        protected async UniTaskVoid SaveQueueData()
        {
            if (_isSaving)
            {
                return;
            }

            _isSaving = true;
            while (_savingQueue.Count > 0)
            {
                if (_savingQueue.TryDequeue(out var data))
                {
                    await SerializeAndSaveDataAsync(data);
                }
            }
            _isSaving = false;
        }

        protected string GetFullPathToJsonFile(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath
                , kSavesDirectory
                , fileName
                , kJsonFileExtension);

            return path;
           // return string.Format(kFullPathFormat, fileDirectoryPath, fileName, kJsonFileExtension);
        }

        private TClass LoadAndDeserializeData<TClass>() where TClass : new()
        {
            if (File.Exists(_fullFilePath) == false)
            {
                Debug.Log($"File does not exist at: {_fullFilePath}");
                TClass data = new TClass();
                SerializeAndSaveData(data);
                return data;
            }

            string fileText = File.ReadAllText(_fullFilePath);
            var deserializedData = JSON.FromJSON<TClass>(fileText, DeserializerSettings);

            if (deserializedData is null)
            {
                Debug.Log($"Deserialized object: {typeof(TClass)} is NULL");
                return new TClass();
            }

            return deserializedData;
        }

        private async UniTask<TClass> LoadAndDeserializeDataAsync<TClass>() where TClass : new()
        {
            if (File.Exists(_fullFilePath) == false)
            {
                Debug.Log($"File does not exist at: {_fullFilePath}");
                TClass data = new TClass();
                await SerializeAndSaveDataAsync(data);
                return data;
            }

            string fileText = await File.ReadAllTextAsync(_fullFilePath);
            var deserializedData = await JSON.FromJSONAsync<TClass>(fileText, DeserializerSettings);

            if (deserializedData is null)
            {
                Debug.Log($"Deserialized object: {typeof(TClass)} is NULL");
                return new TClass();
            }

            return deserializedData;
        }

        private void SerializeAndSaveData<TClass>(TClass data)
        {
            string serializedData = JSON.ToJSON(data, SerializerSettings);
            File.WriteAllText(_fullFilePath, serializedData);
        }

        private async UniTask SerializeAndSaveDataAsync<TClass>(TClass data)
        {
            string serializedData = await JSON.ToJSONAsync(data, SerializerSettings);
            await File.WriteAllTextAsync(_fullFilePath, serializedData);
        }

        private void DeleteFile()
        {
            if (File.Exists(_fullFilePath) == false)
            {
                return;
            }
            File.Delete(_fullFilePath);
        }
    }
}
