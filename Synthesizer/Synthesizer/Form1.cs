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
                    frequency = 220.0000f; 
                    break;
                case Keys.X:
                    frequency = 246.9417f; 
                    break;
                case Keys.S:
                    frequency = 233.0819f; 
                    break;

                case Keys.C:
                    frequency = 261.6256f;
                    break;
                case Keys.F:
                    frequency = 277.1826f; 
                    break;
                case Keys.V:
                    frequency = 293.6648f;
                    break;

                case Keys.G:
                    frequency = 311.1270f; 
                    break;
                case Keys.B:
                    frequency = 329.6276f; 
                    break;
                case Keys.N:
                    frequency = 349.2282f; 
                    break;

                case Keys.J:
                    frequency = 369.9944f; 
                    break;
                case Keys.M:
                    frequency = 391.9954f; 
                    break;
                case Keys.K:
                    frequency = 415.3047f;
                    break;
                case Keys.Oemcomma:
                    frequency = 440.0000f; 
                    break;


                case Keys.L:
                    frequency = 466.1638f; 
                    break;
                case Keys.OemPeriod:
                    frequency = 493.8833f; 
                    break;
                case Keys.OemQuestion:
                    frequency = 523.2511f; 
                    break;

                case Keys.D1:
                    frequency = 554.3653f; 
                    break;
                case Keys.Q:
                    frequency = 587.3295f; 
                    break;
                case Keys.D2:
                    frequency = 622.2540f; 
                    break;
                case Keys.W:
                    frequency = 659.2551f; 
                    break;
                case Keys.E:
                    frequency = 698.4565f; 
                    break;
                case Keys.D3:
                    frequency = 739.9888f; 
                    break;
                case Keys.R:
                    frequency = 783.9909f; 
                    break;

                case Keys.D4:
                    frequency = 830.6094f; 
                    break;
                case Keys.T:
                    frequency = 880.0000f; 
                    break;
                case Keys.D5:
                    frequency = 932.3275f; 
                    break;
                case Keys.Y:
                    frequency = 987.7666f; 
                    break;
                case Keys.U:
                    frequency = 1046.502f;
                    break;
                case Keys.D7:
                    frequency = 1108.731f; 
                    break;
                case Keys.I:
                    frequency = 1174.659f; 
                    break;

                case Keys.D9:
                    frequency = 1244.508f; 
                    break;
                case Keys.O:
                    frequency = 1318.510f; 
                    break;
                case Keys.P:
                    frequency = 1396.913f; 
                    break;
                case Keys.OemMinus:
                    frequency = 1479.978f; 
                    break;
                case Keys.OemOpenBrackets:
                    frequency = 1567.982f; 
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
            frequency = 220.0000f;//A2
            keySoundMs(sender,e);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            frequency = 233.0819f;//A#2
            keySoundMs(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frequency = 246.9417f;//B2
            keySoundMs(sender, e);
        }




        private void button3_Click(object sender, EventArgs e)
        {
            frequency = 261.6256f; //c3
            keySoundMs(sender, e);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            frequency = 277.1826f;//C#3
            keySoundMs(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frequency = 293.6648f;//D3
            keySoundMs(sender, e);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            frequency = 311.1270f;//D#3
            keySoundMs(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frequency = 329.6276f;//E3
            keySoundMs(sender, e);
        }




        private void button6_Click(object sender, EventArgs e)
        {
            frequency = 349.2282f;
            keySoundMs(sender, e);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            frequency = 369.9944f;
            keySoundMs(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frequency = 391.9954f;
            keySoundMs(sender, e);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            frequency = 415.3047f;
            keySoundMs(sender, e);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frequency = 440.0000f;
            keySoundMs(sender, e);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            frequency = 466.1638f;
            keySoundMs(sender, e);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            frequency = 493.8833f;
            keySoundMs(sender, e);
        }





        private void button10_Click(object sender, EventArgs e)
        {
            frequency = 523.2511f;
            keySoundMs(sender, e);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            frequency = 554.3653f;
            keySoundMs(sender, e);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frequency = 587.3295f;
            keySoundMs(sender, e);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            frequency = 622.2540f;
            keySoundMs(sender, e);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frequency = 659.2551f;
            keySoundMs(sender, e);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            frequency = 698.4565f;
            keySoundMs(sender, e);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            frequency = 739.9888f;
            keySoundMs(sender, e);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            frequency = 783.9909f;
            keySoundMs(sender, e);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            frequency = 830.6094f;
            keySoundMs(sender, e);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            frequency = 880.0000f;
            keySoundMs(sender, e);

        }

        private void button30_Click(object sender, EventArgs e)
        {
            frequency = 932.3275f;
            keySoundMs(sender, e);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            frequency = 987.7666f;
            keySoundMs(sender, e);
        }






        private void button17_Click(object sender, EventArgs e)
        {
            frequency = 1046.502f;
            keySoundMs(sender, e);

        }

        private void button39_Click(object sender, EventArgs e)
        {
            frequency = 1108.731f;
            keySoundMs(sender, e);

        }

        private void button18_Click(object sender, EventArgs e)
        {
            frequency = 1174.659f;
            keySoundMs(sender, e);

        }

        private void button38_Click(object sender, EventArgs e)
        {
            frequency = 1244.508f;
            keySoundMs(sender, e);

        }

        private void button19_Click(object sender, EventArgs e)
        {
            frequency = 1318.510f;
            keySoundMs(sender, e);

        }

        private void button20_Click(object sender, EventArgs e)
        {
            frequency = 1396.913f;
            keySoundMs(sender, e);

        }

        private void button37_Click(object sender, EventArgs e)
        {
            frequency = 1479.978f;
            keySoundMs(sender, e);

        }

        private void button21_Click(object sender, EventArgs e)
        {
            frequency = 1567.982f;
            keySoundMs(sender, e);

        }

        private void button36_Click(object sender, EventArgs e)
        {
            frequency = 1661.219f;
            keySoundMs(sender, e);

        }

        private void button22_Click(object sender, EventArgs e)
        {
            frequency = 1760.000f;
            keySoundMs(sender, e);

        }

        private void button35_Click(object sender, EventArgs e)
        {
            frequency = 1864.655f;
            keySoundMs(sender, e);

        }

        private void button23_Click(object sender, EventArgs e)
        {
            frequency = 1975.533f;
            keySoundMs(sender, e);

        }

        private void button40_Click(object sender, EventArgs e)
        {
            frequency = 2093.005f;
            keySoundMs(sender, e);

        }
    }


    public enum WaveForm
    {
        Sine, Square, Saw, Triangle, Noise
    }
}
