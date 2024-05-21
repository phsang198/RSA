using System; 
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO.Compression;
using System.Xml.Serialization;

namespace RoBot
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point startPoint = new Point(0, 0);
        public RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

        public string path = "";
        public string inputText = "";
        public string encryptedText = "";
        public string decryptedText = "";
        public string extension = "";
        public string encryptedFilePath = "encrypted";
        public string decryptedFilePath = "decrypted";
        public RSAParameters publicKey ;
        public RSAParameters privateKey ;
        public Form1()
        {
            InitializeComponent();

            //Bitmap bitmap0 = new Bitmap(pictureBox.Image);
            //var bt = MakeTransparent(bitmap0, Color.White, 25);
            //// var bt = MakeTransparent(bitmap0, Color.White, 5);
            //pictureBox.Image = bt;

            //Bitmap bitmap1 = new Bitmap(pictureBox1.Image);
            //var bt1 = MakeTransparent(bitmap1, Color.White, 25);
            //// var bt = MakeTransparent(bitmap0, Color.White, 5);
            //pictureBox1.Image = bt1;

            //this.BackColor = Color.White;
            //this.TransparencyKey = Color.White;
        }

        private void Transparent_Form_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Green;
            this.TransparencyKey = Color.Green;
        }

        private void Transparent_images_Click(object sender, EventArgs e)
        {

        }

        private Bitmap MakeTransparent(Bitmap bitmap, Color color, int tolerance)
        {
            Bitmap transparentImage = new Bitmap(bitmap);

            for (int i = transparentImage.Size.Width - 1; i >= 0; i--)
            {
                for (int j = transparentImage.Size.Height - 1; j >= 0; j--)
                {
                    var currentColor = transparentImage.GetPixel(i, j);
                    if (Math.Abs(color.R - currentColor.R) < tolerance &&
                      Math.Abs(color.G - currentColor.G) < tolerance &&
                      Math.Abs(color.B - currentColor.B) < tolerance)
                        transparentImage.SetPixel(i, j, color);
                }
            }

            transparentImage.MakeTransparent(color);

            return transparentImage;
        }


        private void mix(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point p = PointToScreen(e.Location);
                this.Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void picturaBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;

        }

        private void dauX_click(object sender, MouseEventArgs e)
        {
            this.Close();

        }

        private void Nhap_Enter(object sender, EventArgs e)
        {

        }

        private void check_enter(object sender, KeyEventArgs e)
        {
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Word files (*.docx)|*.docx|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Hiển thị hộp thoại Open File và lấy kết quả trả về
            openFileDialog.ShowDialog();

            //if (result == true)
            {
                show.Clear();
                textBox1.Clear();
                textBox4.Clear();
                // Lấy đường dẫn đến file được chọn
                path = openFileDialog.FileName;
                string directory = Path.GetDirectoryName(path);
                extension = Path.GetExtension(path);
                encryptedFilePath = directory +"\\"+ encryptedFilePath + extension; 
                decryptedFilePath = directory +"\\"+ decryptedFilePath + extension; 
                textBox1.AppendText(path);
                // Mở file bằng cách sử dụng StreamReader
                //StreamReader reader = new StreamReader(filePath);
                // ...
               

            }

        }

        public string getPublicKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, publicKey);
            return sw.ToString(); 
        }
        public string getPrivateKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, privateKey);
            return sw.ToString();
        }
        private void GenKey(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox3.Clear(); 
            {
                rsa = new RSACryptoServiceProvider(2048);
                // Lấy khóa công khai và khóa bí mật
                publicKey = rsa.ExportParameters(false);
                privateKey = rsa.ExportParameters(true);

                
                textBox3.AppendText(getPublicKey()); 
                textBox2.AppendText(getPrivateKey());

                show.Clear();
            }
        }
        public string Encrypt(string text )
        {
            rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(publicKey);
            var data = UTF8Encoding.Unicode.GetBytes(text);
            var cypher = rsa.Encrypt(data, false);
            return Convert.ToBase64String(cypher); 
        }
        public string Decrypt(string text)
        {
            var databytes = Convert.FromBase64String(text);
            rsa.ImportParameters(privateKey);
            var res = rsa.Decrypt(databytes, false);
            return UTF8Encoding.Unicode.GetString(res);
        }
        private void Encode(object sender, EventArgs e)
        {

            show.Clear();
            textBox4.Clear();
            string inputText = textBox1.Text;
            if (inputText == "")
            {
                show.AppendText("Vui lòng mở file hoặc nhập text");
                return;
            }
            if (inputText != path)
            {
                show.AppendText(Encrypt(inputText));
                return;
            }

            if (rsa != null)
            {
                byte[] data = File.ReadAllBytes(path);

                if (data == null)
                {
                    show.AppendText("File rỗng");
                }

                if (extension == ".txt")
                {
                    // Mã hóa file bằng khóa công khai
                    byte[] encryptedData = rsa.Encrypt(data, false);

                    // Ghi file kết quả ra đĩa
                    File.WriteAllBytes(encryptedFilePath, encryptedData);
                }
                else
                {
                    {
                        // Open the output file
                        using (var output = File.Create(encryptedFilePath))
                        {
                            //Process the file byte by byte
                            foreach (byte b in data)
                            {
                                //Encrypt the byte
                                byte[] encrypted = rsa.Encrypt(new[] { b }, false);

                                //Write the encrypted byte to the output file
                                output.Write(encrypted, 0, encrypted.Length);
                            }
                        }
                    }
                    //string text = UTF8Encoding.Unicode.GetString(data);

                    //string tmp = Encrypt(UTF8Encoding.Unicode.GetString(data));
                    //byte[] encryptedData = rsa.Encrypt(data, false);
                    //byte[] res = Encoding.Unicode.GetBytes(Encrypt(UTF8Encoding.Unicode.GetString(data)));
                    //File.WriteAllBytes(encryptedFilePath, res);
                }
                show.AppendText("Đã mã hóa thành công , mở file để xem");
            }
            else
            {
                show.AppendText("Vui lòng gen key");
            }
        }

        private void Decode(object sender, EventArgs e)
        {
            textBox4.Clear();
            string encryptedText = show.Text;
            if (encryptedText == "")
            {
                show.AppendText("Vui lòng mở file hoặc nhập text");
                return; 
            }
            if (encryptedText != path && encryptedText != "Đã mã hóa thành công , mở file để xem")
            {
                textBox4.AppendText(Decrypt(encryptedText));
                return;
            }
            if (rsa != null)
            {
                byte[] encryptedData = File.ReadAllBytes(encryptedFilePath);

                if (encryptedData == null)
                {
                    show.AppendText("File rỗng");

                }
                if (extension == ".txt")
                {
                    // Giải mã file bằng khóa bí mật
                    byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                    //?????????
                    // Ghi file kết quả ra đĩa
                    File.WriteAllBytes(decryptedFilePath, decryptedData);
                }
                else
                {
                    //byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                    //File.WriteAllBytes(decryptedFilePath, decryptedData);

                    using (var output = File.Create(decryptedFilePath))
                    {
                        // Process the file byte by byte
                        int count = -1;
                        byte[] arr = new byte[256]; 
                        foreach (byte b in encryptedData)
                        {
                            count++;
                            arr[count] = b;
                            // Decrypt the byte
                            if ( count == 255 )
                            {
                                byte[] decrypted = rsa.Decrypt(arr, false);

                                // Write the decrypted byte to the output file
                                output.Write(decrypted, 0, decrypted.Length);
                                count = -1; 
                            }
                        }
                    }
                } 
                    
                textBox4.AppendText("Đã giải mã thành công , mở file để xem");

            }
            else
            {
                show.AppendText("Vui lòng gen key");

               // MessageBox.Show("", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void InputTextEnter(object sender, EventArgs e)
        {
            //show.Clear();
            //textBox4.Clear();
            //inputText = textBox1.Text;
            
        }

        private void InputText(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    show.Clear();
            //    textBox4.Clear();
            //    inputText = textBox1.Text;
            //}
            //if (e.KeyCode == Keys.Delete)
            //{
            //    show.Clear();
            //    textBox4.Clear();
            //    textBox1.Clear(); 
            //}
        }

        private void Reset(object sender, EventArgs e)
        {
            show.Clear();
            textBox1.Clear(); 
            textBox2.Clear(); 
            textBox3.Clear(); 
            textBox4.Clear();

            path = "";
            inputText = "";
            encryptedText = "";
            decryptedText = "";
            extension = "";
            encryptedFilePath = "encrypted";
            decryptedFilePath = "decrypted";
    }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}