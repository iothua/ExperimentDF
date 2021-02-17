using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using System;

namespace ExperimentFramework
{
    public static class ExperimentUtilitys
    {
        /// <summary>
        /// 校验输入的内容是否为邮箱
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string inputData)
        {
            Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }
        
        public static Texture2D GetTexture2DFromPath(string imgPath,int width,int height)
        {
            if (!File.Exists(imgPath)) return null;

            using (FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
            {
                int byteLength = (int)fs.Length;
                byte[] imgBytes = new byte[byteLength];
                fs.Read(imgBytes, 0, byteLength);
                fs.Close();
                fs.Dispose();

                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(imgBytes);
                texture.Apply();

                return texture;
            }
        }

        public static Texture2D GetTexture2DFromPath(byte[] imgBytes, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(imgBytes);
            texture.Apply();

            return texture;
        }

        public static Sprite GetSpriteFromPath(string imgPath, int width, int height)
        {
            return GetSpriteFromTexture(GetTexture2DFromPath(imgPath, width, height));
        }

        public static Sprite GetSpriteFromTexture(Texture2D texture)
        {
            if (texture == null) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            return sprite;
        }
        
        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainWorldToScreenPoint(Vector3 point)
        {
            if (Camera.main == null)
                throw new Exception("mainCamera相机对象为Null");

            return Camera.main.WorldToScreenPoint(point);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标(主相机为准)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 MainScreenToWorldPoint(Vector3 point)
        {
            if (Camera.main == null)
                throw new Exception("mainCamera相机对象为Null");

            return Camera.main.ScreenToWorldPoint(point);
        }
        
        /// <summary>
        /// 计算适口偏移值
        /// </summary>
        /// <returns>The offset position.</returns>
        /// <param name="handPosition">Hand position.</param>
        /// <param name="grabObject">Grab object.</param>
        public static Vector3 GetOffsetPosition(Vector3 handPosition,GameObject grabObject)
        {

            var offset = Vector3.zero;
            Vector3 screenDevice = MainWorldToScreenPoint(grabObject.transform.position);
            Vector3 vPos = MainScreenToWorldPoint(new Vector3(handPosition.x,handPosition.y,screenDevice.z));

            offset = vPos - grabObject.transform.position;

            return offset;
        }
        
        /// <summary>
        /// 控制旋转角度范围：min max
        /// </summary>
        /// <param name="angle">当前角度</param>
        /// <param name="min">最小角度</param>
        /// <param name="max">最大角度</param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            // 控制旋转角度不超过360
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
        
        /// <summary>
        /// 将秒转化为时间
        /// </summary>
        /// <param name="second">秒</param>
        /// <param name="type">00:00  00分00秒</param>
        /// <returns></returns>
        public static string ToTimeFormat(int second,string type)
        {
            string timeStr = string.Empty;

            var span = new TimeSpan(0, 0, second);

            string hours = span.Hours.ToString().PadLeft(2, '0');
            string minutes = span.Minutes.ToString().PadLeft(2, '0');
            string seconds = span.Seconds.ToString().PadLeft(2, '0');
            
            switch (type)
            {
                //00:00
                case "00:00":
                    timeStr = string.Format("{0}:{1}", minutes, seconds);
                    break;
                //00分00秒
                case "00分00秒":
                    timeStr = string.Format("{0}分{1}秒", minutes,seconds);
                    break;
                case "00:00:00":
                    timeStr = string.Format("{0}:{1}:{2}", hours, minutes, seconds);
                    break;
            }

            return timeStr;
        }
        
        /// <summary>
        /// 获取到碰撞体八个点的坐标
        /// </summary>
        /// <param name="boxcollider"></param>
        /// <returns></returns>
        public static Vector3[] GetBoxColliderVertexPositions (BoxCollider boxcollider) 
        {
            var vertices = new Vector3[8];
            //下面4个点
            vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
            vertices[1] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
            vertices[2] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
            vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
            //上面4个点
            vertices[4] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
            vertices[5] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
            vertices[6] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
            vertices[7] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
 
            return vertices;
        }

        public static Vector3[] GetBoxColliderCenter(BoxCollider boxcollider)
        {
            var vertices = new Vector3[6];
            //上下
            vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(0, boxcollider.size.y, 0) * 0.5f);
            vertices[1] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(0, -boxcollider.size.y, 0) * 0.5f);
            
            //左右
            vertices[2] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(-boxcollider.size.x, 0, 0) * 0.5f);
            vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, 0,0) * 0.5f);
            
            //前后
            vertices[4] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(0, 0, -boxcollider.size.z) * 0.5f);
            vertices[5] = boxcollider.transform.TransformPoint( boxcollider.center + new Vector3(0, 0, boxcollider.size.z) * 0.5f);
            
            return vertices;
        }
        
        /// <summary>
        /// 角度转换
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float CorrectAngle(float angle)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;

            return angle;
        }
        
        /// <summary>
        /// 根据名称添加脚本
        /// </summary>
        /// <returns>The equipment by name.</returns>
        /// <param name="t">T.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Component AddEquipmentByName<T>(this T t,string name) where T : Component
        {
            Type type = null;
            type = Type.GetType(name);

            return t.gameObject.GetComponent(type) ?? t.gameObject.AddComponent(type);
        }

        /// <summary>
        /// 设置局部Tranform值
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="transformData">Transform data.</param>
        public static void SetTransform(this Transform transform,TransformData transformData,bool isLocal = true)
        {
            if (isLocal)
            {
                transform.localPosition = transformData.Position.UnityVector();
                transform.localRotation = Quaternion.Euler(transformData.Rotate.UnityVector());
                transform.localScale = transformData.Scale.UnityVector();
            }
            else
            {

                transform.SetParent(null);

                transform.position = transformData.Position.UnityVector();
                transform.rotation = Quaternion.Euler(transformData.Rotate.UnityVector());
                transform.localScale = transformData.Scale.UnityVector();
            }

        }
        
        /// <summary>
        /// 获得网格最小点，默认支持蒙皮网格
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 BoundsMin(this GameObject target,bool inChildren = true,bool skinnedMesh = true)
        {
            Renderer[] renderers;
            Vector3 sizeMin = Vector3.zero;
            if (target != null)
            {
                if (CheckTheConditions(target,out renderers,inChildren,skinnedMesh))
                {
                    Bounds[] bounds = RenderersToBounds(renderers);
                    sizeMin = bounds[0].min;
                    foreach (Bounds bound in bounds)
                    {
                        Vector3 boundsMin = bound.min;
                        sizeMin.x = Mathf.Min(sizeMin.x,boundsMin.x);
                        sizeMin.y = Mathf.Min(sizeMin.y,boundsMin.y);
                        sizeMin.z = Mathf.Min(sizeMin.z,boundsMin.z);
                    }
                }
            }
            return sizeMin;
        }

        /// <summary>
        /// 获得网格最大点，默认支持蒙皮网格
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 BoundsMax(this GameObject target,bool inChildren = true,bool skinnedMesh = true)
        {
            Renderer[] renderers;
            Vector3 sizeMax = Vector3.zero;
            if (CheckTheConditions(target,out renderers,inChildren,skinnedMesh))
            {
                Bounds[] bounds = RenderersToBounds(renderers);
                sizeMax = bounds[0].max;
                foreach (Bounds bound in bounds)
                {
                    Vector3 boundsMax = bound.max;
                    sizeMax.x = Mathf.Max(sizeMax.x,boundsMax.x);
                    sizeMax.y = Mathf.Max(sizeMax.y,boundsMax.y);
                    sizeMax.z = Mathf.Max(sizeMax.z,boundsMax.z);
                }
            }
            return sizeMax;
        }

        /// <summary>
        /// 检查自己及子物体是否包含Renderers相关组件
        /// </summary>
        private static bool CheckTheConditions(GameObject obj,out Renderer[] renderers,bool inChildren = true,bool skinnedMesh = true)
        {
            List<Renderer> tmpRenderers = new List<Renderer>();

            //网格渲染器与蒙皮网格渲染器的获取
            MeshRenderer[] meshRenderers = null;
            SkinnedMeshRenderer[] skinnedMeshRenderers = null;
            if (obj != null)
            {
                if (inChildren)
                {
                    meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                    if (skinnedMesh)
                        skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                }
                else
                {
                    meshRenderers = obj.GetComponentInChildren<MeshRenderer>() == null ? null : new MeshRenderer[] { obj.GetComponentInChildren<MeshRenderer>() };
                    if (skinnedMesh)
                        skinnedMeshRenderers = obj.GetComponentInChildren<SkinnedMeshRenderer>() == null ? null : new SkinnedMeshRenderer[] { obj.GetComponentInChildren<SkinnedMeshRenderer>() };
                }

                //集合不同网格的Renderers
                if (meshRenderers != null)
                {
                    foreach (var meshRenderer in meshRenderers)
                    {
                        //检测 获取的Bounds是否符合条件
                        if (meshRenderer.bounds.size != Vector3.zero)
                            tmpRenderers.Add(meshRenderer);
                    }
                }

                if (skinnedMeshRenderers != null)
                {
                    foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
                    {
                        if (skinnedMeshRenderer.bounds.size != Vector3.zero)
                            tmpRenderers.Add(skinnedMeshRenderer);
                    }
                }
            }

            //赋值OUT关键字参数
            renderers = tmpRenderers.ToArray();
            if (renderers.Length == 0)
            {
                if (obj != null)
                {
                    Debug.LogError(obj.name + "及子物体没有Renderer相关组件");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算物体包围盒list
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<UnityEngine.Object> CalculateBounds(Transform go)
        {
            //Bounds b = new Bounds(go.position, Vector3.zero);
            List<UnityEngine.Object> rList = new List<UnityEngine.Object>();

            for (int i = 0; i < go.GetComponentsInChildren(typeof(MeshRenderer)).Length; i++)
            {
                rList.Add(go.GetComponentsInChildren(typeof(MeshRenderer))[i]);
            }
            for (int i = 0; i < go.GetComponentsInChildren(typeof(SkinnedMeshRenderer)).Length; i++)
            {
                rList.Add(go.GetComponentsInChildren(typeof(SkinnedMeshRenderer))[i]);
            }
            //foreach (Renderer r in rList)
            //{
            //    b.Encapsulate(r.bounds);
            //}
            return rList;
        }

        /// <summary>
        /// 计算物体包围盒
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Bounds ObjectBounds(Transform go)
        {
            Bounds b = new Bounds(go.position, Vector3.zero);
            List<UnityEngine.Object> rList = new List<UnityEngine.Object>();

            for (int i = 0; i < go.GetComponentsInChildren(typeof(MeshRenderer)).Length; i++)
            {
                rList.Add(go.GetComponentsInChildren(typeof(MeshRenderer))[i]);
            }
            for (int i = 0; i < go.GetComponentsInChildren(typeof(SkinnedMeshRenderer)).Length; i++)
            {
                rList.Add(go.GetComponentsInChildren(typeof(SkinnedMeshRenderer))[i]);
            }
            foreach (Renderer r in rList)
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }

        /// <summary>
        /// 将Renderer转换为Bound
        /// </summary>
        /// <param name="renderers"></param>
        /// <returns></returns>
        private static Bounds[] RenderersToBounds(Renderer[] renderers)
        {
            List<Bounds> bounds = new List<Bounds>();
            foreach (var item in renderers)
            {
                bounds.Add(item.bounds);
            }
            return bounds.ToArray();
        }

        /// <summary>
        /// 根据类名，创建对象
        /// </summary>
        /// <param name="className"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T CreateType<T>(string className)
            where T : class
        {
            Type type = Type.GetType(className);

            if (type == null) throw new Exception("className名称不对，找不到此脚本");

            var t = Activator.CreateInstance(type) as T;

            return t;
        }

    }
}

