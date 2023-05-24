using System;
using System.Windows.Forms;
using System.IO;

namespace ExplorerApp
{
    public partial class Form1 : Form
    {
        private string currentDirectory;    // задаём переменную как текущая директория

        // Создания экземпляра Form1, ДЛЯ инициализация компонентов формы и настройка списка для отображения данных. 
        public Form1()   
        {
            InitializeComponent();    // инициализация элементов для их использования
            InitializeListView();
        }

        private void InitializeListView()  // Отображение описания папок и файлов
        {

            // Настройка элементов управления ListView
            listView1.View = View.Details;   // С помощью Details добавляем колонки в окно listView1
            listView1.Columns.Add("Имя");     // Columns.Add  - добавление одной колонки
            listView1.Columns.Add("Тип");
            listView1.Columns.Add("Дата создания");
            listView1.Columns.Add("Дата изменения");
            listView1.Columns.Add("Размер");
            listView1.Columns.Add("Автор");

            // Автоматическое изменение размера столбцов в соответствии с содержимым
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);  // Размер столбцов
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);    // Размер заголовок
        }

        // загружаем форму Form1 нашего приложения
        private void Form1_Load(object sender, EventArgs e)
        {
            currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);     // Установка начальной директории на Рабочий стол
            UpdateDirectory(currentDirectory);                                                  // Обновление содержимого текущей директории
        }


        // Операции с адресной строкой и директорией
        private void UpdateDirectory(string path)             // Обновление содержимого текущей директории и отображение его в элементах управления формы, включая адресную строку  и список файлов/папок.
        {    
            try           // для обработки исключений ( если не выполнится, то переходит в catch) 
            {
                currentDirectory = path;  
                textBox1.Text = currentDirectory;  // Обращаемся к свойству Text в textBox1 и обновляем строку директории, присваивая значение currentDirectory
                listView1.Items.Clear();           // Очистка списка файлов и папок

             

                string[] directories = Directory.GetDirectories(currentDirectory);      // Получаем список папок в текущей директории и сохраняем в массиве directories
              
                foreach (string directory in directories)           // цикл для обработки всех папок в директории
                {   
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);  // Создаём экземпляр DirectoryInfo для конкретного объекта этого класса, который представляет определенную папку на диске.  
                    ListViewItem item = new ListViewItem(dirInfo.Name);   //  Добавление папки в список
                    item.SubItems.Add("Папка");                          // указываем тип как папка во втором столбце
                    item.SubItems.Add(dirInfo.CreationTime.ToString());         // преобразования даты в строку для добавления в список  
                    item.SubItems.Add(dirInfo.LastWriteTime.ToString());           
                    listView1.Items.Add(item);                                 // элементы списка добавляем в элемент управления
                }

                 // Получение списка файлов в текущей директории
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (string file in files)               // проводим итерацию по массиву файлов
                {
                    FileInfo fileInfo = new FileInfo(file);         // создаём экземпляр
                    ListViewItem item = new ListViewItem(fileInfo.Name); // Добавление файла в список
                    item.SubItems.Add("Файл");
                    item.SubItems.Add(fileInfo.CreationTime.ToString());
                    item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                    item.SubItems.Add(GetFormattedSize(fileInfo.Length)); //  получение форматированного размера файла    
                    item.SubItems.Add(GetFileOwner(file));                  // получение информации об Авторе
                    listView1.Items.Add(item); 
                }
            }
            catch (UnauthorizedAccessException)  // нет доступа к пути
            {
                MessageBox.Show("Доступ к пути отклонён");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }


        private string GetFormattedSize(long size)    // Для удобства определим единицу измерения размера файла
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (size >= GB)
                return $"{size / GB} GB";
            else 
            if (size >= MB)
                return $"{size / MB} MB";
            else 
            if (size >= KB)
                return $"{size / KB} KB";
            else
                return $"{size} bytes";
        }

        // Вспомогательный метод для получения автора файла
        private string GetFileOwner(string filePath)
        {
            try
            {
                var fileSecurity = File.GetAccessControl(filePath);  // fileSecurity - представляет права доступа к файлу
                var fileOwner = fileSecurity.GetOwner(typeof(System.Security.Principal.NTAccount));   // получить информацию об владельце файлa
                return fileOwner.ToString();  // возвращаем строкой значение fileOwner
            }
            catch (Exception)         
            {
                return "N/A";
            }
        }

        // Строка адреса директории
        private void textBox1_KeyDown(object sender, KeyEventArgs e)   // обработчик события при нажатии Enter           
        {
            if (e.KeyCode == Keys.Enter)   // был ли нажат Enter?
            {
                string newDirectory = textBox1.Text.Trim();   //  Извлекается текст из текстового поля и удаляются лишние пробелы
                if (Directory.Exists(newDirectory))    // проверяем существует ли указанный путь в качестве директории
                {
                    currentDirectory = newDirectory;
                    UpdateDirectory(currentDirectory);
                }
                else
                {
                    MessageBox.Show("Некорректный путь!");      // если путь не существует
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
                    catch (UnauthorizedAccessException)  // нет прав к удалению файла
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

                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();   // создаём экземпляр класса FolderBrowserDialog для того, чтобы выбрать целевую папку для перемещения файла.
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string destinationFolder = folderBrowser.SelectedPath;                           // Получение пути к целевой папке
                    string destinationPath = Path.Combine(folderBrowser.SelectedPath, selectedFile);  // Создание пути к целевому файлу

                    try
                    {
                        File.Move(sourcePath, destinationPath);   // Перемещение файла от.. к ..
                        UpdateDirectory(Path.GetDirectoryName(sourcePath));         // Обновление содержимого текущей директории
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
            else
            {
                MessageBox.Show("Выделите в окне папку или файл");
            }
        }

        // Функция поиска 
        private void button3_Click(object sender, EventArgs e)                             
        {
            string searchQuery = textBox3.Text.Trim();    // Получение поискового запроса 
            if (!string.IsNullOrEmpty(searchQuery))   // проверяем не пустой ли поисковой запрос
            {
                try
                {
                    string[] matchingFiles = Directory.GetFiles(currentDirectory, searchQuery, SearchOption.AllDirectories); // Поиск файлов, соответствующих запросу

                    if (matchingFiles.Length > 0)   // если найдены соответствующие файлы, то очистка списка 
                    {
                        listView1.Items.Clear();

                        foreach (string file in matchingFiles)   // затем запрашиваем информацию о файле в найденном запросе
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
                string searchFolder = folderBrowser.SelectedPath; // Выбор папки для поиска

                textBox1.Text = searchFolder; // Обновление адресной строки

                string[] files = Directory.GetFiles(searchFolder, "*", SearchOption.AllDirectories); // Поиск всех файлов в папке и подпапках

                listView1.Items.Clear(); // Очистка списка файлов и папок

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    if (fileInfo.Exists) // Проверка, существует ли файл перед открытием папки                  
                    {
                        ListViewItem item = new ListViewItem(fileInfo.Name); // Добавление файла в список
                        item.SubItems.Add("Файл");
                        item.SubItems.Add(fileInfo.CreationTime.ToString());
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                        item.SubItems.Add(fileInfo.Length.ToString());
                        listView1.Items.Add(item);
                    }
                    else
                    {
                        // Обработать случай, когда файл не существует
                        MessageBox.Show($"Файл не найден: {file}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void label2_Click(object sender, EventArgs e) {   } 
    } 
}
