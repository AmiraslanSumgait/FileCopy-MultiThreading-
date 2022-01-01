using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CopyFileWithThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> Text { get; set; } = new List<string>();
        Thread thread;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_filefrom_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files|*.*|Text files|*.txt";
            openFileDialog.FilterIndex = 2;
            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Text.Add(line);
                    }
                    txb_from.Text = openFileDialog.FileName;
                }

            }
        }

        private void btn_to_Click(object seSnder, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Test";
            save.Filter = "Text File | *.txt";
            if (save.ShowDialog() == true)
            {
               txb_to.Text = save.FileName;
            }
        }
        private void CopyProcess()
        {
            string toFile = "";
           Dispatcher.Invoke(new Action(() =>
            {
                toFile = txb_to.Text;
            }));

            if (!File.Exists(toFile))
            {
                using (var stream = File.Create(toFile))
                {
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        foreach (var item in Text)
                        {
                            Thread.Sleep(1000);
                            Dispatcher.Invoke(new Action(() =>
                            {
                                progressbar.Value += 100 / Text.Count;
                            }));

                            sw.WriteLine(item);
                        }
                    }
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    progressbar.Value = 100;
                }));
                MessageBox.Show("Copy Success!");
            }

        }
        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart threadStart = new ThreadStart(CopyProcess);
            thread = new Thread(threadStart);
            thread.Start();
        }

        private void btn_resume_Click(object sender, RoutedEventArgs e)
        {
            thread.Resume();
        }

        private void btn_suspend_Click(object sender, RoutedEventArgs e)
        {
            thread.Suspend();
        }
    }
}
