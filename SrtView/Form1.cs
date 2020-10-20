// Вельдяйкин Александр Олегович ИЭС 165-19 Отображение субтитров

using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO.Compression;

//<.*?>

namespace SrtView
{
    using HWND = IntPtr;
    public partial class Form1 : Form
    {
        public Form1() // конструктор формы
        {
            InitializeComponent(); // функция инициализации компонентов и самой формы
        }

        string path; // переменная хранящая путь к файлу субтитров
        StreamReader sr, sd; // переменные хранящие свойства читателей файла
        string[] splits; // массив хранящий слова после деления строки
        int hours = 0, minutes = 0, seconds = 0,msp = 0; // переменные хранящие параметры времени
        int time,ms, mtime = 0; // переменные хранящие время
        string preline; // переменная хранящая строку перед активной
        int row; // переменная для хранения номера строки
        int endfile; //переменная для хранения количества строк в файле
        string lasttime; // переменная для хранения последнего временного кода файла
        string lasttext; // переменная для хранения номера последней фразы
        bool vis = true;
        

        /// <summary>
        /// функция сбрасывающая таймер и производящая анализ файла
        /// </summary>
        private void Restart()
        {
            //AXVLC.DotNet.Core.Medias.LocationMedia
            int timer; // переменная хранящая код времени, который ввел пользователь
            string line; // переменная для хранения строки из файла
            if (textBox1.Text != "") // если ввод не пустой
            {
                try
                {
                    textBox1.BackColor = Color.White;
                    timer = Convert.ToInt32(textBox1.Text); // запись в переменную данных из ввода
                    seconds = timer % 100; //выборка секунд из ввода
                    timer /= 100; // отрезание секунд из ввода
                    minutes = timer % 100; //выборка минут из ввода
                    timer /= 100; // отрезание минут из ввода
                    hours = timer; // выборка часов из ввода
                }
                catch
                {
                    textBox1.BackColor = Color.Red;
                }
            }
            sr = new StreamReader(path); // создание читателя из файла
            endfile = 0; // обнуление переменной

            while ((line = sr.ReadLine()) != null) // пока не наступил конец файла
            {
                splits = line.Split(':'); // разделения временного кода
                if (splits[0] == "00" || splits[0] == "01" || splits[0] == "02" || splits[0] == "03") // если первое в строке это количество часов 
                {
                    //lasttime = splits[0] + ":" + splits[1]; // записать в переменную количество часов и минут последнего временного кода
                    lasttext = preline; // запись номера последней фразы в файле
                }
                preline = line; // запись строки для хранения
                endfile++; // увеличение счётчика колличества строк в файле
            }
            sr = new StreamReader(path); // создание читателя из файла
            row = 0; // обнуление переменной
            time = hours * 10000 + minutes * 100 + seconds; // запись времени введеного пользователем
            label4.Text = hours + ":" + minutes + ":" + seconds; // отображение в форме какое время стоит в программе
            Timework(); // запуск функции поиска следующей строки для отображения
        }

        /// <summary>
        /// Выбор файла и проверка правильности
        /// </summary>
        private bool Startpath()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // проверка выбора файла
            {
                var pathway = openFileDialog1.FileName.Split('.'); // разделение строки пути к файлу
                if (pathway[pathway.Length - 1] == "txt" || pathway[pathway.Length - 1] == "srt") // проверка расширения файла
                {
                    path = openFileDialog1.FileName; // сохранение пути к файлу
                    Restart(); // запуск функции, сбрасывающей таймер и анализирующей файл
                    return true; // возвращения, что файл правильный
                }
                else if (pathway[pathway.Length - 1] == "zip")
                {
                    string ZipPath = openFileDialog1.FileName;
                    pathway = openFileDialog1.FileName.Split('\\');
                    string ExtractPath = "";
                    for (int i = 0; i < pathway.Length-1 ;i++)
                        ExtractPath += pathway[i] + "\\";
                    FileStream ZipToOpen = new FileStream(ZipPath, FileMode.Open);
                    ZipArchive Archive = new ZipArchive(ZipToOpen, ZipArchiveMode.Update, true, Encoding.GetEncoding("cp866"));
                    ZipArchiveExtension.ExtractToDirectory(Archive, ExtractPath);
                    for (int i = 0; i < Archive.Entries.Count; i++)
                    {
                        pathway = Archive.Entries[i].Name.Split('.');
                        if (pathway[pathway.Length - 1] == "txt" || pathway[pathway.Length - 1] == "srt") // проверка расширения файла
                        {
                            path = ExtractPath + Archive.Entries[i].Name; // сохранение пути к файлу
                            Restart(); // запуск функции, сбрасывающей таймер и анализирующей файл
                            return true; // возвращения, что файл правильный
                        }
                    }
                     
                }
            }
            return false; // возвращение, что файл неправильный
        }

        /// <summary>
        /// Поиск следующей строки для отображения
        /// </summary>
        private void Timework()
        {
            string line; // переменная для хранения строки из файла
            string[] splits2; // массив для кранения слов из строки файла

            while ((line = sr.ReadLine()) != null) // пока идет чтение файла
            {
                row++; // умеличить счётчик строк
                splits = line.Split(':'); // разделение кода времени на компоненты
                if (splits[0] == "00" || splits[0] == "01" || splits[0] == "02" || splits[0] == "03") // если первое в строке это количество часов
                {
                    splits2 = splits[2].Split(','); // отделение милисекунд от секунд
                    mtime = Convert.ToInt32(splits[0]) * 10000 + Convert.ToInt32(splits[1]) * 100 + Convert.ToInt32(splits2[0]); // соединение временного кода
                    
                    ms = Convert.ToInt32(splits2[1].Remove(3,splits2[1].Length-3));
                    if (mtime >= time) // если временной код из файла превышает текущее время
                    {
                        break; // выход из цикла
                    }
                }
                preline = line; // запись линии для хранения
            }
            //label5.Text = row.ToString() + " / " + endfile.ToString(); // отображение сравнения текущей строки к количеству строк в файле
        }
        
        /// <summary>
        /// нажатие на кнопку переход
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (splits[0] != "Wrong file")
                Restart(); // запуск функции сбрасывающей таймер и анализирующей файл
        }

        

        /// <summary>
        /// нажатие на кнопку выбор файла
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (Startpath() == false) // если функция вернула true
            {
                splits = new[] { "Wrong file" }; // 
            }
        }

        /// <summary>
        /// нажатие на кнопку рамка
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (FormBorderStyle == FormBorderStyle.Sizable) // если у формы есть рамка
            {
                FormBorderStyle = FormBorderStyle.None; // отключение рамки
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable; // включение рамки
            }
        }

        /// <summary>
        /// нажатие на кнопку банер
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            if (TopMost == true) // если форма является банером
            {
                TopMost = false; // выключить свойство банера
            }
            else
            {
                TopMost = true; // включить свойство банера
            }
        }

        /// <summary>
        /// Нажатие на кнопку пауза
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true) // если таймер включон
            {
                timer1.Enabled = false; // выключить таймер
                button2.BackColor = Color.Red; // сделать кнопку красной
                Video_click();
            }
            else
            {
                Video_click();
                if (splits[0] != "Wrong file")
                {
                    timer1.Enabled = true; // включить таймер
                }
                button2.BackColor = Color.Green; // сделать кнопку зеленой
            }
        }

        
        /// <summary>
        /// запуск выполнения программы
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Startpath() == false) // если функция вернула true
            {
                splits = new[] { "Wrong file" }; // 
            }
            //sd = new StreamReader("dictionary.txt");
            //controlscount = Controls.Count - 1; // сохранение количества объектов на форме
            
        }

        /// <summary>
        /// функция хода времени
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            string line;
            if (splits[0] != "00" && splits[0] != "01" && splits[0] != "02" && splits[0] != "03") // если первое слово в строке это не часы
            {
                Timework(); // запуск функции поиска следующей строки для отображения
            }
            msp += 100;
            if (mtime <= time && ms <= msp) // если время следующей строки равно времени программы
            {
                Regex regex = new Regex("<.*?>"); // создание регулярного выражения
                label6.Text = regex.Replace(sr.ReadLine(), "");
                if ((line = sr.ReadLine()) != null)
                {
                    label1.Text = regex.Replace(line, "");
                }
                this.AutoSize = true;
                row += 2; // смещение указателя строки
                label3.Text = preline + " / " + lasttext; // отображение сравнения текущего номера фразы и номера последней фразы
                if (Convert.ToInt32(preline) == Convert.ToInt32(lasttext))
                {
                    timer1.Enabled = false;
                    button2.BackColor = Color.Red;
                }
                splits[0] = ""; // удаление информации из первого элемента массива
                //Timework();
                //label5.Text = row.ToString() + " / " + endfile.ToString(); // отображение сравнения текущего номера строки к общему колличеству строк в файле
                
            }
            //seconds++; // увеличение счётчика секунд
            if (msp == 1000)
            {
                msp = 0;
                seconds++;
                time = hours * 10000 + minutes * 100 + seconds; // запись времени
                lasttime = (mtime / 10000).ToString();
                lasttime += ":" + (mtime / 100 % 100);
                lasttime += ":" + (mtime % 100);
                label4.Text = hours + ":" + minutes + ":" + seconds + " / " + lasttime; // отображение сравнения текущего времени к времени последней фразы
            }
            if (seconds == 60) // если секунды равны 60
            {
                minutes++; // увеличение счетчика минут
                seconds = 0; // обнуление счётчика секунд
                time = hours * 10000 + minutes * 100 + seconds; // запись времени
            }
            if (minutes == 60) // если минуты равны 60
            {
                hours++; // увеличение счетчика часов
                minutes = 0; // обнуление счетчика минут
                time = hours * 10000 + minutes * 100 + seconds; // запись времени
            }

        }

        private void label6_SizeChanged(object sender, EventArgs e)
        {

            label6.Left = this.Width / 2 - label6.Width / 2;//привели в центр
            label1.Left = this.Width / 2 - label1.Width / 2;//привели в центр
            
        }


        private void label1_SizeChanged(object sender, EventArgs e)
        {
            label6.Left = this.Width / 2 - label6.Width / 2;//привели в центр
            label1.Left = this.Width / 2 - label1.Width / 2;//привели в центр
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (vis == true)
            {
                vis = false;
                label4.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                button1.Visible = false;
                button4.Visible = false;
                button6.Visible = false;
                button5.Visible = false;
                textBox1.Visible = false;
                comboBox1.Visible = false;
            }
            else
            {
                vis = true;
                label4.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                button1.Visible = true;
                button4.Visible = true;
                button6.Visible = true;
                button5.Visible = true;
                textBox1.Visible = true;
                comboBox1.Visible = true;
            }
        }

        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            IDictionary<HWND, string> dw;
            dw = OpenWindowGetter.GetOpenWindows();
            foreach (KeyValuePair<HWND, string> text in dw)
            {
                string[] name = text.Value.Split(' ');
                if(name[0] == "SubView")
                {
                    comboBox1.Items.Add("None");
                }
                else
                {
                    comboBox1.Items.Add(text.Value);
                }
                
            }
        }

        

        // Send a series of key presses to the Video application.
        private void Video_click()
        {
            if (comboBox1.Text != "None")
            {
                IntPtr VideoHandle;
                // Get a handle to the Video application. The window class
                // and window name were obtained using the Spy++ tool.
                VideoHandle = FindWindow(null, comboBox1.Text);
                // Verify that Video is a running process.
                if (VideoHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Video is not running.");
                    return;
                }
                // Make Video the foreground application and send it
                // a set of calculations.
                SetForegroundWindow(VideoHandle);
                SendKeys.SendWait(" ");
            }
        }

    }
}
