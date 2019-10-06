using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _8PuzzelGame
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        public int RowsOption { get; set; } = 0;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string text = RowsOptionTextBlock.Text;
            int value = 0;
            bool isSuccess = true;

            try
            {
                value = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                MessageBox.Show(ex.Message);
            }

            if (isSuccess)
            {
                RowsOption = value;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
