using System;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ExperimentFramework
{
    public class GameObjectInfo
    {
        public string Guid;
        public GameObject GenerateObject;
    }

    public class AddressableManager
    {
        private static Dictionary<string, GameObjectInfo> Infos = new Dictionary<string, GameObjectInfo>();//生成的物体

        private static Dictionary<string, GameObject> DicSources = new Dictionary<string, GameObject>();

        private static Dictionary<Object, AsyncOperationHandle>
            Handles = new Dictionary<Object, AsyncOperationHandle>();
        
        public static async void LoadSourceGameObject(string assetPath, Action<GameObject> onFinishing = null)
        {
            await LoadSourceGameObjectCoroutine(assetPath, onFinishing);
        }

        public static IEnumerator LoadSourceGameObjectCoroutine(string assetPath, Action<GameObject> onFinishing = null)
        {
            if (DicSources.ContainsKey(assetPath))
            {
                onFinishing?.Invoke(DicSources[assetPath]);
                yield break;
            }

            AsyncOperationHandle<GameObject> handle;

            try
            {
                handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            }
            catch 
            {
                LogManager.Debug($"资源路径{assetPath}错误");
                onFinishing?.Invoke(null);
                yield break;
            }

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (!DicSources.ContainsKey(assetPath))
                    DicSources.Add(assetPath, handle.Result);

                onFinishing?.Invoke(handle.Result);
                
                if (!Handles.ContainsKey(handle.Result))
                    Handles.Add(handle.Result, handle);
            }
            else
            {
                onFinishing?.Invoke(null);
            }
        }

        public static async void LoadSource<T>(string assetPath,Action<T> onFinished)
            where T : Object
        {
            await LoadSourceCoroutine(assetPath, onFinished);
        }

        public static IEnumerator LoadSourceCoroutine<T>(string assetPath, Action<T> onFinished)
            where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetPath);
            yield return handle;

            onFinished?.Invoke(handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null);
            if (!Handles.ContainsKey(handle.Result))
                Handles.Add(handle.Result, handle);
        }


        public static async void LoadSourceList<T>(string label, Action<Dictionary<string,T>> onFinished)
            where T : Object
        {
            await LoadSourceListCoroutine<T>(label, onFinished);
        }

        public static IEnumerator LoadSourceListCoroutine<T>(string label, Action<Dictionary<string,T>> onFinished)
            where T : Object
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(label);
            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                onFinished?.Invoke(new Dictionary<string, T>());

            Dictionary<string, T> dics = new Dictionary<string, T>();
            
            IList<IResourceLocation> locations = handle.Result;

            foreach (var location in locations)
            {
                AsyncOperationHandle<T> handleT =
                    Addressables.LoadAssetAsync<T>(location.PrimaryKey);

                yield return handleT;

                if (handleT.Status == AsyncOperationStatus.Succeeded)
                {
                    dics.Add(location.PrimaryKey, handleT.Result);
                    
                    if (!Handles.ContainsKey(handleT.Result))
                        Handles.Add(handleT.Result, handleT);
                }
                else
                {
                    Addressables.Release(handleT);
                }
            }

            onFinished?.Invoke(dics);

            Addressables.Release(handle);
        }

        public static void CreateGameObject(string assetPath,Action<GameObjectInfo> onFinished)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                onFinished?.Invoke(null);
                return;
            }

            if (DicSources.ContainsKey(assetPath))
            {
                var info = CreateObject(DicSources[assetPath]);
                
                onFinished?.Invoke(info);
            }
            else
            {
                LoadSourceGameObject(assetPath, result =>
                {
                    if (!result)
                    {
                        onFinished?.Invoke(null);
                    }
                    
                    CreateGameObject(assetPath, onFinished);
                });
            }
        }

        static GameObjectInfo CreateObject(GameObject source)
        {
            if (source == null) return null;
            
            var target = GameObject.Instantiate(source);
            var info = new GameObjectInfo()
            {
                Guid = Guid.NewGuid().ToString(),
                GenerateObject = target
            };

            Infos.Add(info.Guid, info);

            return info;
        }

        public static IEnumerator CreateGameObjectCoroutine(string assetPath, Action<GameObjectInfo> onFinished)
        {
            yield return LoadSourceGameObjectCoroutine(assetPath, result => onFinished?.Invoke(CreateObject(result)));
        }

        public static GameObjectInfo CreateGameObject(string guid)
        {
            Infos.TryGetValue(guid, out var info);

            return info;
        }

        public static GameObject GetGameObject(string guid)
        {
            Infos.TryGetValue(guid, out var info);
            return info?.GenerateObject;
        }

        public static void DestoryGameObject(string guid, bool isPool = false)
        {
            Infos.TryGetValue(guid, out var info);
            if (info == null) return;

            if (isPool)
            {
                info.GenerateObject.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.GenerateObject);
                Infos.Remove(guid);
            }
        }

        public static void DestoryGameObject(params string[] guids)
        {
            foreach (var guid in guids)
            {
                DestoryGameObject(guid);
            }
        }

        public static void UnLoadSource(params string[] assetPath)
        {
            foreach (var item in assetPath)
            {
                if (!DicSources.ContainsKey(item)) continue;
                
                var source = DicSources[item];
                AddressableRelease(source);
                DicSources.Remove(item);
            }
        }

        public static void UnLoadSourceAll()
        {
            DicSources.Clear();

            foreach (var handle in Handles)
            {
                Addressables.Release(handle);
            }
            
            Handles.Clear();
        }

        public static void AddressableRelease<T>(params T[] objs)
            where T : Object
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (Handles.ContainsKey(objs[i]))
                {
                    var handle = Handles[objs[i]];

                    Addressables.Release(handle);

                    Handles.Remove(objs[i]);
                }
                
            }
        }
    }
}