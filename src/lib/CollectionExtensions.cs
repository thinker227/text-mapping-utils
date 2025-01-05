#if !NETCOREAPP

using System.Reflection;
using System.Reflection.Emit;

namespace TextMappingUtils;

/// <summary>
/// Polyfill for CollectionsMarshal for netstandard2.0.
/// </summary>
internal static class CollectionExtensions<T>
{
    private delegate (T[], int) GetBackingArrayDelegate(List<T> list);

    private static readonly GetBackingArrayDelegate getBackingArray = CreateGetBackingArrayFunc();

    /// <summary>
    /// Get a <see cref="Span{T}"/> view over a <see cref="List{T}"/>'s data.
    /// Items should not be added or removed from the <see cref="List{T}"/> while the <see cref="Span{T}"/> is in use.
    /// </summary>
    /// <param name="list">The list to get the data view over.</param>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    public static Span<T> AsSpan(List<T> list)
    {
        var (items, size) = getBackingArray(list);
        return new(items, 0, size);
    }

    private static GetBackingArrayDelegate CreateGetBackingArrayFunc()
    {
        var dm = new DynamicMethod(nameof(getBackingArray), typeof((T[], int)), [typeof(List<T>)], true);

        var fiItems = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
        var fiSize = typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);

        var ctor = typeof((T[], int)).GetConstructor([typeof(T[]), typeof(int)]);

        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fiItems);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fiSize);
        il.Emit(OpCodes.Newobj, ctor);
        il.Emit(OpCodes.Ret);

        return (GetBackingArrayDelegate)dm.CreateDelegate(typeof(GetBackingArrayDelegate));
    }
}

#endif
