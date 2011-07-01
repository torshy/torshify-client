using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Torshify.Client.Infrastructure.Controls
{
    public static class UIHelpers
    {
        #region Methods

        /// <summary>
        /// Finds the logical ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static DependencyObject FindLogicalAncestor(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            DependencyObject o = startElement;
            while ((o != null) && !condition(o))
            {
                o = LogicalTreeHelper.GetParent(o);
            }
            return o;
        }

        /// <summary>
        /// Finds the logical ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns></returns>
        public static T FindLogicalAncestorByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindLogicalAncestor(startElement, o => o is T);
        }

        /// <summary>
        /// Finds the logical root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns></returns>
        public static DependencyObject FindLogicalRoot(this DependencyObject startElement)
        {
            DependencyObject o = null;
            while (startElement != null)
            {
                o = startElement;
                startElement = LogicalTreeHelper.GetParent(startElement);
            }
            return o;
        }

        /// <summary>
        /// Finds the visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static DependencyObject FindVisualAncestor(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            DependencyObject o = startElement;
            while ((o != null) && !condition(o))
            {
                o = VisualTreeHelper.GetParent(o);
            }
            return o;
        }

        /// <summary>
        /// Finds the visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns></returns>
        public static T FindVisualAncestorByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualAncestor(startElement, o => o is T);
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null parent is being returned.</returns>
        public static T FindVisualChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid.
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindVisualChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child.
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Finds the visual descendant.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static DependencyObject FindVisualDescendant(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            if (startElement != null)
            {
                if (condition(startElement))
                {
                    return startElement;
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startElement); ++i)
                {
                    DependencyObject o = FindVisualDescendant(VisualTreeHelper.GetChild(startElement, i), condition);
                    if (o != null)
                    {
                        return o;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the visual descendant by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns></returns>
        public static T FindVisualDescendantByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualDescendant(startElement, o => o is T);
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }

            // use recursion to proceed with next level
            return FindVisualParent<T>(parentObject);
        }

        /// <summary>
        /// Finds the visual root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns></returns>
        public static DependencyObject FindVisualRoot(this DependencyObject startElement)
        {
            return FindVisualAncestor(startElement, o => VisualTreeHelper.GetParent(o) == null);
        }

        /// <summary>
        /// Gets the visual children.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static IEnumerable<Visual> GetVisualChildren(this Visual parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; ++i)
            {
                yield return (Visual)VisualTreeHelper.GetChild(parent, i);
            }
        }

        #endregion Methods
    }
}