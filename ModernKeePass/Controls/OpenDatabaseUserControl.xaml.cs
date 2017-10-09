﻿using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page http://go.microsoft.com/fwlink/?LinkId=234236

namespace ModernKeePass.Controls
{
    public sealed partial class OpenDatabaseUserControl : UserControl
    {
        public OpenDatabaseUserControl()
        {
            InitializeComponent();
        }

        public event PasswordCheckedEventHandler ValidationChecked;
        public delegate void PasswordCheckedEventHandler(object sender, EventArgs e);

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            StatusTextBlock.Text = app.Database.Open(PasswordBox.Password);
            ValidationChecked?.Invoke(this, new EventArgs());
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) OpenButton_OnClick(null, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(
                () => Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                        () => PasswordBox.Focus(FocusState.Programmatic)));
        }
    }
}
