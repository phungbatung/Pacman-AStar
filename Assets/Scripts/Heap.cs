using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IheapItem<T>
{ 
    private int capacity;
    T[] items;
    public int index;

    public Heap(int maxHeapSize) 
    {
        items = new T[maxHeapSize];
    }
    public int CurrentCapacity
    {
        get
        {
            return capacity;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.index], item);
    }

   

    public T RemoveFirst()
    {
        //takes first element
        T first = items[0];
        capacity--;

        //sets last element to first element
        items[0] = items[capacity];
        items[0].index = 0;


        SortDown(items[0]);
        return first;
    }
    void SortDown(T item)
    {
        while (true)
        {
            int left = Left(item);
            int right = Right(item);
            int swapIndex = 0;
            

            //if left index is higher than capacity, it means we
            //are out of bounds
            if (left < capacity)
            {
                swapIndex = left;

                if (right < capacity)
                {
                    //compares two child nodes, if left cost is higher,
                    //sets swapIndex to lower right index
                    if (items[left].CompareTo(items[right]) < 0)
                        swapIndex = right;
                }

                //compares parent node to lowest child node, then does a  
                //swap if the child node is smaller than the parent node
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else 
            {
                return;
            }
        }
    }

    public void Add(T item)
    {
        item.index = capacity;
        items[capacity] = item;
        SortUp(item);
        capacity++;
    }

    void SortUp(T item)
    {
        while (true)
        {
            //finds the object above it in the heap.
            T parentItem = items[Parent(item)];

            //compares two items; if the cost is lower, it moves up the heap 
            if (item.CompareTo(parentItem) > 0)
                Swap(item, parentItem);
            else 
                break;

            /*Parent(item);*/
        }
    }

    /*public void UpdateItem(T item)
    {
        SortUp(item);
    }*/

    //Move element up

    void Swap(T lhs, T rhs)
    {
        items[lhs.index] = rhs;
        items[rhs.index] = lhs;
        int tempIndex = lhs.index;
        lhs.index = rhs.index;
        rhs.index = tempIndex;
    } 
    
    // Get the Parent index for the given index
    public int Parent(T item)
    {
        return (item.index - 1) / 2;
    }

    // Get the Left Child index for the given index
    public int Left(T item)
    {
        return 2 * item.index + 1;
    }

    // Get the Right Child index for the given index
    public int Right(T item)
    {
        return 2 * item.index + 2;
    }
}


public interface IheapItem<T> : IComparable<T>
{ 
    int index { get; set; }
}
