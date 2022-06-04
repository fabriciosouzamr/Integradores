namespace Teste
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      this.button1 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.textBoxPara = new System.Windows.Forms.TextBox();
      this.textBoxCC = new System.Windows.Forms.TextBox();
      this.textBoxAssunto = new System.Windows.Forms.TextBox();
      this.richTextBoxCorpo = new System.Windows.Forms.RichTextBox();
      this.button4 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(163, 86);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(178, 37);
      this.button1.TabIndex = 0;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(12, 153);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(196, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.Text = "dd/MM/yyyy HH:mm:ss";
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(12, 182);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(196, 20);
      this.textBox2.TabIndex = 2;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(439, 170);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 3;
      this.button2.Text = "button2";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(466, 84);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(75, 23);
      this.button3.TabIndex = 4;
      this.button3.Text = "button3";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // textBoxPara
      // 
      this.textBoxPara.Location = new System.Drawing.Point(306, 218);
      this.textBoxPara.Name = "textBoxPara";
      this.textBoxPara.Size = new System.Drawing.Size(384, 20);
      this.textBoxPara.TabIndex = 5;
      this.textBoxPara.Text = "fabriciosouzamr@yahoo.com.br";
      // 
      // textBoxCC
      // 
      this.textBoxCC.Location = new System.Drawing.Point(306, 244);
      this.textBoxCC.Name = "textBoxCC";
      this.textBoxCC.Size = new System.Drawing.Size(384, 20);
      this.textBoxCC.TabIndex = 6;
      // 
      // textBoxAssunto
      // 
      this.textBoxAssunto.Location = new System.Drawing.Point(306, 270);
      this.textBoxAssunto.Name = "textBoxAssunto";
      this.textBoxAssunto.Size = new System.Drawing.Size(384, 20);
      this.textBoxAssunto.TabIndex = 7;
      this.textBoxAssunto.Text = "Teste de envio de e-mail";
      // 
      // richTextBoxCorpo
      // 
      this.richTextBoxCorpo.Location = new System.Drawing.Point(306, 300);
      this.richTextBoxCorpo.Name = "richTextBoxCorpo";
      this.richTextBoxCorpo.Size = new System.Drawing.Size(384, 128);
      this.richTextBoxCorpo.TabIndex = 8;
      this.richTextBoxCorpo.Text = "Teste de envio de e-mail";
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(637, 82);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(75, 23);
      this.button4.TabIndex = 9;
      this.button4.Text = "button4";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(644, 119);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 10;
      this.label1.Text = "label1";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(646, 152);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 11;
      this.label2.Text = "label2";
      // 
      // timer1
      // 
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.richTextBoxCorpo);
      this.Controls.Add(this.textBoxAssunto);
      this.Controls.Add(this.textBoxCC);
      this.Controls.Add(this.textBoxPara);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.textBox2);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.button1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxPara;
        private System.Windows.Forms.TextBox textBoxCC;
        private System.Windows.Forms.TextBox textBoxAssunto;
        private System.Windows.Forms.RichTextBox richTextBoxCorpo;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1;
    }
}

