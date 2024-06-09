using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace XactTodo.WinUI.Utils
{
    /// <summary>
    /// 对象克隆工具类
    /// </summary>
    public static class ObjectCloner
    {
        #region 公共方法
        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="model">原对象</param>
        /// <param name="excludeProperties">需要排除的属性(不克隆)</param>
        /// <returns>克隆生成的新对象</returns>
        public static T Clone<T>(this T model, bool deepCopy=true, params string[] excludeProperties) where T:class
        {
            Dictionary<object, object> cloneDictionary = null;
            if (deepCopy)
                cloneDictionary = new Dictionary<object, object>();
            return Clone(model, cloneDictionary, excludeProperties);
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="excludeProperties">需要排除的属性(不复制)</param>
        public static void CopyTo(this object source, object target, params string[] excludeProperties)
        {
            //Dictionary<object, object> cloneDictionary = new Dictionary<object, object>();
            CopyTo(source, target, null, excludeProperties);
        }

        public static T Copy<T>(this object source, params string[] excludeProperties)
        {
            var target = Activator.CreateInstance<T>();
            CopyTo(source, target, excludeProperties);
            return target;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="excludeProperties">需要排除的属性(不复制)</param>
        /// <param name="cloneDictionary">保存克隆对象的字典集合，用于确保同一源对象不会生成多个克隆对象。如果该对象为null，则只对属性值做浅拷贝</param>
        private static void CopyTo(object source, object target, Dictionary<object, object> cloneDictionary, params string[] excludeProperties)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var typeSource = source.GetType();
            var typeTarget = target.GetType();
            var sameType = typeSource == typeTarget;
            PropertyInfo prop2;
            foreach (PropertyInfo prop in typeSource.GetProperties())
            {
                if (!prop.CanRead)
                    continue;

                if (Array.IndexOf<string>(excludeProperties, prop.Name) >= 0)
                {
                    //如果该属性被排除
                    continue;
                }
                var isList = prop.PropertyType.GetInterface(nameof(IList)) != null;
                //如果源类型与目标类型不同则从目标类型获取一下同名属性的类型
                prop2 = sameType ? prop : typeTarget.GetProperty(prop.Name);
                if (prop2 == null || !(prop2.CanWrite || isList))//如果目标类型中不存在该名称的属性或不可写，则跳过
                {
                    continue;
                }

                var propertyName = prop.Name;
                //如果该Property实现了IList接口
                if (prop2.PropertyType.GetInterface(nameof(IList)) != null)
                {
                    IList origCollection = prop.GetValue(source, null) as IList;
                    if (origCollection == null)
                    {
                        if (prop2.CanWrite)
                            prop2.SetValue(target, null, null);

                        continue;
                    }

                    IList cloneCollection = prop.GetValue(target, null) as IList;

                    Type t = origCollection.GetType();

                    if (t.IsArray)
                    {
                        if (prop2.CanWrite)
                        {
                            cloneCollection = (origCollection as Array).Clone() as IList;
                            prop2.SetValue(target, cloneCollection, null);
                        }
                    }
                    else
                    {
                        if (cloneCollection == null)
                        {
                            cloneCollection = Activator.CreateInstance(t) as IList;
                            prop2.SetValue(target, cloneCollection, null);
                        }

                        foreach (object elem in origCollection)
                        {
                            //如果cloneDictionary参数为null，则做浅拷贝，否则做深拷贝
                            if (cloneDictionary == null)
                            {
                                cloneCollection.Add(elem);
                            }
                            else
                            {
                                cloneCollection.Add(Clone(elem, cloneDictionary, excludeProperties));
                            }
                        }
                    }

                    continue;
                }

                object value;
                //如果指定不克隆属性值(直接使用对象赋值)或是属性为值类型则直接赋值，否则克隆该属性值再赋值
                if (cloneDictionary == null || prop.PropertyType.IsValueType)
                {
                    value = prop.GetValue(source, null);
                }
                else
                {
                    value = Clone(prop.GetValue(source, null), cloneDictionary, excludeProperties);
                }
                if (prop.PropertyType != prop2.PropertyType)
                {
                    try
                    {
                        if (prop2.PropertyType.IsEnum)
                            value = Enum.ToObject(prop2.PropertyType, value);
                        else
                            value = Convert.ChangeType(value, prop2.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException(string.Format("类型转换失败，属性名：{0}，值：{1}，目标类型：{2}。", prop.Name, value, prop2.PropertyType.Name) + ex.Message);
                    }
                }
                prop2.SetValue(target, value, null);
            }
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <remarks>保存原对象与克隆对象的字典参数cloneDictionary不能定义为Dictionary<T, T>，因为在深度拷贝模式时不仅要克隆对象本身还需要克隆其属性</remarks>
        private static T Clone<T>(T model, Dictionary<object, object> cloneDictionary, params string[] excludeProperties) where T : class
        {
            if (model == null)
                return default(T);

            if (model.GetType().IsValueType)
                return model;

            if (model is string)
                return model;

            //cloneDictionary不是null，则做深拷贝
            if (cloneDictionary!=null && cloneDictionary.ContainsKey(model))
                return (T)cloneDictionary[model];

            T clone = default(T);

            try
            {
                clone = (T)Activator.CreateInstance(model.GetType());
            }
            catch
            {
            }

            if(cloneDictionary!=null)
                cloneDictionary[model] = clone;

            if (clone == null)
                return default(T);

            CopyTo(model, clone, cloneDictionary, excludeProperties);

            return clone;
        }

        #endregion

    }
}
