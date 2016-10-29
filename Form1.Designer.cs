namespace OC
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Command = new System.Windows.Forms.TextBox();
            this.Log = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Command
            // 
            this.Command.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(34)))), ((int)(((byte)(43)))));
            this.Command.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Command.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Command.ForeColor = System.Drawing.SystemColors.Window;
            this.Command.Location = new System.Drawing.Point(13, 12);
            this.Command.Name = "Command";
            this.Command.Size = new System.Drawing.Size(553, 23);
            this.Command.TabIndex = 0;
            this.Command.TextChanged += new System.EventHandler(this.Command_TextChanged);
            this.Command.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Accept_KeyDown);
            // 
            // Log
            // 
            this.Log.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(34)))), ((int)(((byte)(43)))));
            this.Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Log.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Log.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Log.ForeColor = System.Drawing.SystemColors.Window;
            this.Log.Location = new System.Drawing.Point(13, 40);
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.Size = new System.Drawing.Size(553, 373);
            this.Log.TabIndex = 1;
            this.Log.Text = "";
            this.Log.TextChanged += new System.EventHandler(this.Log_TextChanged);
            this.Log.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Record);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 425);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.Command);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Операционная система";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox Command;
        public System.Windows.Forms.RichTextBox Log;

    }
}

