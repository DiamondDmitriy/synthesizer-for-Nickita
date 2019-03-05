using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//добавили
using System.Windows.Forms;
using System.Drawing;



namespace Synthesizer
{
    //сделали группу объектов
    public class oscillator : GroupBox
    {
        public oscillator()
        {
            //в ней создали 5 кнопок
            this.Controls.Add(new Button() {
                Name = "Sine",
                Location = new Point(10, 15),
                Text = "Sine",
                BackColor = Color.Yellow
                

            });

            this.Controls.Add(new Button()
            {
                Name = "Square",
                Location = new Point(65, 15),
                Text = "Square",
                

            });

            this.Controls.Add(new Button()
            {
                Name = "Saw",
                Location = new Point(120, 15),
                Text = "Saw",
                

            });

            this.Controls.Add(new Button()
            {
                Name = "Triangle",
                Location = new Point(10, 50),
                Text = "Triangle",
               

            });

            this.Controls.Add(new Button()
            {
                Name = "Noise",
                Location = new Point(65, 50),
                Text = "Noise",
                

            });
            //тут задаём размер и шрифт (значения можно менять и верху тоже)
            foreach(Control control in this.Controls)
            {
                control.Size = new Size(50, 30);
                control.Font = new Font("Microsoft Sans Serif", 6.75f);
                control.Click += WaveButton_Click; 
            }
            //конец создания кнопок




        }

        public WaveForm WaveForm { get; private set;  }

        private void WaveButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            this.WaveForm = (WaveForm)Enum.Parse(typeof(WaveForm),button.Text);
            //задаём цвет 
            foreach (Button otherButtons in this.Controls.OfType<Button>())
            {
                otherButtons.UseVisualStyleBackColor = true;
            }
            button.BackColor = Color.Yellow;
        }

    }
}
