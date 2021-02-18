using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Config
{
    public static class EquipmentOperateErrorConfig
    {
       public static async void GetOperateError(string errorPath,Action<List<EquipmentErrorConfig>> OnComplete)
        {
            var asset = Addressables.LoadAssetAsync<TextAsset>(errorPath);
            await asset.Task;

            if(asset.Status==AsyncOperationStatus.Succeeded)
            {
                List<EquipmentErrorConfig> _config = JsonHelper.JsonToObject<List<EquipmentErrorConfig>>(asset.Result.text);
                OnComplete?.Invoke(_config);
            }
            else
            {
                throw asset.OperationException;
            }
            Addressables.Release(asset);
        }
    }


    [System.Serializable]
    public struct EquipmentErrorConfig
    {
        public string equipmentKey;
        public List<ErrorDatas> Errors;
    }

    [System.Serializable]
    public struct ErrorDatas
    {
        public string error; 
    }
}

