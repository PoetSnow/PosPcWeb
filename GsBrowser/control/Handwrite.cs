using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ink;

namespace Handwrite
{
    public partial class Handwrite : UserControl
    {
        public Handwrite()
        {
            InitializeComponent();
        }

        private InkCollector ic;
        private string FullCACText;
        private RecognizerContext rct;
        public delegate void SelectTextDelegate(object sender, EventArgs e, String value);
        public delegate void CloseDelegate(object sender, EventArgs e);
        public delegate void DeleteInputDelegate(object sender, EventArgs e);
        
        private void Handwrite_Load(object sender, EventArgs e)
        {
            ic = new InkCollector(PictureboxInk.Handle);

            ic.Enabled = true;
            ink_();

            this.ic.Stroke += new InkCollectorStrokeEventHandler(ic_Stroke);
            this.rct.RecognitionWithAlternates += new RecognizerContextRecognitionWithAlternatesEventHandler(rct_RecognitionWithAlternates);

            rct.Strokes = ic.Ink.Strokes;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rct_RecognitionWithAlternates(object sender, RecognizerContextRecognitionWithAlternatesEventArgs e)
        {
            string ResultString = "";
            RecognitionAlternates alts;
            var abc = e.ToString();
            if (e.RecognitionStatus == RecognitionStatus.NoError)
            {
                alts = e.Result.GetAlternatesFromSelection();

                foreach (RecognitionAlternate alt in alts)
                {
                    ResultString = ResultString + alt.ToString() + " ";
                }
            }
            FullCACText = ResultString.Trim();
            Control.CheckForIllegalCrossThreadCalls = false;

            var controls = from tempcontrol in this.panel1.Controls.OfType<Button>().AsQueryable() orderby tempcontrol.Name select tempcontrol;
            string[] words = ResultString.Split(' ');
            if (controls != null && words != null)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    for (int j = 0; j < controls.Count(); j++)
                    {
                        Control control = controls.ToArray()[j];
                        if (i == j)
                        {
                            control.Text = words[i];
                            break;
                        }
                    }
                }
            }

            returnString(FullCACText);
            Control.CheckForIllegalCrossThreadCalls = true;

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        private void returnString(string str)
        {
            string[] str_temp = str.Split(' ');
            string str_temp1 = "shibie_";
            string str_temp2 = "";
            if (str_temp.Length > 0)
            {
                for (int i = 0; i < str_temp.Length; i++)
                {
                    str_temp2 = str_temp1 + i.ToString();
                    Control[] con_temp = Controls.Find(str_temp2, true);
                    if (con_temp.Length > 0)
                    {
                        ((Button)(con_temp[0])).Text = str_temp[i];
                    }
                }
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ic_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            rct.StopBackgroundRecognition();
            rct.Strokes.Add(e.Stroke);
            //rct.CharacterAutoCompletion = RecognizerCharacterAutoCompletionMode.Full;
            rct.BackgroundRecognizeWithAlternates(0);
        }
        /// <summary>
        ///
        /// </summary>
        private void ink_()
        {
            Recognizers recos = new Recognizers();
            Recognizer chineseReco = recos.GetDefaultRecognizer();

            rct = chineseReco.CreateRecognizerContext();
        }

        /// <summary>
        /// 选择文本事件
        /// </summary>
        public event SelectTextDelegate SelectTextEvent;
        private void selectText_Click(object sender, EventArgs e)
        {
            SelectTextEvent(sender, e, ((Button)sender).Text);
            btnClean_Click(sender, e);
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClean_Click(object sender, EventArgs e)
        {
            if (!ic.CollectingInk)
            {
                Strokes strokesToDelete = ic.Ink.Strokes;
                rct.StopBackgroundRecognition();
                ic.Ink.DeleteStrokes(strokesToDelete);
                rct.Strokes = ic.Ink.Strokes;
                ic.Ink.DeleteStrokes();//清除手写区域笔画;
                PictureboxInk.Refresh();//刷新手写区域
            }
        }
        /// <summary>
        /// 选择颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog1.FullOpen = true;
            colorDialog1.ShowDialog();
            ic.DefaultDrawingAttributes.Color = colorDialog1.Color;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        public event CloseDelegate CloseEvent;
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseEvent(sender, e);
        }

        /// <summary>
        /// 退格
        /// </summary>
        public event DeleteInputDelegate DeleteInput;
        private void button1_Click(object sender, EventArgs e)
        {
            DeleteInput(sender, e);
        }
    }
}
