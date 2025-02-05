﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FastDeepCloner
{
    /// <summary>
    /// DeepCloner
    /// </summary>
    public static class DeepCloner
    {
        /// <summary>
        /// Clear cached data
        /// </summary>
        public static void CleanCachedItems()
        {
            FastDeepClonerCachedItems.CleanCachedItems();
        }

        /// <summary>
        /// DeepClone object
        /// </summary>
        /// <param name="objectToBeCloned">Desire object to cloned</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Clone<T>(this T objectToBeCloned, FastDeepClonerSettings settings) where T : class
        {
            return (T)new ReferenceClone(settings).Clone(objectToBeCloned);
        }

        /// <summary>
        /// DeepClone object
        /// </summary>
        /// <param name="objectToBeCloned">Desire object to cloned</param>
        /// <param name="fieldType">Clone Method</param>
        /// <returns></returns>
        public static object Clone(this object objectToBeCloned, FieldType fieldType = FieldType.PropertyInfo)
        {
            return new ReferenceClone(fieldType).Clone(objectToBeCloned);
        }

        /// <summary>
        /// DeepClone object
        /// </summary>
        /// <param name="objectToBeCloned">Desire object to cloned</param>
        /// <param name="fieldType">Clone Method</param>
        /// <returns></returns>
        public static T Clone<T>(this T objectToBeCloned, FieldType fieldType = FieldType.PropertyInfo) where T : class
        {
            return (T)new ReferenceClone(fieldType).Clone(objectToBeCloned);
        }

        /// <summary>
        /// DeepClone dynamic(AnonymousType) object
        /// </summary>
        /// <param name="objectToBeCloned">Desire AnonymousType object</param>
        /// <returns></returns>
        public static dynamic CloneDynamic(this object objectToBeCloned)
        {
            return new ReferenceClone(FieldType.PropertyInfo).Clone(objectToBeCloned);
        }

        /// <summary>
        /// Create CreateInstance()
        /// this will use ILGenerator to create new object from the cached ILGenerator
        /// This is alot faster then using Activator or GetUninitializedObject.
        /// TThe library will be using ILGenerator or Expression depending on the platform and then cach both the contructorinfo and the type,
        /// so it can be reused later on
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">Optional</param>
        /// <returns></returns>
        public static T CreateInstance<T>(params object[] args) where T : class
        {
            return (T)typeof(T).Creator(true, args);
        }

        /// <summary>
        /// Create CreateInstance()
        /// this will use ILGenerator to create new object from the cached ILGenerator
        /// This is alot faster then using Activator or GetUninitializedObject.
        /// The library will be using ILGenerator or Expression depending on the platform and then cach both the contructorinfo and the type,
        /// so it can be reused later on
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args">Optional</param>
        /// <returns></returns>
        public static object CreateInstance(this Type type, params object[] args)
        {
            return type.Creator(true, args);
        }

        /// <summary>
        /// Get the internal item type of the List or ObservableCollection types
        /// </summary>
        /// <param name="listType"></param>
        /// <returns> will return the same value if the type is not an list type </returns>
        public static Type GetListItemType(this Type listType)
        {
            return listType.GetIListItemType();
        }

        /// <summary>
        /// Convert an object to an interface
        /// The object dose not have to inherit from the interface as the library will handle the job of doing it
        /// </summary>
        /// <param name="interfaceType">interface</param>
        /// <param name="o">the item</param>
        /// <returns></returns>
        public static object ActAsInterface(Type interfaceType, object o)
        {
            return interfaceType.InterFaceConverter(o);
        }

        /// <summary>
        /// Convert an object to an interface
        /// The object dose not have to inherit from the interface as the library will handle the job of doing it
        /// </summary>
        /// <param name="o">the item</param>
        /// <returns></returns>
        public static T ActAsInterface<T>(this object o)
        {
            return (T)typeof(T).InterFaceConverter(o);
        }

        /// <summary>
        /// This will try and load the assembly and cached
        /// then from that assembly it will load typePath and also cach it, so it will load much faster next time
        /// </summary>
        /// <param name="typePath">xxxx</param>
        /// <param name="assembly">xxx.dll</param>
        /// <returns></returns>
        public static Type GetObjectType(this string typePath, string assembly)
        {
            return typePath.GetFastType(assembly);
        }

        /// <summary>
        /// will return fieldInfo from the cached fieldinfo. Get and set value is much faster.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<IFastDeepClonerProperty> GetFastDeepClonerFields(this Type type)
        {
            return FastDeepClonerCachedItems.GetFastDeepClonerFields(type).Values.ToList();
        }

        /// <summary>
        /// will return propertyInfo from the cached propertyInfo. Get and set value is much faster.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<IFastDeepClonerProperty> GetFastDeepClonerProperties(this Type type)
        {
            return FastDeepClonerCachedItems.GetFastDeepClonerProperties(type).Values.ToList();
        }

        /// <summary>
        /// Get field by Name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFastDeepClonerProperty GetField(this Type type, string name)
        {
            return FastDeepClonerCachedItems.GetFastDeepClonerFields(type).ContainsKey(name)
                ? FastDeepClonerCachedItems.GetFastDeepClonerFields(type)[name]
                : null;
        }

        /// <summary>
        /// Get Property by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFastDeepClonerProperty GetProperty(this Type type, string name)
        {
            return FastDeepClonerCachedItems.GetFastDeepClonerProperties(type).ContainsKey(name)
                ? FastDeepClonerCachedItems.GetFastDeepClonerProperties(type)[name]
                : null;
        }

        /// <summary>
        /// This will handle only internal types
        /// and noneinternal type must be of the same type to be cloned
        /// </summary>
        /// <param name="itemToClone"></param>
        /// <param name="CloneToItem"></param>
        public static void CloneTo(this object itemToClone, object CloneToItem)
        {
            new ReferenceClone(FieldType.PropertyInfo).CloneTo(itemToClone, CloneToItem);
        }

        /// <summary>
        /// Convert Value from Type to Type
        /// when fail a default value will be loaded.
        /// can handle all known types like datetime, time span, string, long etc
        /// ex
        ///  "1115rd" to int? will return null
        ///  "152" to int? 152
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ValueConverter<T>(this object value, object defaultValue = null)
        {
            return (T)FastDeepClonerCachedItems.Value(value, typeof(T), true, defaultValue);
        }

        /// <summary>
        /// Convert Value from Type to Type
        /// when fail a default value will be loaded.
        /// can handle all known types like datetime, time span, string, long etc
        /// ex
        ///  "1115rd" to int? will return null
        ///  "152" to int? 152
        /// </summary>
        /// <param name="value"></param>
        /// <param name="datatype">eg typeof(int?)</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ValueConverter(this object value, Type datatype, object defaultValue = null)
        {
            return FastDeepClonerCachedItems.Value(value, datatype, true, defaultValue);
        }

        /// <summary>
        /// Get DefaultValue by type
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ValueByType(this Type propertyType, object defaultValue = null)
        {
            return FastDeepClonerCachedItems.ValueByType(propertyType, defaultValue);
        }

        public static T CreateInstance<T>() where T : class
        {
            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(constructor);
            var lambda = Expression.Lambda<Func<T>>(newExpression);
            var createInstance = lambda.Compile();
            return createInstance();
        }
    }
}