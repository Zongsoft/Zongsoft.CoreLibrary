namespace Zongsoft.Options.Configuration.Samples.Views
{
	partial class GeneralOptionView
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.grdCmdlets = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.txtDateTime = new System.Windows.Forms.TextBox();
			this.lblDateTime = new System.Windows.Forms.Label();
			this.txtInt = new System.Windows.Forms.TextBox();
			this.lblInt = new System.Windows.Forms.Label();
			this.txtText = new System.Windows.Forms.TextBox();
			this.lblText = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdCmdlets)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(8, 240);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(320, 192);
			this.tabControl1.TabIndex = 13;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.grdCmdlets);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(312, 166);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// grdCmdlets
			// 
			this.grdCmdlets.AllowUserToOrderColumns = true;
			this.grdCmdlets.AllowUserToResizeRows = false;
			this.grdCmdlets.BackgroundColor = System.Drawing.Color.White;
			this.grdCmdlets.ColumnHeadersHeight = 24;
			this.grdCmdlets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.grdCmdlets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
			this.grdCmdlets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grdCmdlets.Location = new System.Drawing.Point(3, 3);
			this.grdCmdlets.Name = "grdCmdlets";
			this.grdCmdlets.RowHeadersVisible = false;
			this.grdCmdlets.RowTemplate.Height = 23;
			this.grdCmdlets.Size = new System.Drawing.Size(306, 160);
			this.grdCmdlets.TabIndex = 0;
			// 
			// Column1
			// 
			this.Column1.DataPropertyName = "Name";
			this.Column1.HeaderText = "Name";
			this.Column1.Name = "Column1";
			// 
			// Column2
			// 
			this.Column2.DataPropertyName = "CommandType";
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.Column2.DefaultCellStyle = dataGridViewCellStyle1;
			this.Column2.HeaderText = "CommandType";
			this.Column2.Name = "Column2";
			// 
			// Column3
			// 
			this.Column3.DataPropertyName = "Text";
			this.Column3.HeaderText = "Text";
			this.Column3.Name = "Column3";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(312, 166);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// txtDateTime
			// 
			this.txtDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDateTime.Location = new System.Drawing.Point(8, 88);
			this.txtDateTime.Name = "txtDateTime";
			this.txtDateTime.Size = new System.Drawing.Size(319, 21);
			this.txtDateTime.TabIndex = 12;
			// 
			// lblDateTime
			// 
			this.lblDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDateTime.AutoEllipsis = true;
			this.lblDateTime.Location = new System.Drawing.Point(8, 64);
			this.lblDateTime.Name = "lblDateTime";
			this.lblDateTime.Size = new System.Drawing.Size(319, 24);
			this.lblDateTime.TabIndex = 11;
			this.lblDateTime.Text = "日期时间属性";
			this.lblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtInt
			// 
			this.txtInt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtInt.Location = new System.Drawing.Point(8, 32);
			this.txtInt.Name = "txtInt";
			this.txtInt.Size = new System.Drawing.Size(319, 21);
			this.txtInt.TabIndex = 10;
			// 
			// lblInt
			// 
			this.lblInt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInt.AutoEllipsis = true;
			this.lblInt.Location = new System.Drawing.Point(8, 8);
			this.lblInt.Name = "lblInt";
			this.lblInt.Size = new System.Drawing.Size(319, 24);
			this.lblInt.TabIndex = 9;
			this.lblInt.Text = "整数属性";
			this.lblInt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtText
			// 
			this.txtText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtText.Location = new System.Drawing.Point(8, 152);
			this.txtText.Multiline = true;
			this.txtText.Name = "txtText";
			this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtText.Size = new System.Drawing.Size(319, 80);
			this.txtText.TabIndex = 8;
			// 
			// lblText
			// 
			this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblText.AutoEllipsis = true;
			this.lblText.Location = new System.Drawing.Point(8, 128);
			this.lblText.Name = "lblText";
			this.lblText.Size = new System.Drawing.Size(319, 24);
			this.lblText.TabIndex = 7;
			this.lblText.Text = "文本属性";
			this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.txtDateTime);
			this.Controls.Add(this.lblDateTime);
			this.Controls.Add(this.txtInt);
			this.Controls.Add(this.lblInt);
			this.Controls.Add(this.txtText);
			this.Controls.Add(this.lblText);
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(337, 442);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdCmdlets)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.DataGridView grdCmdlets;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox txtDateTime;
		private System.Windows.Forms.Label lblDateTime;
		private System.Windows.Forms.TextBox txtInt;
		private System.Windows.Forms.Label lblInt;
		private System.Windows.Forms.TextBox txtText;
		private System.Windows.Forms.Label lblText;
	}
}
