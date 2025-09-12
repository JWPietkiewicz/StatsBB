using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace StatsBB.MVVM;

/// <summary>
/// An optimized ObservableCollection that supports bulk operations and performance improvements
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public class OptimizedObservableCollection<T> : ObservableCollection<T>
{
    private bool _suppressNotification = false;

    /// <summary>
    /// Initializes a new instance of OptimizedObservableCollection
    /// </summary>
    public OptimizedObservableCollection() : base() { }

    /// <summary>
    /// Initializes a new instance with the specified items
    /// </summary>
    /// <param name="collection">Items to add to the collection</param>
    public OptimizedObservableCollection(IEnumerable<T> collection) : base(collection) { }

    /// <summary>
    /// Adds multiple items to the collection efficiently
    /// </summary>
    /// <param name="items">Items to add</param>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null) return;

        var itemsList = items.ToList();
        if (itemsList.Count == 0) return;

        _suppressNotification = true;
        try
        {
            foreach (var item in itemsList)
            {
                Add(item);
            }
        }
        finally
        {
            _suppressNotification = false;
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Replaces all items in the collection with new items
    /// </summary>
    /// <param name="items">New items for the collection</param>
    public void ReplaceAll(IEnumerable<T> items)
    {
        if (items == null)
        {
            Clear();
            return;
        }

        var itemsList = items.ToList();
        
        _suppressNotification = true;
        try
        {
            Clear();
            foreach (var item in itemsList)
            {
                Add(item);
            }
        }
        finally
        {
            _suppressNotification = false;
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Removes multiple items from the collection efficiently
    /// </summary>
    /// <param name="items">Items to remove</param>
    public void RemoveRange(IEnumerable<T> items)
    {
        if (items == null) return;

        var itemsList = items.ToList();
        if (itemsList.Count == 0) return;

        _suppressNotification = true;
        try
        {
            foreach (var item in itemsList)
            {
                Remove(item);
            }
        }
        finally
        {
            _suppressNotification = false;
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Temporarily suppresses change notifications for bulk operations
    /// </summary>
    /// <param name="action">Action to perform while notifications are suppressed</param>
    public void SuppressNotifications(Action action)
    {
        if (action == null) return;

        _suppressNotification = true;
        try
        {
            action();
        }
        finally
        {
            _suppressNotification = false;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// Overrides the default notification behavior to support suppression
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressNotification)
        {
            base.OnCollectionChanged(e);
        }
    }

    /// <summary>
    /// Overrides the default property changed notification to support suppression
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (!_suppressNotification)
        {
            base.OnPropertyChanged(e);
        }
    }

    /// <summary>
    /// Sorts the collection in-place using the specified comparer
    /// </summary>
    /// <param name="comparer">Comparer to use for sorting</param>
    public void Sort(IComparer<T> comparer = null)
    {
        var sorted = comparer == null 
            ? this.OrderBy(x => x).ToList() 
            : this.OrderBy(x => x, comparer).ToList();

        ReplaceAll(sorted);
    }

    /// <summary>
    /// Sorts the collection in-place using the specified key selector
    /// </summary>
    /// <typeparam name="TKey">Type of the sort key</typeparam>
    /// <param name="keySelector">Function to select the sort key</param>
    public void SortBy<TKey>(Func<T, TKey> keySelector)
    {
        if (keySelector == null) return;

        var sorted = this.OrderBy(keySelector).ToList();
        ReplaceAll(sorted);
    }

    /// <summary>
    /// Updates an item in the collection and raises appropriate notifications
    /// </summary>
    /// <param name="oldItem">Item to replace</param>
    /// <param name="newItem">New item</param>
    /// <returns>True if the item was found and replaced</returns>
    public bool UpdateItem(T oldItem, T newItem)
    {
        var index = IndexOf(oldItem);
        if (index >= 0)
        {
            SetItem(index, newItem);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Filters the collection and returns a new collection with matching items
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>New collection with filtered items</returns>
    public OptimizedObservableCollection<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null) return new OptimizedObservableCollection<T>(this);

        return new OptimizedObservableCollection<T>(this.Where(predicate));
    }

    /// <summary>
    /// Gets the collection as a read-only collection for safe exposure
    /// </summary>
    /// <returns>Read-only collection</returns>
    public ReadOnlyObservableCollection<T> AsReadOnly()
    {
        return new ReadOnlyObservableCollection<T>(this);
    }
}

/// <summary>
/// Static helper methods for collection optimization
/// </summary>
public static class CollectionOptimizationHelpers
{
    /// <summary>
    /// Creates a new OptimizedObservableCollection from an enumerable
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="source">Source enumerable</param>
    /// <returns>Optimized collection</returns>
    public static OptimizedObservableCollection<T> ToOptimizedObservable<T>(this IEnumerable<T> source)
    {
        return new OptimizedObservableCollection<T>(source);
    }

    /// <summary>
    /// Performs a batch update on a collection with minimal notifications
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="collection">Collection to update</param>
    /// <param name="updates">Updates to perform</param>
    public static void BatchUpdate<T>(this OptimizedObservableCollection<T> collection, Action<OptimizedObservableCollection<T>> updates)
    {
        if (collection == null || updates == null) return;

        collection.SuppressNotifications(() => updates(collection));
    }

    /// <summary>
    /// Adds items to a collection only if they don't already exist
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="collection">Target collection</param>
    /// <param name="items">Items to add</param>
    /// <param name="comparer">Equality comparer (optional)</param>
    public static void AddDistinct<T>(this OptimizedObservableCollection<T> collection, 
        IEnumerable<T> items, IEqualityComparer<T> comparer = null)
    {
        if (collection == null || items == null) return;

        var comp = comparer ?? EqualityComparer<T>.Default;
        var newItems = items.Where(item => !collection.Contains(item, comp)).ToList();
        
        if (newItems.Count > 0)
        {
            collection.AddRange(newItems);
        }
    }

    /// <summary>
    /// Synchronizes a collection with a source enumerable
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="collection">Collection to synchronize</param>
    /// <param name="source">Source enumerable</param>
    /// <param name="comparer">Equality comparer (optional)</param>
    public static void SyncWith<T>(this OptimizedObservableCollection<T> collection,
        IEnumerable<T> source, IEqualityComparer<T> comparer = null)
    {
        if (collection == null || source == null) return;

        var comp = comparer ?? EqualityComparer<T>.Default;
        var sourceList = source.ToList();

        collection.SuppressNotifications(() =>
        {
            // Remove items not in source
            var toRemove = collection.Where(item => !sourceList.Contains(item, comp)).ToList();
            foreach (var item in toRemove)
            {
                collection.Remove(item);
            }

            // Add items not in collection
            var toAdd = sourceList.Where(item => !collection.Contains(item, comp)).ToList();
            foreach (var item in toAdd)
            {
                collection.Add(item);
            }
        });
    }
}