namespace FastDeepCloner
{
    /// <summary>
    /// 特性集合
    /// </summary>
    public class AttributesCollections : List<Attribute>
    {
        internal SafeValueType<Attribute, Attribute> ContainedAttributes = new();
        internal SafeValueType<Type, Attribute> ContainedAttributesTypes = new();

        /// <summary>
        /// 构造方法
        /// </summary>
        public AttributesCollections(IEnumerable<Attribute>? attrs)
        {
            if (attrs == null)
                return;

            foreach (Attribute attr in attrs)
                Add(attr);
        }

        /// <summary>
        /// 添加特性
        /// </summary>
        /// <param name="attr"></param>
        public new void Add(Attribute attr)
        {
            ContainedAttributes.TryAdd(attr, attr, true);
            ContainedAttributesTypes.TryAdd(attr.GetType(), attr, true);
            base.Add(attr);
        }

        /// <summary>
        /// 移除特性
        /// </summary>
        /// <param name="attr"></param>
        public new void Remove(Attribute attr)
        {
            base.Remove(attr);
            ContainedAttributes.TryRemove(attr, out _);
            ContainedAttributesTypes.TryRemove(attr.GetType(), out _);
        }
    }
}