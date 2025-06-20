﻿using System.Windows;
using System.Windows.Controls;
using Shop.ViewModels;

namespace Shop.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}