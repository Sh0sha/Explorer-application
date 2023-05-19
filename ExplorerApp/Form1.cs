using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ExplorerApp
{
    public partial class Form1 : Form
    {
        private string currentDirectory;    // Текущая директория
     
        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            textBox1.KeyDown += textBox1_KeyDown;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            button1.Click += button1_Click;
            button4.Click += button4_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);     // Установка начальной директории на Рабочий стол
            UpdateDirectory(currentDirectory);                                                  // Обновление содержимого текущей директории
        }

    private void UpdateDirectory(string path)             // Директории
        {
           
            try
            {
                currentDirectory = path;

                // Обновляем строку директории
                textBox1.Text = currentDirectory;


                // Clear the list view
                listView1.Items.Clear();

                // Add directories to the list view
                string[] directories = Directory.GetDirectories(currentDirectory);
                foreach (string directory in directories)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    ListViewItem item = new ListViewItem(dirInfo.Name);
                    item.SubItems.Add(dirInfo.CreationTime.ToString());
                    item.SubItems.Add(dirInfo.LastWriteTime.ToString());
                    item.SubItems.Add("Путь");
                    listView1.Items.Add(item);
                }

                // Add files to the list view
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    ListViewItem item = new ListViewItem(fileInfo.Name);
                    item.SubItems.Add(fileInfo.CreationTime.ToString());
                    item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                    item.SubItems.Add(fileInfo.Length.ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Доступ к пути отклонён");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }


        private void textBox1_TextChanged_1(object sender, EventArgs e)
        { 

        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string newDirectory = textBox1.Text.Trim();
                if (Directory.Exists(newDirectory))
                {
                    currentDirectory = newDirectory;
                    UpdateDirectory(currentDirectory);
                }
                else
                {
                    MessageBox.Show("Некорректный путь!");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;

            if (parentDirectory != null)
            {
                currentDirectory = parentDirectory;
                UpdateDirectory(currentDirectory);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedItem = listView1.SelectedItems[0].Text;
                string newPath = Path.Combine(currentDirectory, selectedItem);

                if (Directory.Exists(newPath))
                {
                    currentDirectory = newPath;
                    UpdateDirectory(currentDirectory);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)                  /// DELETE
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFile = listView1.SelectedItems[0].Text;
                string filePath = Path.Combine(currentDirectory, selectedFile);

                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить? ", "Потвердить Удаление", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(filePath);
                        UpdateDirectory(currentDirectory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("У вас недостаточно прав!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка во время удаления:  " + ex.Message);
                    }
                }
            }
        }


        private List<string> SearchFiles(string directory, string searchQuery)
        {
            List<string> foundFiles = new List<string>();  // Specify the type argument as string

            try
            {
                string[] files = Directory.GetFiles(directory, searchQuery, SearchOption.AllDirectories);
                foundFiles.AddRange(files);
            }
            catch (Exception)
            {
                // Ignore any errors and continue the search
            }

            return foundFiles; ;
        }
 private void button4_Click(object sender, EventArgs e)                 // MOVE
        {

            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFile = listView1.SelectedItems[0].Text;
                string sourcePath = Path.Combine(currentDirectory, selectedFile);

                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationPath = Path.Combine(folderBrowserDialog.SelectedPath, selectedFile);

                    try
                    {
                        File.Move(sourcePath, destinationPath);
                        UpdateDirectory(currentDirectory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("У вас недостаточно прав для перемещения файла.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка в время перехода: " + ex.Message);
                    }
                }
            }
        }

         private void button3_Click(object sender, EventArgs e)                             // FIND
        {
            string searchQuery = textBox3.Text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                List<string> foundFiles = SearchFiles(currentDirectory, searchQuery);
                if (foundFiles.Count > 0)
                {
                    string message = "Найдены файлы, соответствующие критериям поиска:\n\n";
                    message += string.Join("\n", foundFiles);
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show("Файлы, соответствующие критериям поиска, не найдены");
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
