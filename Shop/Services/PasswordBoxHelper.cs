using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Shop.Services
{
    public class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
           DependencyProperty.RegisterAttached(
               "BoundPassword",
               typeof(string),
               typeof(PasswordBoxHelper),
               new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj) =>
            (string)obj.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword(DependencyObject obj, string value) =>
            obj.SetValue(BoundPasswordProperty, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
                if (!(bool)passwordBox.GetValue(IsUpdatingProperty))
                {
                    passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.SetValue(IsUpdatingProperty, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                passwordBox.SetValue(IsUpdatingProperty, false);
            }
        }
    }
}
