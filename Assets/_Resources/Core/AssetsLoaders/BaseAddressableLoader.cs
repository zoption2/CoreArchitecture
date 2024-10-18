using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace AssetsLoading
{

    /// Wrapper for GameObject addressable loading
    [System.Serializable]
    public sealed class AddressableGameobjectsLoader
        : BaseAddressableLoader<GameObject, AssetReferenceGameObject>
    {

    }


    /// Wrapper for Sprite addressable loading
    public sealed class AddressableSpriteLoader
        : BaseAddressableLoader<Sprite, AssetReferenceSprite>
    {

    }


    /// Wrapper for Scriptable Object addressable loading
    public sealed class AddressableScriptableObjectLoader
        : BaseAddressableLoader<ScriptableObject, AssetReference>
    {

    }


    /// <summary>
    /// Base class-mapper for all kinds of assets, that using for Load and Release addressable asset
    /// </summary>
    /// <typeparam name="TObject">Generic for asset type</typeparam>
    /// <typeparam name="TRef">Generic for Asset Reference type</typeparam>
    [System.Serializable]
    public abstract class BaseAddressableLoader<TObject, TRef>
        where TObject : class
        where TRef : AssetReference
    {
        [SerializeField] protected TRef _reference;
        protected AsyncOperationHandle<TObject> _operationHandler;


        /// <summary>
        /// Load asset async. Method using cache to store Loading result until Clearing.
        /// </summary>
        /// <returns>Asset of type, WITHOUT instantiating</returns>
        public async UniTask<TObject> LoadAsync()
        {
            TObject loaded = null;

            if (_operationHandler.IsValid()
                && _operationHandler.Status == AsyncOperationStatus.Succeeded)
            {
                loaded = _operationHandler.Result;
            }
            else
            {
                _operationHandler = Addressables.LoadAssetAsync<TObject>(_reference.RuntimeKey);
                await _operationHandler;
                int attempts = 0;

                while (_operationHandler.Status != AsyncOperationStatus.Succeeded
                    && attempts < 5)
                {
                    _operationHandler = Addressables.LoadAssetAsync<TObject>(_reference.RuntimeKey);
                    await _operationHandler;
                    attempts++;
                    Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, _reference.AssetGUID);
                }           
            }

            return loaded;
        }


        /// Release cached async operation 
        public void Clear()
        {
            if (_operationHandler.IsValid()
                && _operationHandler.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(_operationHandler);
                _operationHandler = default;
            }
        }
    }
}

