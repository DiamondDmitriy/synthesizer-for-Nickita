using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//подключили 
using System.Media;
using System.IO;

//разрешили нажатие клавиш  keyprew.. true
//cоздали нажатие кейдаун

namespace Synthesizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const int SAMPLE_RATE = 44100;//частота дискритизации 
        private const short BIT_PER_SEMPLE = 16; //16 бит 
        private void Form1_Load(object sender, EventArgs e)
        {

        }
       private float frequency = 0.1f; //частота мелодии как в пианино 


        //метод при нажатии клавиши на клаве есть звук
        private void keySoundBtn(object sender, KeyEventArgs e)
        {
            IEnumerable<oscillator> oscillators = this.Controls.OfType<oscillator>().Where(o => o.On);//создаём колекцию осициляторов
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

             int oscillatorsCount = oscillators.Count();

            switch (e.KeyCode)
            {

                case Keys.Z:
                    frequency = 65.4f; // c2
                    break;
                case Keys.X:
                    frequency = 138.59f; //c3
                    break;
                case Keys.C:
                    frequency = 261.62f; //c4
                    break;
                case Keys.V:
                    frequency = 523.25f; //c5
                    break;
                case Keys.B:
                    frequency = 1046.5f; //c6
                    break;
                case Keys.N:
                    frequency = 2093f; //c7
                    break;
                case Keys.M:
                    frequency = 4186.01f; //c8
                    break;

            }

            foreach (oscillator oscillator in oscillators)
            {
                int samplesPerWaveLenght = (int)(SAMPLE_RATE / frequency);
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLenght);
                short tempSample = 0;
                Random random = new Random();
                //все формулы которые ниже ты найдешь на сайте который я вложил в документ 
                switch (oscillator.WaveForm)
                {
                    case WaveForm.Sine:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)) / oscillatorsCount);
                        }
                        break;

                    case WaveForm.Square:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sign(Math.Sin((Math.PI * 2 * frequency) / SAMPLE_RATE * i))) / oscillatorsCount);
                        }

                        break;

                    case WaveForm.Saw:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            tempSample -= short.MaxValue;
                            for (int j = 0; j < samplesPerWaveLenght && i < SAMPLE_RATE; j++)
                            {
                                tempSample += ampStep;
                                wave[i++] += Convert.ToInt16(tempSample / oscillatorsCount);
                            }
                            i--;
                        }
                        break;

                    case WaveForm.Triangle:
                        tempSample = -short.MaxValue;
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            if (Math.Abs(tempSample + ampStep) > short.MaxValue)
                            {
                                ampStep = (short)-ampStep;
                            }
                            tempSample += ampStep;
                            wave[i] += Convert.ToInt16(tempSample / oscillatorsCount);

                        }
                        break;

                    case WaveForm.Noise:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16(random.Next(-short.MaxValue, short.MaxValue) / oscillatorsCount);
                        }
                        break;



                }

            }

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
            //для работы с wav контейнером
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWritter = new BinaryWriter(memoryStream))
            {
                short blocklign = BIT_PER_SEMPLE / 8;//число каналов
                int subChunckTwoSize = SAMPLE_RATE * blocklign;
                binaryWritter.Write(new[] { 'R', 'I', 'F', 'F' });
                binaryWritter.Write(36 + subChunckTwoSize);

                binaryWritter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                binaryWritter.Write(16);
                binaryWritter.Write((short)1);
                binaryWritter.Write((short)1);
                binaryWritter.Write(SAMPLE_RATE);
                binaryWritter.Write(SAMPLE_RATE * blocklign);
                binaryWritter.Write(blocklign);
                binaryWritter.Write(BIT_PER_SEMPLE);
                binaryWritter.Write(new[] { 'd', 'a', 't', 'a' });
                binaryWritter.Write(subChunckTwoSize);
                binaryWritter.Write(binaryWave);

                memoryStream.Position = 0;
                new SoundPlayer(memoryStream).Play();



            }
        }






        private void keySoundMs(object sender, EventArgs e)
        {
            IEnumerable<oscillator> oscillators = this.Controls.OfType<oscillator>().Where(o => o.On);//создаём колекцию осициляторов
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

            int oscillatorsCount = oscillators.Count();

            foreach (oscillator oscillator in oscillators)
            {
                int samplesPerWaveLenght = (int)(SAMPLE_RATE / frequency);
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLenght);
                short tempSample = 0;
                Random random = new Random();
                //все формулы которые ниже ты найдешь на сайте который я вложил в документ 
                switch (oscillator.WaveForm)
                {
                    case WaveForm.Sine:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)) / oscillatorsCount);
                        }
                        break;

                    case WaveForm.Square:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sign(Math.Sin((Math.PI * 2 * frequency) / SAMPLE_RATE * i))) / oscillatorsCount);
                        }

                        break;

                    case WaveForm.Saw:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            tempSample -= short.MaxValue;
                            for (int j = 0; j < samplesPerWaveLenght && i < SAMPLE_RATE; j++)
                            {
                                tempSample += ampStep;
                                wave[i++] += Convert.ToInt16(tempSample / oscillatorsCount);
                            }
                            i--;
                        }
                        break;

                    case WaveForm.Triangle:
                        tempSample = -short.MaxValue;
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            if (Math.Abs(tempSample + ampStep) > short.MaxValue)
                            {
                                ampStep = (short)-ampStep;
                            }
                            tempSample += ampStep;
                            wave[i] += Convert.ToInt16(tempSample / oscillatorsCount);

                        }
                        break;

                    case WaveForm.Noise:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16(random.Next(-short.MaxValue, short.MaxValue) / oscillatorsCount);
                        }
                        break;



                }

            }

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
            //для работы с wav контейнером
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWritter = new BinaryWriter(memoryStream))
            {
                short blocklign = BIT_PER_SEMPLE / 8;//число каналов
                int subChunckTwoSize = SAMPLE_RATE * blocklign;
                binaryWritter.Write(new[] { 'R', 'I', 'F', 'F' });
                binaryWritter.Write(36 + subChunckTwoSize);

                binaryWritter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                binaryWritter.Write(16);
                binaryWritter.Write((short)1);
                binaryWritter.Write((short)1);
                binaryWritter.Write(SAMPLE_RATE);
                binaryWritter.Write(SAMPLE_RATE * blocklign);
                binaryWritter.Write(blocklign);
                binaryWritter.Write(BIT_PER_SEMPLE);
                binaryWritter.Write(new[] { 'd', 'a', 't', 'a' });
                binaryWritter.Write(subChunckTwoSize);
                binaryWritter.Write(binaryWave);

                memoryStream.Position = 0;
                new SoundPlayer(memoryStream).Play();



            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            keySoundBtn(sender, e);

        }




        private void button1_Click(object sender, EventArgs e)
        {
            frequency = 440.0000f;//A4
            keySoundMs(sender,e);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frequency = 466.1638f;//A#
            keySoundMs(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frequency = 493.8833f;//B4
            keySoundMs(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frequency = 523.2511f;
            keySoundMs(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            frequency = 554.3653f;
            keySoundMs(sender, e);
        }















    }


    public enum WaveForm
    {
        Sine, Square, Saw, Triangle, Noise
    }
}
