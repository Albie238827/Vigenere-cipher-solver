using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vigenere_cipher_solver
{
    public partial class Form1 : Form
    {
        int count = 0;
        int keyLength;
        TextBox[] caesarTextBoxes;
        TextBox[] caesarTextBoxesPT;
        TextBox[] keyTextBoxes;
        TextBox[] finalKeyTextBoxes;


        public Form1()
        {
            InitializeComponent();
            InitializeTextBoxArray();
            InitializeTextBoxArrayPT();
            InitializeTextBoxArrayKeys();
            InitializeTextBoxArrayFinalKey();
        }

        private void InitializeTextBoxArrayFinalKey()
        {
            finalKeyTextBoxes = new TextBox[]
            {
                textBox000, textBox111, textBox222, textBox333, textBox444, textBox555, textBox666, textBox777, textBox888, textBox999, textBox1000
            };
        }

        private void InitializeTextBoxArrayKeys()
        {
            keyTextBoxes = new TextBox[]
            {
                textBox01, textBox02, textBox03,
                textBox11, textBox12, textBox13,
                textBox21, textBox22, textBox23,
                textBox31, textBox32, textBox33,
                textBox41, textBox42, textBox43,
                textBox51, textBox52, textBox53,
                textBox61, textBox62, textBox63,
                textBox71, textBox72, textBox73,
                textBox81, textBox82, textBox83,
                textBox91, textBox92, textBox93,
                textBox101, textBox102, textBox103,
            };
        }

        private void InitializeTextBoxArray()
        {
            caesarTextBoxes = new TextBox[]
            {
                txtCaesar1, txtCaesar2, txtCaesar3, txtCaesar4, txtCaesar5, txtCaesar6, txtCaesar7, txtCaesar8, txtCaesar9, txtCaesar10, txtCaesar11
            };
        }

        private void InitializeTextBoxArrayPT()
        {
            caesarTextBoxesPT = new TextBox[]
            {
                txtCaesarPT1, txtCaesarPT2, txtCaesarPT3, txtCaesarPT4, txtCaesarPT5, txtCaesarPT6, txtCaesarPT7, txtCaesarPT8, txtCaesarPT9, txtCaesarPT10, txtCaesarPT11
            };
        }

        private void txtKeyLength_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtKeyLength.Text))
            {
                keyLength = Convert.ToInt32(txtKeyLength.Text);
            }

            txtCipherText_TextChanged(sender, e);
        }


        private void txtCipherText_TextChanged(object sender, EventArgs e)
        {
            foreach (var box in caesarTextBoxes) box.Clear();
            foreach (var box in caesarTextBoxesPT) box.Clear();

            txtPlainText.Clear();
            txtKey.Clear();

            if (txtCipherText.Text == "")
            {
            }
            else
            {
                count = 0;

                List<int> shiftList = new List<int>();

                string cipherText = normaliseCipherText();

                string[] caesarCiphers = splitInColumns(cipherText);

                foreach (string cipher in caesarCiphers)
                {
                    passToCaesarTxt(cipher, count);

                    int[] frequencies = findFrequencies(cipher);

                    int[] localShiftList = calculateMostFrequent(frequencies);
                    passToKeyTxt(localShiftList, count);
                    passToFinalKeyText(localShiftList, count);

                    string plainText = performShift(cipher, Convert.ToInt32(char.Parse(finalKeyTextBoxes[count].Text.ToString())) - 69);
                    passToPlainTxt(plainText, count);

                    shiftList.Add(localShiftList[0]);
                    count++;
                }
                fillPlainText(count);

                findKey(finalKeyTextBoxes, count);
            }
        }



        private void passToFinalKeyText(int[] localShiftList, int count)
        {
            finalKeyTextBoxes[count].Text = ((char)(64 + localShiftList[0] + 5)).ToString();
        }

        private void passToKeyTxt(int[] localShiftList, int count)
        {
            for (int i = 0; i < 3; i++)
            {
                if (localShiftList[i] == -5)
                {
                    keyTextBoxes[3 * (count + 1) - 3 + i].Text = "";
                    continue;
                }
                int shift = localShiftList[i] + 5; //which letter has become E e.g 1 = a, 2 = b, etc
                char keyLetter = (char)(64 + shift);

                keyTextBoxes[3 * (count + 1) - 3 + i].Text = keyLetter.ToString();
            }
        }

        private void findKey(TextBox[] keyTextBoxes, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int shift = Convert.ToInt32(char.Parse(finalKeyTextBoxes[i].Text.ToString())) - 64; //which letter has become E e.g 1 = a, 2 = b, etc
                int shiftValue = shift - 5; //how many letters have been shifted to get to E
                if (shiftValue < 0)
                {
                    shiftValue = shiftValue + 26;
                }
                char keyLetter = (char)(65 + shiftValue);
                txtKey.Text += keyLetter;
            }
        }

        private void fillPlainText(int count)
        {
            string plainText = "";

            for (int i = 0; i < caesarTextBoxesPT[0].Text.Length; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (i < caesarTextBoxesPT[j].Text.Length)
                    {
                        plainText += caesarTextBoxesPT[j].Text[i];
                    }
                }
            }

            txtPlainText.Text = plainText;
        }

        private void passToPlainTxt(string plainText, int count)
        {
            caesarTextBoxesPT[count].Text = plainText;
        }

        private string[] splitInColumns(string cipherText)
        {
            List<char> tempList = new List<char>();

            for (int i = 0; i < keyLength; i++)
            {
                for (int j = i; j < cipherText.Length; j += keyLength)
                {
                    tempList.Add(cipherText[j]);
                }
                tempList.Add(',');
            }
            tempList.RemoveAt(tempList.Count - 1);

            string ctString = string.Concat(tempList);
            string[] caesarList = ctString.Split(',');
            return caesarList;
        }


        private string performShift(string cipher, int shift)
        {
            List<char> tempList = new List<char>();

            foreach (char letter in cipher)
            {
                int ptLetter = ((int)letter - shift);
                if (ptLetter > 90)
                {
                    ptLetter = ptLetter % 91 + 65;
                }
                else if (ptLetter < 65)
                {
                    ptLetter += 26;
                }
                tempList.Add((char)ptLetter);
            }

            string ptString = string.Concat(tempList);
            return ptString;
        }

        private int[] findFrequencies(string caesarCipherText)
        {
            int[] frequencies = new int[26];
            foreach (char letter in caesarCipherText)
            {
                frequencies[letter - 'A']++;
            }
            return frequencies;
        }

        private void passToCaesarTxt(string caesarCipherText, int count)
        {
            caesarTextBoxes[count].Text = caesarCipherText;
        }

        private int[] calculateMostFrequent(int[] frequencies)
        {
            int[] localShiftValues = new int[3];
            int[] shiftLetters = new int[3];
            shiftLetters[0] = -1;
            shiftLetters[1] = -1;
            shiftLetters[2] = -1;

            foreach (int frequency in frequencies)
            {
                if (frequency > shiftLetters[0] && frequency != 0)
                {
                    shiftLetters[2] = shiftLetters[1];
                    shiftLetters[1] = shiftLetters[0];
                    shiftLetters[0] = frequency;
                }
                else if (frequency > shiftLetters[1] && frequency != 0)
                {
                    shiftLetters[2] = shiftLetters[1];
                    shiftLetters[1] = frequency;
                }
                else if (frequency > shiftLetters[2] && frequency != 0)
                {
                    shiftLetters[2] = frequency;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int shift = -4 + Array.IndexOf(frequencies, shiftLetters[i]);
                localShiftValues[i] = (shift);
                if (shiftLetters[i] != -1)
                {
                    frequencies[Array.IndexOf(frequencies, shiftLetters[i])] = 0;
                }
            }
            return localShiftValues;
        }


        private string normaliseCipherText()
        {
            string cipherText = txtCipherText.Text;
            cipherText = Regex.Replace(cipherText, @"[^a-zA-Z]", "");
            cipherText = cipherText.ToUpper();
            return cipherText;
        }

        private void manualChange(int num, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            finalKeyTextBoxes[num].Text = e.KeyChar.ToString().ToUpper();
            finalKeyTextBoxes[num].SelectionStart = 1; 
            e.Handled = true;

            char letter = char.Parse(e.KeyChar.ToString().ToUpper());
            string plainText = performShift(caesarTextBoxes[num].Text.ToString(), Convert.ToInt32(letter) - 69);
            passToPlainTxt(plainText, num);

            fillPlainText(count);

            txtKey.Clear();
            findKey(finalKeyTextBoxes, count);
        }



        private void textBox000_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(0, e);
        }

        private void textBox111_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(1, e);
        }

        private void textBox222_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(2, e);
        }

        private void textBox333_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(3, e);
        }

        private void textBox444_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(4, e);
        }

        private void textBox555_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(5, e);
        }

        private void textBox666_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(6, e);
        }

        private void textBox777_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(7, e);
        }

        private void textBox888_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(8, e);
        }

        private void textBox999_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(9, e);
        }

        private void textBox1000_KeyPress(object sender, KeyPressEventArgs e)
        {
            manualChange(10, e);
        }
    }
}



