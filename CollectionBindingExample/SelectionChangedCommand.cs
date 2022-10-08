using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace TestListBinding
{
    /// <summary>
    /// Handles syncing SelectedItems changes from ListViews.
    /// </summary>
    public class SelectionChangedCommand : ICommand
    {
        /// <summary>
        /// The items list to sync to.
        /// </summary>
        public IList SelectedItems { get; set; }

        /// <summary>
        /// Created a new instance of the SelectionChangedCommand class.
        /// </summary>
        /// <param name="selectedItems">The list of items to sync the changes to.</param>
        public SelectionChangedCommand(IList selectedItems)
        {
            SelectedItems = selectedItems;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return SelectedItems is not null && parameter is SelectionChangedEventArgs;
        }

        public void Execute(object? parameter)
        {
            if (SelectedItems is not null && parameter is SelectionChangedEventArgs e)
            {
                foreach(var item in e.RemovedItems)
                {
                    SelectedItems.Remove(item);
                }
                foreach(var item in e.AddedItems)
                {
                    SelectedItems.Add(item);
                }
            }
        }
    }
}
