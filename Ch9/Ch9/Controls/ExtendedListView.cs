using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.Controls
{
    // Idea:
    // https://www.pujolsluis.com/creating-custom-controls-part-1-extendedlistview/
    class ExtendedListView : ListView
    {
        // ItemAppearing Property and field to allow binding of a custom command in our control
        // directly from our xaml code
        public static readonly BindableProperty ItemAppearingCommandProperty =
            BindableProperty.Create(nameof(ItemAppearingCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand ItemAppearingCommand
        {
            get { return (ICommand)GetValue(ItemAppearingCommandProperty); }
            set { SetValue(ItemAppearingCommandProperty, value); }
        }

        // LoadMoreCommandProperty Property and field, which will be the one we will call
        // If we need to Load more data as our control needs it.
        public static readonly BindableProperty LoadMoreCommandProperty
            = BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        // TappedCommandProperty Property and field, which will be the one we will call
        // If the user taps an item on our listview.
        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }
        public ExtendedListView() : base()
        {
            ItemAppearing += OnItemAppearing;
            ItemTapped += OnItemTapped;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e) => TappedCommand?.Execute(e.Item);


        // If the new item that is appearing on the screen is the last one in the 
        // collection, we execute the LoadMoreCommand so our ViewModel knows we
        // want to load more data for our user from the data service.                  
        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            ItemAppearingCommand?.Execute(e.Item);

            //If its the last item execute command and load more data.
            if (e.Item == ItemsSource.Cast<object>().Last())
                LoadMoreCommand?.Execute(null);
        }
    }
}
