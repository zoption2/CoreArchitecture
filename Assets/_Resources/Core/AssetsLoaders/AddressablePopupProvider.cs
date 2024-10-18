using UnityEngine;
using Cysharp.Threading.Tasks;


namespace AssetsLoading
{
    /// <summary>
    /// Interface for addressable asset lifecycle.
    /// </summary>
    public interface IAddressablePopupProvider
    {
        /// <summary>
        /// Instantiate gameobject from addressable asset and return specific type
        /// </summary>
        UniTask<TPopup> InstantiatePopupAsync<TPopup>(Transform parent);

        /// <summary>
        /// Clears the cache of addressable asset references.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Base class for providing access to addressable assets.
    /// Inherit from this class to create provider for specific gameObject asset.
    /// </summary>
    public abstract class AddressablePopupProvider : ScriptableObject, IAddressablePopupProvider
    {
        [SerializeField] private AddressableGameobjectsLoader _loader;

        /// Instantiate Popup instance (GameObject) at Parent
        public async UniTask<TPopup> InstantiatePopupAsync<TPopup>(Transform parent)
        {
            GameObject prefab = await _loader.LoadAsync();
            GameObject instance = Instantiate(prefab, parent);
            TPopup result = instance.GetComponent<TPopup>();
            return result;
        }

        /// <summary>
        /// Clears the cache of addressable asset references.
        /// </summary>
        public virtual void Clear()
        {
            _loader.Clear();
        }
    }
}
