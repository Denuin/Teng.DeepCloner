using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastDeepCloner
{
    /// <summary>
    /// 属性
    /// </summary>
    public class FastDeepClonerProperty : IFastDeepClonerProperty
    {
        /// <summary>
        /// get 方法
        /// </summary>
        public Func<object?, object?> GetMethod { get; set; }

        /// <summary>
        /// set 方法
        /// </summary>

        public Action<object?, object?> SetMethod { get; set; }

        /// <summary>
        /// 可读属性
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary>
        /// 可写属性
        /// </summary>

        public bool CanWrite { get; private set; }
        /// <summary>
        /// Simple can read.
        /// </summary>

        public bool ReadAble { get; private set; }
        /// <summary>
        /// Ignored
        /// </summary>

        public bool FastDeepClonerIgnore { get => ContainAttribute<IgnoreAttribute>(); }

        /// <summary>
        /// Incase you have circular references in some object
        /// You could mark an identifier or a primary key property so that fastDeepcloner could identify them
        /// </summary>
        public bool FastDeepClonerPrimaryIdentifire { get => ContainAttribute<PrimaryIdentifireAttribute>(); }

        /// <summary>
        /// Apply this to properties that cant be cloned, eg ImageSource and other controls.
        /// Those property will still be copied insted of cloning
        /// </summary>
        public bool NoneCloneable { get => ContainAttribute<NoneCloneableAttribute>(); }

        /// <summary>
        /// 属性名称
        /// </summary>

        public string Name { get; private set; }
        /// <summary>
        /// 属性全称
        /// </summary>

        public string? FullName { get; private set; }

        /// <summary>
        /// 是否为内置类型
        /// </summary>
        public bool IsInternalType { get; private set; }

        private Type _propertyType = null!;

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType
        {
            get => _propertyType;
            set
            {
                if (ConfigrationManager.OnPropertTypeSet != null)
                {
                    _propertyType = ConfigrationManager.OnPropertTypeSet.Invoke(this);
                }
                else _propertyType = value;
            }
        }

        /// <summary>
        /// 是否为虚拟属性
        /// </summary>
        public bool? IsVirtual { get; private set; }

        /// <summary>
        /// 特性集合
        /// </summary>

        public AttributesCollections Attributes { get; set; }

        /// <summary>
        /// 获取值方法
        /// </summary>
        public MethodInfo? PropertyGetValue { get; private set; }

        /// <summary>
        /// 设置属性方法
        /// </summary>

        public MethodInfo? PropertySetValue { get; private set; }

        internal FastDeepClonerProperty(FieldInfo field)
        {
            CanRead = !(field.IsInitOnly || field.FieldType == typeof(IntPtr) || field.IsLiteral);
            CanWrite = CanRead;
            ReadAble = CanRead;
            GetMethod = field.GetValue;
            SetMethod = field.SetValue;
            Name = field.Name;
            FullName = field.FieldType.FullName;
            PropertyType = field.FieldType;
            Attributes = new AttributesCollections(field.GetCustomAttributes().ToList());
            IsInternalType = field.FieldType.IsInternalType();
            ConfigrationManager.OnAttributeCollectionChanged?.Invoke(this);
        }

        internal FastDeepClonerProperty(PropertyInfo property)
        {
            CanRead = !(!property.CanWrite || !property.CanRead || property.PropertyType == typeof(IntPtr) || property.GetIndexParameters().Length > 0);
            CanWrite = property.CanWrite;
            ReadAble = property.CanRead;
            GetMethod = property.GetValue;
            SetMethod = property.SetValue;
            Name = property.Name;
            FullName = property.PropertyType.FullName;
            IsInternalType = property.PropertyType.IsInternalType();
            IsVirtual = property.GetMethod?.IsVirtual;
            PropertyGetValue = property.GetMethod;
            PropertySetValue = property.SetMethod;
            PropertyType = property.PropertyType;
            Attributes = new AttributesCollections(property.GetCustomAttributes().ToList());
            ConfigrationManager.OnAttributeCollectionChanged?.Invoke(this);
        }

        /// <summary>
        /// 获取指定特性集合
        /// </summary>
        public IEnumerable<T>? GetCustomAttributes<T>() where T : Attribute
        {
            return Attributes.Where(x => x is T)?.Select(x => (x as T)!);
        }

        /// <summary>
        /// 获取特性集合
        /// </summary>
        public IEnumerable<Attribute>? GetCustomAttributes(Type type)
        {
            return Attributes.Where(x => x.GetType() == type);
        }

        /// <summary>
        /// 是否存在指定特性
        /// </summary>
        public bool ContainAttribute<T>() where T : Attribute
        {
            return Attributes.ContainedAttributesTypes?.ContainsKey(typeof(T)) ?? false;
        }

        /// <summary>
        /// 获取特性集合
        /// </summary>
        public T? GetCustomAttribute<T>() where T : Attribute
        {
            if (Attributes.ContainedAttributesTypes.TryGetValue(typeof(T), out var attr))
            {
                return (T)attr;
            }
            return default;
        }

        /// <summary>
        /// 获取指定特性
        /// </summary>
        public Attribute? GetCustomAttribute(Type type)
        {
            if (Attributes.ContainedAttributesTypes.TryGetValue(type, out var attr))
            {
                return attr;
            }
            return default;
        }

        /// <summary>
        /// 是否存在特性
        /// </summary>
        public bool ContainAttribute(Type type)
        {
            return Attributes.ContainedAttributesTypes?.ContainsKey(type) ?? false;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(object o, object value)
        {
            SetMethod(o, value);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        public object? GetValue(object? o)
        {
            return GetMethod(o);
        }

        /// <summary>
        /// 添加特性
        /// </summary>
        public void Add(Attribute attr)
        {
            Attributes.Add(attr);
            ConfigrationManager.OnAttributeCollectionChanged?.Invoke(this);
        }
    }
}