using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 小工具
{
    public partial class 小工具 : Form

    {
        // 定义默认参数
        private const float A_PARA = 6.112f;
        private const float BH_PARA = 17.62f;
        private const float BL_PARA  = 22.46f;
        private const float LH_PARA = 243.12f;
        private const float LL_PARA  = 272.62f;
        private const float K_PARA   = 0.000662f;

        // 使用 System.Windows.Forms.Label 类型
        private System.Windows.Forms.Label[,] labels;

        public 小工具()
        {
        InitializeComponent();

            // 绑定按钮点击事件
            FormatBtn.Click += FormatBtn_Click;
            OutputCodeCopyBtn.Click += OutputCodeCopyBtn_Click;

        }

        // 计算Pws按钮的点击事件处理方法
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(textBoxT.Text);
                float pws = CalculatePws(t);
                labelResult.Text = $"结果: {pws} HPa";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 计算饱和水蒸气压Pws的方法
        private float CalculatePws(float t)
        {
            float result = (BH_PARA * t) / (LH_PARA + t);
            result = A_PARA * (float)Math.Exp(result);
            return result;
        }

        // 计算饱和水蒸气压的方法
        private float Ew_calc(float t)
        {
            float result = (BH_PARA * t) / (LH_PARA + t);
            result = A_PARA * (float)Math.Exp(result);
            return result;
        }

        // 计算水蒸气压Pv的方法
        private float E_calc(float rh, float t)
        {
            float ew = Ew_calc(t); // 饱和水蒸气压
            float result = (rh / 100) * ew; // 水蒸气压
            return result;
        }
        //绝对湿度
        float Dv_calc(float rh, float t)
        {
            float result = 0;
            result = (float)(216.7 * E_calc(rh, t) / (273.15 + t));
            return result;
        }

        // 混合比
        float R_cale(float rh,float t,float p)
        {
            float result = 0;
            float e = E_calc(rh, t);
            result = (float)(0.622 * e / (p - e) * 1000);
            return result;
        }

        //焓值
        float H_calc(float rh, float t, float p)
        {
            float result = 0;
            float r = R_cale(rh, t, p) / 1000;
            result = (float)(1.005 * t + (2501 + 1.859 * t) * r);
            return result;
        }

        //水蒸气压偏差
        float E_Diff(float t, float tw, float p, float e)
        {
            float result = 0;
            result = Ew_calc(tw) - p * K_PARA * (t - tw);
            result = e - result;
            return result;
        }

        //湿球温度
        float Tw_calc(float rh, float t, float p)
        {
            float result = 0;
            float tw_min = -100;
            float tw_max = 100;
            float tw_middle;
            float e = E_calc(rh, t);

            if (t < 100)
            {
                tw_max = t + 5;
            }
            else
            {
                tw_max = 100;
            }

            tw_middle = (tw_min + tw_max) / 2;

            while (Math.Abs(E_Diff(t, tw_middle, p, e)) > 1e-3)
            {
                if (E_Diff(t, tw_middle, p, e) > 0)
                {
                    tw_min = tw_middle;
                }
                else
                {
                    tw_max = tw_middle;
                }
                tw_middle = (tw_min + tw_max) / 2;
            }
            result = tw_middle;
            return result;
        }

        private void btnCalculate2_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp2InputText.Text);
                float rh = float.Parse(rhInputText.Text);
                float pv = E_calc(rh, t);
                labelResult2.Text = $"结果: {pv} HPa";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCalculate3_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp3InputText.Text);
                float rh = float.Parse(Rh3InputText.Text);
                float dv = Dv_calc(rh, t);
                labelResult3.Text = $"结果: {dv} g/m\u207F";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalculate4_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp4InputText.Text);
                float rh = float.Parse(Rh4InputText.Text);
                float p = float.Parse(P1InputText.Text);
                float R = R_cale(rh, t, p);
                labelResult4.Text = $"结果: {R} g/Kg";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalculate5_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp5InputText.Text);
                float rh = float.Parse(Rh5InputText.Text);
                float p = float.Parse(P2InputText.Text);
                float H = H_calc(rh, t, p);
                labelResult5.Text = $"结果: {H} KJ/Kg";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalculate6_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp6InputText.Text);
                float rh = float.Parse(Rh6InputText.Text);
                float p = float.Parse(P3InputText.Text);
                float Tw = Tw_calc(rh, t, p);
                labelResult6.Text = $"结果: {Tw} ℃";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalculate7_Click(object sender, EventArgs e)
        {
            try
            {
                float t = float.Parse(temp7InputText.Text);
                float rh = float.Parse(RH7InputText.Text);
                float p = float.Parse(P4InputText.Text);
                float tw = Tw_calc(rh, t, p);
                float e1 = E_calc(rh, t);
                float e_diff = E_Diff(t, tw, p,e1);
                labelResult7.Text = $"结果: {e_diff} Pa";
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入无效，请输入有效的温度和湿度值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatBtn_Click(object sender, EventArgs e)
        {
            string inputCode = InputCodeText.Text;
            string name = nameInputText.Text;
            string formattedCode = FormatCode(inputCode,name);
            OutputCodeText.Text = formattedCode;
        }

        private void OutputCodeCopyBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputCodeText.Text))
            {
                Clipboard.SetText(OutputCodeText.Text);
                MessageBox.Show("格式化后的代码已复制到粘贴板！");
            }
            else
            {
                MessageBox.Show("没有要复制的内容！");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void CountBtn_Click(object sender, EventArgs e)
        {
            string input = InputHexText.Text;
            int count = CountHexNumbers(input);
            CountResultLabel.Text = count.ToString();
        }

        private int CountHexNumbers(string input)
        {
            var matches = Regex.Matches(input, @"0x[0-9A-Fa-f]+");
            return matches.Count;
        }

        private void TransferBtn_Click(object sender, EventArgs e)
        {
            string input = InputHexText.Text;
            if (int.TryParse(ColText.Text, out int cols) && cols > 0)
            {
                string result = FormatHexNumbers(input, cols);
                TransferResultText.Text = result;
            }
            else
            {
                MessageBox.Show("请输入有效的列数！");
            }
        }

        private string FormatHexNumbers(string input, int cols)
        {
            var matches = Regex.Matches(input, @"0x[0-9A-Fa-f]+");
            int count = 0;
            string result = string.Empty;

            foreach (Match match in matches)
            {
                if (count > 0 && count % cols == 0)
                {
                    result = result.TrimEnd(',') + Environment.NewLine;
                }
                result += match.Value + ", ";
                count++;
            }

            return result.TrimEnd(',', ' ');
        }

        private void CopyBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TransferResultText.Text))
            {
                Clipboard.SetText(TransferResultText.Text);
                MessageBox.Show("结果已复制到粘贴板！");
            }
            else
            {
                MessageBox.Show("没有要复制的内容！");
            }
        }
        private void CommandButton_Click(object sender, EventArgs e)
        {
            // 获取按钮的文本
            string buttonText = ((Button)sender).Text;

            // 根据按钮文本确定要复制的内容
            string command = GetCommandByButton(buttonText);

            // 复制命令到剪贴板
            Clipboard.SetText(command);

            // 根据按钮文本确定要显示的回复内容
            string response = GetResponseByButton(buttonText);

            // 合并提示内容
            string message = "复制命令成功：" + command + "\n示例回复：" + response;

            // 显示弹窗提醒
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 根据按钮文本获取对应的命令
        private string GetCommandByButton(string buttonText)
        {
            switch (buttonText)
            {
                case "1.关闭自加热：01 10 9e 00 00 01 02 00 00 D9 99":
                    return "01 10 9e 00 00 01 02 00 00 D9 99";

                case "2.调节电阻箱到低电阻值读取（如80Ω）：01 03 9e 02 00 01 0A 22":
                    return "01 03 9e 02 00 01 0A 22";

                case "3.调节电阻箱到低电阻值读取（如160Ω）：01 03 9e 02 00 01 0A 22":
                    return "01 03 9e 02 00 01 0A 22";

                case "4.发送标定值：（电阻值x100并转换为16进制，低位在前高位在后）：01 10 9e 04 00 04 08 FC B8 40 1F 76 BD 80 3E 56 0E":
                    return "01 10 9e 04 00 04 08 FC B8 40 1F 76 BD 80 3E 56 0E";

                case "5.读取标定后电阻值：（浮点数）：01 03 9E 0C 00 02 2B E0":
                    return "01 03 9E 0C 00 02 2B E0";

                case "6.读取标定后温度值：（浮点数）：01 03 9E 10 00 02 EA 26":
                    return "01 03 9E 10 00 02 EA 26";

                case "7.打开自加热：01 10 9e 00 00 01 02 01 00 D8 09":
                    return "01 10 9e 00 00 01 02 01 00 D8 09";

                default:
                    return "无对应命令";
            }
        }

        // 根据按钮文本获取对应的回复内容
        private string GetResponseByButton(string buttonText)
        {
            switch (buttonText)
            {
                case "1.关闭自加热：01 10 9e 00 00 01 02 00 00 D9 99":
                    return "回复：01 10 9E 00 00 01 2E 21";

                case "2.调节电阻箱到低电阻值读取（如80Ω）：01 03 9e 02 00 01 0A 22":
                    return "回复：01 03 02 FC B8 F9 36 （例子，不是真实值）";

                case "3.调节电阻箱到低电阻值读取（如160Ω）：01 03 9e 02 00 01 0A 22":
                    return "回复：01 03 02 76 BD 5E 55 （例子，不是真实值）";

                case "4.发送标定值：（电阻值x100并转换为16进制，低位在前高位在后）：01 10 9e 04 00 04 08 FC B8 40 1F 76 BD 80 3E 56 0E":
                    return "回复：01 10 9E 04 00 04 AF E3";

                case "5.读取标定后电阻值：（浮点数）：01 03 9E 0C 00 02 2B E0":
                    return "回复：01 03 04 90 EE F2 42 72 57";

                case "6.读取标定后温度值：（浮点数）：01 03 9E 10 00 02 EA 26":
                    return "回复：01 03 04 00 00 C8 42 2D C2";

                case "7.打开自加热：01 10 9e 00 00 01 02 01 00 D8 09":
                    return "回复：01 10 9E 00 00 01 2E 21";

                default:
                    return "无对应回复";
            }
        }

        // 格式化代码
        private string FormatCode(string input, string name)
        {
            // 使用正则表达式提取所有的十六进制数
            var hexNumbers = System.Text.RegularExpressions.Regex.Matches(input, @"0x[0-9A-Fa-f]{2}");
            var comment = System.Text.RegularExpressions.Regex.Match(input, @"/\*.*?\*/").Value;

            int hexCount = hexNumbers.Count;
            int numbersPerLine = (hexCount % 24 == 0) ? 24 : (hexCount % 12 == 0) ? 12 : 10;

            var formattedCode = new System.Text.StringBuilder();
            formattedCode.AppendLine($"const unsigned char {name}[{hexCount}] =");
            formattedCode.AppendLine("{");

            // 按行排列，每行最多numbersPerLine个数
            for (int i = 0; i < hexCount; i++)
            {
                if (i > 0 && i % numbersPerLine == 0)
                {
                    formattedCode.AppendLine(",");
                }
                else if (i > 0)
                {
                    formattedCode.Append(", ");
                }

                if (i % numbersPerLine == 0)
                {
                    formattedCode.Append("    ");
                }

                formattedCode.Append(hexNumbers[i]);
            }

            // 添加注释并结束
            if (!string.IsNullOrEmpty(comment))
            {
                formattedCode.Append($" {comment}");
            }

            formattedCode.AppendLine();
            formattedCode.AppendLine("};");

            return formattedCode.ToString();
        }


        private void btnReduce_Click(object sender, EventArgs e)
        {
            // 获取输入的16进制数和字宽
            string hexInput = codetabInput.Text;
            int width = int.Parse(codeWidthInput.Text);

            var hexMatches = System.Text.RegularExpressions.Regex.Matches(hexInput, @"0x[0-9A-Fa-f]{2}");

            // 将匹配到的16进制数连接成一个字符串
            string hexValuesString = string.Join(",", hexMatches.Cast<Match>().Select(match => match.Value));

            // 将16进制数转换为二进制字符串
            string[] hexValues = hexInput.Split(',');
            int totalHexValues = hexValues.Length;
            int height = totalHexValues / width;

            // 清空 resultPanel
            resultPanel.Controls.Clear();
            labels = new System.Windows.Forms.Label[width, height * 8];

            // 设置 resultPanel 的大小
            resultPanel.Width = width * 20; // 每个方块20像素
            resultPanel.Height = height * 8 * 20; // 每个字节8位

            // 生成图形
            for (int col = 0; col < width; col++)
            {
                List<string> binaryStrings = new List<string>();
                for (int row = 0; row < height; row++)
                {
                    int intValue = Convert.ToInt32(hexValues[row * width + col].Trim(), 16);
                    string binaryString = Convert.ToString(intValue, 2).PadLeft(8, '0');
                    binaryStrings.Add(binaryString);
                }

                // 逆序排列二进制字符串，使低位在前
                binaryStrings.Reverse();

                // 将二进制字符串按顺序显示，同时上下翻转
                for (int i = 0; i < binaryStrings.Count; i++)
                {
                    string binaryString = binaryStrings[i];
                    for (int bit = 0; bit < binaryString.Length; bit++)
                    {
                        System.Windows.Forms.Label label = new System.Windows.Forms.Label
                        {
                            Width = 20,
                            Height = 20,
                            Left = col * 20,
                            // 上下翻转
                            Top = resultPanel.Height - ((i * 8 + bit) * 20) - 20,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = binaryString[bit] == '1' ? Color.Black : Color.White
                        };

                        label.MouseClick += Label_MouseClick;

                        resultPanel.Controls.Add(label);
                        labels[col, i * 8 + bit] = label;
                    }
                }
            }
        }



        private void Label_MouseClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = sender as System.Windows.Forms.Label;
            if (e.Button == MouseButtons.Left)
            {
                label.BackColor = Color.Black;
            }
            else if (e.Button == MouseButtons.Right)
            {
                label.BackColor = Color.White;
            }
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            int width = int.Parse(codeWidthInput.Text);
            int height = resultPanel.Height / (20 * 8);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    byte value = 0;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        if (labels[col, row * 8 + bit].BackColor == Color.Black)
                        {
                            value |= (byte)(1 << bit);
                        }
                    }
                    result.AppendFormat("0x{0:X2},", value);
                }
                result.AppendLine();
            }

            // 移除最后一个逗号和换行符
            result.Length -= 2;

            // 复制到剪贴板
            Clipboard.SetText(result.ToString());
            MessageBox.Show("结果已复制到剪贴板！", "复制成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
    