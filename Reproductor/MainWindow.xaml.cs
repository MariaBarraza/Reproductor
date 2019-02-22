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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using Microsoft.Win32;

namespace Reproductor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //lee el arvchivo de audio
        AudioFileReader reader;
        //dispositivo de salida, responde a los eventos como pausa, play y eso, se comunica con la tarjeta de sonido
        //nuestra comunicacion con la tarjeta de sonido
        WaveOutEvent output;

        public MainWindow()
        {
            InitializeComponent();
            LlenarComboSalida();
        }

        private void LlenarComboSalida()
        {
            cbSalida.Items.Clear();
            for(int i=0; i<WaveOut.DeviceCount; i++)
            {
                //da las capacidades disponibles de un dispositivo en especifico
                WaveOutCapabilities capacidades = WaveOut.GetCapabilities(i);
                //se añaden al combobox los nombres 
                cbSalida.Items.Add(capacidades.ProductName);
            }
            cbSalida.SelectedIndex = 0;
        }

        private void btnElegirArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = 
                new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtRutaArchivo.Text =
                     openFileDialog.FileName;
            }
        }

        private void btnReproducir_Click(object sender, RoutedEventArgs e)
        {
            
            //evitar duplicado despues del primer play
            //si ademas de ya tener el output esta pausado
            if(output!= null && output.PlaybackState == PlaybackState.Paused)
            {
                output.Play();

                //activamos y desactivamos los botones para poner restricciones en la interfaz
                btnReproducir.IsEnabled = false;
                btnPausa.IsEnabled = true;
                btnDetener.IsEnabled = true;
            }
            else
            {
                reader = new AudioFileReader(txtRutaArchivo.Text);
                output = new WaveOutEvent();

                //aqui se cambia donde se reproduce
                output.DeviceNumber = cbSalida.SelectedIndex;

                //se añaden reacciones a los eventos con += pues asi se sobrecarga el operador
                //con tab crea la funcion visual (output_playbackstopped)
                output.PlaybackStopped += Output_PlaybackStopped;
                //inisializamos el output
                output.Init(reader);
                output.Play();

                //activamos y desactivamos los botones para poner restricciones en la interfaz
                btnDetener.IsEnabled = true;
                btnPausa.IsEnabled = true;
                btnReproducir.IsEnabled = false;

                //se quitn los milisegundos y eso
                lblTiempoFinal.Text = reader.TotalTime.ToString().Substring(0, 8);
            }
            
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            reader.Dispose();
            output.Dispose();
        }

        private void btnPausa_Click(object sender, RoutedEventArgs e)
        {
            if(output != null)
            {
                output.Pause();

                //activamos y desactivamos los botones para poner restricciones en la interfaz
                btnDetener.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnReproducir.IsEnabled = true;
            }
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            if (output!= null)
            {
                output.Stop();

                //activamos y desactivamos los botones para poner restricciones en la interfaz
                btnDetener.IsEnabled = false;
                btnPausa.IsEnabled = false;
                btnReproducir.IsEnabled = true;
            }
        }
    }
}
