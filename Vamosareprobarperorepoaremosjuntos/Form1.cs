using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.IO;

namespace Vamosareprobarperorepoaremosjuntos
{
    public partial class Form1 : Form
    {
        string conectadobb = "Server=localhost; Port=3306; Database=Exapractico4; UID=root; Pwd=12252004;";
        SerialPort arduinoPort = new SerialPort("COM6", 9600);
        Timer updateTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            txtnombre.TextChanged += validarnom;
            txtdia.Leave += validardate;
            txtmes.Leave += validardate;
            txtano.Leave += validardate;

            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTemperature;

            arduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!arduinoPort.IsOpen)
                {
                    arduinoPort.Open();
                    MessageBox.Show("Arduino conectado.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("El puerto está en uso por otro programa.");
            }
            catch (IOException ioEx)
            {
                MessageBox.Show("Error al abrir el puerto: " + ioEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error desconocido: " + ex.Message);
            }
        }

        private void InsertarRegistrosusuario(string nombre, int dia, int mes, int ano)
        {
            string insertQuery = "INSERT INTO registro(Nom_usuario, dia, mes, ano) VALUES (@nombre, @dia, @mes, @ano)";

            using (MySqlConnection connection = new MySqlConnection(conectadobb))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@dia", dia);
                        command.Parameters.AddWithValue("@mes", mes);
                        command.Parameters.AddWithValue("@ano", ano);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void InsertarRegistrostem(float tem, string estado)
        {
            string insertQuery = "INSERT INTO tem(Tem_detetada, Estado_servo) VALUES (@tem, @estado)";

            using (MySqlConnection connection = new MySqlConnection(conectadobb))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@tem", tem);
                        command.Parameters.AddWithValue("@estado", estado);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float tempe = float.Parse(lblectem.Text);
            string estado = "encendido";

            InsertarRegistrostem(tempe, estado);

            if (arduinoPort.IsOpen)
            {
                arduinoPort.WriteLine("1");
                updateTimer.Start();
            }
            else
            {
                MessageBox.Show("El puerto serie no está abierto.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float tempe = float.Parse(lblectem.Text);
            string estado = "apagado";

            InsertarRegistrostem(tempe, estado);

            if (arduinoPort.IsOpen)
            {
                arduinoPort.WriteLine("0");
                updateTimer.Stop();
            }
            else
            {
                MessageBox.Show("El puerto serie no está abierto.");
            }
        }

        private void UpdateTemperature(object sender, EventArgs e)
        {
            if (arduinoPort.IsOpen)
            {
                arduinoPort.WriteLine("GET_TEMP");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string data = arduinoPort.ReadLine();
            if (float.TryParse(data, out float temperature))
            {
                Invoke(new Action(() =>
                {
                    lblectem.Text = temperature.ToString("F1");
                    InsertarRegistrostem(temperature, "encendido");
                }));
            }
        }

        private void lblectem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"La temperatura actual es: {lblectem.Text} °C");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string nombre = txtnombre.Text;
            int dia = int.Parse(txtdia.Text);
            int mes = int.Parse(txtmes.Text);
            int ano = int.Parse(txtano.Text);

            InsertarRegistrosusuario(nombre, dia, mes, ano);
        }

        private bool Estextovalido(string valor) => Regex.IsMatch(valor, @"^^[a-zA-Z\s]+$");
        private bool EsEnteroValido2(string valor) => long.TryParse(valor, out long resultado) && valor.Length == 2;

        private void validardate(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.Text.Length == 2 && EsEnteroValido2(textbox.Text))
            {
                textbox.BackColor = ColorTranslator.FromHtml("#00FF00");
            }
            else
            {
                textbox.BackColor = ColorTranslator.FromHtml("#FF0000");
                MessageBox.Show("INGRESE UNICAMENTE NÚMEROS", "INGRESE UNA FECHA VÁLIDA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void validarnom(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!Estextovalido(textBox.Text))
            {
                MessageBox.Show("INGRESE ÚNICAMENTE VALORES DE LETRAS", "INGRESE UN NOMBRE VÁLIDO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox.Clear();
            }
        }
    }
}