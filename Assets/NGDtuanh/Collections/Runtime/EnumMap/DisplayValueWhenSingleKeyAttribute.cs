using System;

namespace NGDtuanh.Collections {
    /// <summary>
    /// Just display single value.<br/>
    /// Use when your enum just have one key, and you don't want to see full <see cref="Collections"/>> display.<br/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayValueWhenSingleKeyAttribute : Attribute { }
}