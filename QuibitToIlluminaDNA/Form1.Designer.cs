namespace QuibitToIlluminaDNA
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.FileSelectedLabel = new System.Windows.Forms.Label();
            this.ProcessInputButton = new System.Windows.Forms.Button();
            this.dataGridViewOut = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.FinishButton = new System.Windows.Forms.Button();
            this.ExportFilesButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOut)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(62, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Qubit Input";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FileSelectedLabel
            // 
            this.FileSelectedLabel.AutoSize = true;
            this.FileSelectedLabel.Location = new System.Drawing.Point(12, 35);
            this.FileSelectedLabel.Name = "FileSelectedLabel";
            this.FileSelectedLabel.Size = new System.Drawing.Size(85, 13);
            this.FileSelectedLabel.TabIndex = 1;
            this.FileSelectedLabel.Text = "No File Selected";
            // 
            // ProcessInputButton
            // 
            this.ProcessInputButton.Enabled = false;
            this.ProcessInputButton.Location = new System.Drawing.Point(62, 120);
            this.ProcessInputButton.Name = "ProcessInputButton";
            this.ProcessInputButton.Size = new System.Drawing.Size(148, 23);
            this.ProcessInputButton.TabIndex = 2;
            this.ProcessInputButton.Text = "Process Input";
            this.ProcessInputButton.UseVisualStyleBackColor = true;
            this.ProcessInputButton.Click += new System.EventHandler(this.ProcessInputButton_Click);
            // 
            // dataGridViewOut
            // 
            this.dataGridViewOut.AllowUserToAddRows = false;
            this.dataGridViewOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewOut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOut.Location = new System.Drawing.Point(15, 163);
            this.dataGridViewOut.Name = "dataGridViewOut";
            this.dataGridViewOut.Size = new System.Drawing.Size(623, 202);
            this.dataGridViewOut.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(333, 64);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(40, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "10";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(333, 96);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(40, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "0.2";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(249, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "DNA Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 26);
            this.label2.TabIndex = 7;
            this.label2.Text = "Concentration\r\nFinal";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.Location = new System.Drawing.Point(252, 126);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Using 1:10 input";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // FinishButton
            // 
            this.FinishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FinishButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FinishButton.Location = new System.Drawing.Point(462, 370);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(176, 24);
            this.FinishButton.TabIndex = 9;
            this.FinishButton.Text = "Finish and close";
            this.FinishButton.UseVisualStyleBackColor = false;
            this.FinishButton.Click += new System.EventHandler(this.FinishButton_Click);
            // 
            // ExportFilesButton
            // 
            this.ExportFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportFilesButton.Enabled = false;
            this.ExportFilesButton.Location = new System.Drawing.Point(274, 370);
            this.ExportFilesButton.Name = "ExportFilesButton";
            this.ExportFilesButton.Size = new System.Drawing.Size(182, 24);
            this.ExportFilesButton.TabIndex = 10;
            this.ExportFilesButton.Text = "Export to epMotion";
            this.ExportFilesButton.UseVisualStyleBackColor = true;
            this.ExportFilesButton.Click += new System.EventHandler(this.ExportFilesButton_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox2.Location = new System.Drawing.Point(361, 126);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(94, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "Use Low DNA";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox3.Location = new System.Drawing.Point(470, 126);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(96, 17);
            this.checkBox3.TabIndex = 12;
            this.checkBox3.Text = "Use High DNA";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(470, 103);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox4.Size = new System.Drawing.Size(84, 17);
            this.checkBox4.TabIndex = 13;
            this.checkBox4.Text = "12x8 Format";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 396);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.ExportFilesButton);
            this.Controls.Add(this.FinishButton);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridViewOut);
            this.Controls.Add(this.ProcessInputButton);
            this.Controls.Add(this.FileSelectedLabel);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Qubit To Illumina Input";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label FileSelectedLabel;
        private System.Windows.Forms.Button ProcessInputButton;
        private System.Windows.Forms.DataGridView dataGridViewOut;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button FinishButton;
        private System.Windows.Forms.Button ExportFilesButton;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
    }
}

