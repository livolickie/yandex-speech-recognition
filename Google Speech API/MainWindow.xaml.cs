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
using NAudio.Midi;
using System.Net;
using System.IO;
using System.Xml;

namespace Google_Speech_API
{
    public partial class MainWindow : Window
    {
        public WaveIn waveIn = null;
        public WaveStream waveStream = null;
        public WaveFileWriter waveFileWriter = null;
        public BufferedWaveProvider bufferedWaveProvider = null;
        public int tempWav = 0;
        bool record = false;
        string key_data = "uuid=&key=&";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!record)
            {
                record = true;
                waveIn = new WaveIn();
                waveIn.WaveFormat = new WaveFormat();
             //   waveStream = new WaveFormatConversionStream(waveIn.WaveFormat, new );
                waveFileWriter = new WaveFileWriter("sound"+tempWav+".wav", waveIn.WaveFormat);
               // bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
               //  bufferedWaveProvider.DiscardOnBufferOverflow = true;
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += WaveIn_RecordingStopped;
                waveIn.StartRecording();
            }
        }

        public async void SpeachProcess(int fileNum)
        {
            string postData = key_data +
                "topic=queries&lang=ru-RU";
            string uri = "https://asr.yandex.net/asr_xml?" + postData;
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            httpWebRequest.KeepAlive = false;
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "audio/x-wav";
            byte[] data = File.ReadAllBytes("sound" + fileNum + ".wav");
            httpWebRequest.ContentLength = data.Length;
            Stream requestStream = await httpWebRequest.GetRequestStreamAsync();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            XmlDocument xmlDoc = new XmlDocument();
            string responseText = await new StreamReader(httpWebResponse.GetResponseStream()).ReadToEndAsync();
            xmlDoc.LoadXml(responseText);
            foreach (XmlNode node in xmlDoc.SelectNodes("recognitionResults"))
            {
                foreach (XmlNode nodeChild in node.ChildNodes)
                    if (nodeChild.Name == "variant")
                    {
                        textBox_1.Text += nodeChild.InnerText + "\n";
                    }
            }
            textBox_1.Text += "\n";
            File.Delete("sound" + fileNum + ".wav");
            httpWebResponse.Dispose();
            GC.Collect();
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (!record)
            {
                waveFileWriter.Close();
                waveFileWriter.Dispose();
                waveIn.Dispose();
                GC.Collect();
                SpeachProcess(tempWav);
                tempWav++;
            }
            else
            {
                SpeachProcess(tempWav);
                tempWav++;
                waveIn.Dispose();
                GC.Collect();
                waveIn = new WaveIn();
                waveIn.WaveFormat = new WaveFormat();
                waveFileWriter = new WaveFileWriter("sound" + tempWav + ".wav", waveIn.WaveFormat);
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += WaveIn_RecordingStopped;
                waveIn.StartRecording();

            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                if (waveIn != null)
                {
                    if (waveFileWriter.Length + e.BytesRecorded < 500000)
                    {
                        waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                        waveFileWriter.Flush();
                    }
                    else
                    {
                        waveIn.DataAvailable -= WaveIn_DataAvailable;
                        waveFileWriter.Close();
                        waveFileWriter.Dispose();
                        waveIn.StopRecording();
                        GC.Collect();
                    }
                }
            }
            catch(Exception ex)
            {
                textBox_1.Text += ex;
            }
            //bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void Button_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (record)
            {
                record = false;
                if (waveIn != null)
                    waveIn.StopRecording();
            }
        }
    }
}
