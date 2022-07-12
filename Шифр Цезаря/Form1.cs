using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System;



namespace Шифр_Виженера
{
    public partial class Form1 : Form
    {
        char[,] table = new char[32, 32];
        string key;
        bool flag = false;

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "Выберите действие";
            ActiveControl = textBox2;
            label3.Visible = false;
            comboBox1.Visible = false;

        } // Инициализация формы
        
        private void button1_Click(object sender, EventArgs e) //Зашифровать
        {
            textBox3.Clear();
            key = "-1";
            Event(true);
            toolStripStatusLabel1.Text = "Зашифровано";
        }

        private void button2_Click(object sender, EventArgs e) //Расшифровать
        {
            textBox3.Clear();
            key = "-1";
            Event(false);
            toolStripStatusLabel1.Text = "Расшифровано";
        }

        private void button2_Click(object sender, EventArgs e, string key) //Расшифровать c найденным ключом
        {
            textBox3.Clear();
            this.key = key;
            textBox1.Text = key;
            Event(false);
            toolStripStatusLabel1.Text = "Расшифровано";
        }

        private void button3_Click(object sender, EventArgs e) //Взломать шифр
        {
            textBox1.Clear();
            textBox3.Clear();

            if (!comboBox1.Visible)
            {
                int keyLength = FindKey();
                CreateArray();

                if (keyLength > 0)
                {
                    flag = true;
                    button2_Click(sender, e, Hacking(keyLength));
                    toolStripStatusLabel1.Text = "Взломано";
                }

                else
                {
                    MessageBox.Show("Не удалось найти длину ключа");
                    comboBox1.Visible = true;
                    label3.Visible = true;
                    toolStripStatusLabel1.Text = "Выберите действие";
                }
            }
            else
            {
                byte borders = (byte)comboBox1.SelectedIndex;
                Hacking(borders);
                toolStripStatusLabel1.Text = "Взломано";
            }
        }

        private void Encryption ()
        {
            string lineIn = textBox2.Text, lineOut = "";
            int keyLetter = 0, codeOfChar = 0;
            byte fiveChars = 0;
            
            lineIn = lineIn.ToLower();

            foreach (char ch in lineIn)
            {
                codeOfChar = ch;

                if (codeOfChar == 1025 || codeOfChar == 1105) // обработка буквы "ё"
                    codeOfChar = 1077;

                if (codeOfChar >= 1072 && codeOfChar <= 1103)
                {
                    lineOut += table [key[keyLetter]-1072 , codeOfChar - 1072];
                    keyLetter++;
                    fiveChars++;

                    if (keyLetter == key.Length)
                        keyLetter = 0;

                    if (fiveChars == 5 || fiveChars == 10 || fiveChars == 15 || fiveChars == 20)
                        lineOut += " ";
                    else if (fiveChars == 25)
                    {
                        textBox3.Text += lineOut + Environment.NewLine;
                        fiveChars = 0;
                        lineOut = "";
                    }
                }
            }

            if (fiveChars < 25 && fiveChars > 0)
                textBox3.Text += lineOut;

        } // Зашифрование файла

        private void Decryption ()
        {
            string lineIn = textBox2.Text, lineOut = "";
            int codeOfChar = 0;
            byte keyLetter = 0, fiveChars = 0; ;

            lineIn = lineIn.ToLower();

            foreach (char ch in lineIn)
            {
                codeOfChar = ch;

                if (codeOfChar >= 1072 && codeOfChar <= 1103)
                {   
                    for (byte i = 0; i<32; i++)
                    {
                        if (table[key[keyLetter] - 1072 , i] == codeOfChar)
                        {
                            lineOut += Convert.ToChar(i+1072);
                            fiveChars++;

                            if (fiveChars == 5 || fiveChars == 10 || fiveChars == 15 || fiveChars == 20)
                                lineOut += " ";
                            else if (fiveChars == 25)
                            {
                                textBox3.Text += lineOut + Environment.NewLine;
                                fiveChars = 0;
                                lineOut = "";
                            }
                            break;
                        }
                    }
                    keyLetter++;

                    if (keyLetter == key.Length)
                        keyLetter = 0;
                }
            }

            if (fiveChars < 25 && fiveChars > 0)
                textBox3.Text += lineOut;
        } // Расшифрование файла

        private void Hacking(byte borders)
        {
            byte borderFrom = 0, borderTo = 0;
            
            switch (borders)
            {
                case 0:
                    borderFrom = 1;
                    borderTo = 10;
                    break;
                case 1:
                    borderFrom = 11;
                    borderTo = 20;
                    break;
                case 2:
                    borderFrom = 21;
                    borderTo = 30;
                    break;
                case 3:
                    borderFrom = 31;
                    borderTo = 40;
                    break;
                case 4:
                    borderFrom = 41;
                    borderTo = 50;
                    break;
            }

            string lineIn = textBox2.Text;
            lineIn = lineIn.ToLower();
            

            List<char> alfavit = new List<char>() {
                'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й',
                'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у',
                'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э',
                'ю','я' };

            List<double> chastoti = new List<double>()
            {
                0.062, 0.014, 0.038, 0.013, 0.025,
                0.072, 0.007, 0.016, 0.062, 0.010,
                0.028, 0.035, 0.026, 0.053, 0.090,
                0.023, 0.040, 0.045, 0.053, 0.021,
                0.002, 0.009, 0.003, 0.012, 0.006,
                0.003, 0.014, 0.016, 0.014, 0.003,
                0.006, 0.018
            };

            lineIn = lineIn.Replace(" ", "");
            lineIn = lineIn.Replace(System.Environment.NewLine, "");
            
            string[] keys = new string[10];

            for (byte i = borderFrom; i <= borderTo; i++)
            {
                List<string> subset = new List<string>();

                int countLetter = 0, countLetterTotal = 0, listPlus = 0; 
                sbyte resultM = -1, currentM = 0; //currentM - текущее смещение М, resultM запоминает истинное смещение m 
                double dMin = 2000000000, d = 0;
                bool stop = true;
               
                for (int y = 0; y < lineIn.Length; y += i)
                {
                    if (stop)
                    {
                        subset.Add("");

                        for (int z = y; z < i + y; z++)
                        {
                            if (z < lineIn.Length)
                                subset[listPlus] += Convert.ToString(lineIn[z]);
                            else
                            {
                                stop = false;
                                break;
                            }
                        }
                        listPlus++;
                    }
                    else
                    {
                        stop = true;
                        break;
                    }
                } // после этого будет готовый список со строками нужной длины i
                
                for (int y = 0; y < i; y++)
                {
                    for (sbyte m = 0; m < 32; m++)
                    {
                        for (sbyte j = 0; j < 32; j++) // m - смещение
                        {
                            for (int z = 0; z < subset.Count(); z++)
                            {
                                if (y < subset[z].Length)
                                {
                                    currentM = Convert.ToSByte(j + m);
                                    if (currentM >= 32) currentM -= 32;
                                    
                                    if (subset[z][y] == alfavit[currentM])
                                    {
                                        countLetter++;
                                        countLetterTotal++;
                                    }
                                    else
                                        countLetterTotal++;
                                }
                                else continue;
                            }

                            if (countLetter != 0)
                            d += Math.Pow(chastoti[j] - Math.Round((double) countLetter / countLetterTotal,3) ,2);

                            countLetter = 0;
                            countLetterTotal = 0;
                        }

                        if (dMin > d)
                        {
                            dMin = d;
                            resultM = m;
                        }
                        d = 0;
                    }
                    keys[i - borderFrom] += alfavit[resultM];
                    dMin = 2000000000;
                }
            }

            foreach (string str in keys)
            {
                textBox1.Text = str + Environment.NewLine;
            }
        } // Взлом файла

        private string Hacking(int keyLength)
        {
            string textAll = textBox2.Text;
            textAll = textAll.ToLower();


            List<char> alfavit = new List<char>() {
                'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й',
                'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у',
                'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э',
                'ю','я' };

            List<double> chastoti = new List<double>()
            {
                0.062, 0.014, 0.038, 0.013, 0.025,
                0.072, 0.007, 0.016, 0.062, 0.010,
                0.028, 0.035, 0.026, 0.053, 0.090,
                0.023, 0.040, 0.045, 0.053, 0.021,
                0.002, 0.009, 0.003, 0.012, 0.006,
                0.003, 0.014, 0.016, 0.014, 0.003,
                0.006, 0.018
            };

            textAll = textAll.Replace(" ", "");
            textAll = textAll.Replace(System.Environment.NewLine, "");
            
            List<string> subset = new List<string>();
            int countLetter = 0, countLetterTotal = 0, listPlus = 0, i = keyLength;
            sbyte resultM = -1, currentM = 0; //currentM - текущее смещение М, resultM запоминает истинное смещение m 
            double dMin = 2000000000, d = 0;
            bool stop = true;
            string key = "";

            for (int y = 0; y < textAll.Length; y += i)
            {
                if (stop)
                {
                    subset.Add("");

                    for (int z = y; z < i + y; z++)
                    {
                        if (z < textAll.Length)
                            subset[listPlus] += Convert.ToString(textAll[z]);
                        else
                        {
                            stop = false;
                            break;
                        }
                    }
                    listPlus++;
                }
                else
                {
                    stop = true;
                    break;
                }
            } // после этого будет готовый список со строками нужной длины i

                for (int y = 0; y < i; y++)
                {
                    for (sbyte m = 0; m < 32; m++)
                    {
                        for (sbyte j = 0; j < 32; j++) // m - смещение
                        {
                            for (int z = 0; z < subset.Count(); z++)
                            {
                                if (y < subset[z].Length)
                                {
                                    currentM = Convert.ToSByte(j + m);
                                    if (currentM >= 32) currentM -= 32;

                                    if (subset[z][y] == alfavit[currentM])
                                    {
                                        countLetter++;
                                        countLetterTotal++;
                                    }
                                    else
                                        countLetterTotal++;
                                }
                                else continue;
                            }

                            if (countLetter != 0)
                                d += Math.Pow(chastoti[j] - Math.Round((double)countLetter / countLetterTotal, 3), 2);

                            countLetter = 0;
                            countLetterTotal = 0;
                        }

                        if (dMin > d)
                        {
                            dMin = d;
                            resultM = m;
                        }
                        d = 0;
                    }
                    key += alfavit[resultM];
                    dMin = 2000000000;
                }
            return key;
        } // Взлом файла

        private int FindKey ()
        {
            string lineIn = textBox2.Text, currentFrase = "", fraseToCompare = "";
            int help = 0, keyLength = 0, q = 10, u = 0;
            List<int> numberEqualsBefore = new List<int>();
            bool flag = false;

            lineIn = lineIn.ToLower();
            lineIn = lineIn.Replace(" ", "");
            lineIn = lineIn.Replace(System.Environment.NewLine, "");

            

            while(numberEqualsBefore.Count() < 50 && q >= 3)
            {
                for (int a = 0; a < lineIn.Length - q; a++)
                {
                    int j = a;
                    while (currentFrase.Length != q)
                    {
                        currentFrase += lineIn[j];
                        j++;
                    }

                    for (int z = a+1; z < lineIn.Length - q; z++)
                    {
                        for (int p = z; p < z+q; p++)
                        {
                            if (currentFrase[p-z] == lineIn[p])
                                fraseToCompare += lineIn[p];
                            else break;
                        }

                        if (currentFrase == fraseToCompare)
                        {
                            numberEqualsBefore.Add(z - a);
                        }
                        fraseToCompare = "";
                    }
                    currentFrase = "";
                }
                q--;
            }

            numberEqualsBefore.Sort();
            int[] numberEqualsFinal = new int[numberEqualsBefore.Count()];
            
            numberEqualsFinal[0] = numberEqualsBefore[0];

            for (int i = 1; i < numberEqualsBefore.Count(); i++)
            {
                if (numberEqualsBefore[i] != numberEqualsFinal[u])
                {
                    u++;
                    numberEqualsFinal[u] = numberEqualsBefore[i];
                }
            }

            help = numberEqualsFinal.Max();

            for (int y = help; y > 0; y--)
            {
                if (flag == true)
                {
                    keyLength = y+1;
                    break;
                }
                
                for (int i = 0; i < numberEqualsFinal.Count(); i++)
                {
                    if (numberEqualsFinal[i] % y == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return keyLength;

        } // поиск длины ключа

        private void Event(bool enc)
        {
            if ((key == "-1" && CheckKey()) || (key != "-1" && flag)) //если ключ уже создался на взломе
            {
                flag = false;
                key = key.ToLower();
                
                if (enc)
                    Encryption();
                else
                    Decryption();
            }
            else
            {
                MessageBox.Show("Ошибка ввода ключа");
                textBox1.Text = "";
                ActiveControl = textBox1;
            }
        } // Действия при нажатии на кнопки Зашифровать или Расшифровать
        
        private bool CheckKey()
        {
            key = "";
            CreateArray();
            for (int i = 0; i < textBox1.Text.Length; i++)

                if (textBox1.Text[i] != 1025 && textBox1.Text[i] != 1105)
                    key += textBox1.Text[i];
                else
                    key += Convert.ToChar(1077);


            if (key == "")
                return false;
            else
            {
                key = key.ToLower();

                foreach (char ch in key)
                {
                    if (ch < 1072 || ch > 1103)
                    {
                        key = "";
                        return false;
                    }

                }
                return true;
            }
        } // Проверка ключа

        private void CreateArray()
        {
            int count = 1072;
            for (byte i = 0; i < 32; i++)
            {
                for (byte y = 0; y < 32; y++)
                {
                    table[i, y] = Convert.ToChar(count);
                    count++;

                    if (count == 1104)
                        count -= 32;
                }
                count++;
            }
        } // Создание таблицы шифрования

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            comboBox1.Visible = false;
            label3.Visible = false;
        }
    }
}
//this.comboBox1.SelectedIndex = 0;
