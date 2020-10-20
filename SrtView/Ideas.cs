using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;

namespace IdeasSpace
{
    public static class Ideas
    {
        //int x = 10;
        //int y = 1000;
        //mouse_event((uint)MouseEventFlags.LEFTDOWN, x, y, 0, 0);
        //mouse_event((uint)MouseEventFlags.LEFTUP, x, y, 0, 0);
        [DllImport("User32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        public static void tim_tick()
        {
            #region tagsproc
            //line = sr.ReadLine();
            //line = line.Replace("<i>", "~ ").Replace("</i>"," ~");
            //splits = line.Split(' ');
            //if (splits[0] == "~")
            //{
            //    label1.Font = new System.Drawing.Font(label1.Font,System.Drawing.FontStyle.Italic);
            //}
            //else
            //{
            //    label1.Font = new System.Drawing.Font(label1.Font, System.Drawing.FontStyle.Regular);
            //}
            //for(int i = 1; i < splits.Length - 1; i++)
            //{
            //    label1.Text += splits[i];
            //}
            #endregion
            Regex regex = new Regex("<.*?>");
            StreamReader sr=null;
            Alabel(1, regex.Replace(sr.ReadLine(), "")); // запуск функции отображение фразы на 1 строке программы
            Alabel(2, regex.Replace(sr.ReadLine(), "")); // запуск функции отображение фразы на 2 строке программы
        }
        /// <summary>
        /// Отображение фразы на форме в виде отдельных слов
        /// </summary>
        /// <param name="row"> строка отображения в форме</param>
        /// <param name="line"> фраза которую надо отобразить</param>
        private static void Alabel(int row, string line)
        {
            Control.ControlCollection Controls = null;
            int controlscount = 0; // количество объектов со словами
            string[] text; // переменная для хранения слов из строки
            List<Label> labels = new List<Label>(); // создание списка лейблов для слов
            text = line.Split(' '); // разделение фразы на слова
            if (row == 1) // если сейчас первая строка в отображении
            {
                for (int i = Controls.Count - 1; i > controlscount; i--) // пока объектов формы больше чем было изначально
                {
                    Controls.RemoveAt(i); // удаление объекта с формы
                }
            }
            for (int i = 0; i < text.Length; i++) // пока массив слов не пройден
            {
                labels.Add(new Label()); // создание лейбла
                labels[i].AutoSize = true; // включение авторазмера
                labels[i].BackColor = Color.Transparent; // отключение заднего фона
                labels[i].Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 0); // установка шрифта текста 
                labels[i].ForeColor = Color.White; // установка цвета шрифта
                labels[i].Name = "alabel" + i; // обозначение имени лейбла
                labels[i].Size = new Size(108, 29); // установка размера лейбла
                labels[i].Click += new EventHandler(label1_Click); // добавление события к лейблу
                labels[i].TabIndex = 10; // установка уровня отображения на форме
                labels[i].Text = text[i]; // добавление текста в лейбл
                if (row == 1) // если первая строка
                {
                    labels[i].Location = new Point(12, 9); // обозначение места отрисовки лебла
                }
                else
                {
                    labels[i].Location = new Point(12, 47); // обозначение места отрисовки лебла
                }
                if (i > 0) // если это не первый лейбл
                {
                    int x = labels[i - 1].Location.X + labels[i - 1].Size.Width; // запись места нахождения лейбла по оси х
                    labels[i].Location = new Point(x + 5, labels[i - 1].Location.Y); // отрисовка нового лейбла со смещением от старого
                }
                Controls.Add(labels[i]); // добавление лебла в список объектов формы
            }
        }
        /// <summary>
        /// Нажатие на лейбл со словом
        /// </summary>
        private static void label1_Click(object sender, EventArgs e)
        {
            StreamReader sd = null;
            string line; // перепеная для хранения строки из файла перевода
            //string[] sline;
            string[] text; // массив для хранения слов из лейбла
            Regex regex = new Regex(@"[\W^ ]"); // создание регулярного выражения
            sd = new StreamReader("ENRUS.TXT"); // создание читателя файла словаря
            text = sender.ToString().Split(' '); // разделение строки от нажатия на лейбл
            text[0] = regex.Replace(text[text.Length - 1], ""); // замена символов в слове
            text[0] = text[0].ToLower(); // приведение слова к нижнему регистру
            while ((line = sd.ReadLine()) != null) // пока не достигнут конец файла
            {
                //sline = line.Split('-');
                if (text[0] == line) // если нажатое слово совпадает со словом в словаре
                {
                    //label2.Text = sd.ReadLine().Replace('\t', ' '); // замена табуляции на пробелы
                    break; // выход из цикла
                }
            }
        }
        /// <summary>
        /// нажатие на кнопку выход
        /// </summary>
        private static void button3_Click(object sender, EventArgs e)
        {
            //Close(); // закрытие приложения
        }
    }
}
