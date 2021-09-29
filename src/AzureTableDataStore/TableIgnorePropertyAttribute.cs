using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableDataStore
{
   
    /// <summary>
    /// An attribute that marks a property to be ignored in Table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TableIgnorePropertyAttribute : Attribute
    {
    }
}
