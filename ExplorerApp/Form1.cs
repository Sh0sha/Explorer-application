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

          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);     // Установка начальной директории на Рабочий стол
            UpdateDirectory(currentDirectory);                                                  // Обновление содержимого текущей директории
        }
       

      // Операции с адресом директории

    private void UpdateDirectory(string path)             
        {    
            try
            {
                currentDirectory = path;   

                // Обновляем строку директории 

                textBox1.Text = currentDirectory;

                // Очистка списка файлов и папок

                listView1.Items.Clear();

                // Получание списка папок в текущей директории

                string[] directories = Directory.GetDirectories(currentDirectory);
              
                foreach (string directory in directories)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    ListViewItem item = new ListViewItem(dirInfo.Name);   //  Добавление папки в список
                    item.SubItems.Add(dirInfo.CreationTime.ToString());
                    item.SubItems.Add(dirInfo.LastWriteTime.ToString());
                    item.SubItems.Add("Путь");
                    listView1.Items.Add(item);
                }

                // Получение списка файлов в текущей директории

                string[] files = Directory.GetFiles(currentDirectory);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    ListViewItem item = new ListViewItem(fileInfo.Name); // Добавление файла в список
                    item.SubItems.Add("Файл");
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

       // Строка адреса директории

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
        
        // Фунция перехода на предыдущую директорию

        private void button1_Click(object sender, EventArgs e)              
        {
            string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;  // Получение родительской директории

            if (parentDirectory != null)
            {
                currentDirectory = parentDirectory;         // Обновление текущей директории
                UpdateDirectory(currentDirectory);           // Обновление содержимого текущей директории
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // ГЛАВНОЕ окно с папками и файлами

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)            
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedItem = listView1.SelectedItems[0].Text;       // Получение имени выбранной папки 
                string newPath = Path.Combine(currentDirectory, selectedItem);   // Создание нового пути к выбранной папке

                if (Directory.Exists(newPath))
                {
                    currentDirectory = newPath;   // Обновление текущей директории
                    UpdateDirectory(currentDirectory);  // Обновление содержимого текущей директории
                }
            }
        }
            
        //  Функция удаление файла или папки

        private void button2_Click(object sender, EventArgs e)                  
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFile = listView1.SelectedItems[0].Text;   // Получение имени выбранного файла
                string filePath = Path.Combine(currentDirectory, selectedFile);   // Создание пути к выбранному файлу

                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить? ", "Потвердить Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(filePath);  // Удаление файла
                        UpdateDirectory(currentDirectory);   // Обновление содержимого текущей директории
                    }
                    catch (UnauthorizedAccessException)  // нет доступа
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

        //  Функция перемещения файла в другую папку

        private void button4_Click(object sender, EventArgs e)         
        {

            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFile = listView1.SelectedItems[0].Text;              // Получение имени выбранного файла
                string sourcePath = Path.Combine(currentDirectory, selectedFile);   // Создание пути к выбранному файлу

                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string destinationFolder = folderBrowser.SelectedPath;                           // Получение пути к целевой папке
                    string destinationPath = Path.Combine(folderBrowser.SelectedPath, selectedFile);  // Создание пути к целевому файлу

                    try
                    {
                        File.Move(sourcePath, destinationPath);   // Перемещение файла
                        UpdateDirectory(currentDirectory);         // Обновление содержимого текущей директории
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("У вас недостаточно прав для перемещения файла.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка в время перехода: " + ex.Message, "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
            
        // Функция поиска 

         private void button3_Click(object sender, EventArgs e)                             
        {
            string searchQuery = textBox3.Text.Trim();    // Получение поискового запроса 
            if (!string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    string[] matchingFiles = Directory.GetFiles(currentDirectory, searchQuery, SearchOption.AllDirectories); // Поиск файлов, соответствующих запросу

                    if (matchingFiles.Length > 0)
                    {
                        listView1.Items.Clear();

                        foreach (string file in matchingFiles)
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            ListViewItem item = new ListViewItem(fileInfo.Name); // Добавление найденных файлов в список
                            item.SubItems.Add("Файл");
                            item.SubItems.Add(fileInfo.CreationTime.ToString());
                            item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                            item.SubItems.Add(fileInfo.Length.ToString());
                            listView1.Items.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Файлы, соответствующие поисковому запросу, не найдены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при поиске файлов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // Функция ручного выбора папки в диалоговом окне

       private void button5_Click(object sender, EventArgs e)               
       {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string searchFolder = folderBrowser.SelectedPath;            // Выбор папки для поиска

                string[] files = Directory.GetFiles(searchFolder, "*", SearchOption.AllDirectories);   // Поиск всех файлов в папке и подпапках

                listView1.Items.Clear();  // Очистка списка файлов и папок

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    ListViewItem item = new ListViewItem(fileInfo.Name);          // Добавление файла в список
                    item.SubItems.Add("Файл");
                    item.SubItems.Add(fileInfo.CreationTime.ToString());
                    item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                    item.SubItems.Add(fileInfo.Length.ToString());
                    listView1.Items.Add(item);
                }
            }
        }




    }

}
