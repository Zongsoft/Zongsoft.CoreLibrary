using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zongsoft.Options.Configuration.Samples.Views
{
	public partial class GeneralOptionView : UserControl
	{
		#region 成员字段
		private IOption _option;
		#endregion

		#region 构造函数
		public GeneralOptionView(IOption option)
		{
			if(option == null)
				throw new ArgumentNullException("option");

			_option = option;

			//初始化窗体组件
			InitializeComponent();
		}
		#endregion

		#region 视图成员
		public bool IsUsable
		{
			get
			{
				return this.IsHandleCreated;
			}
		}

		public IOption Option
		{
			get
			{
				return _option;
			}
		}
		#endregion

		#region 重写方法
		private BindingSource _bs;

		protected override void OnLoad(EventArgs e)
		{
			if(_option != null && _option.OptionObject != null)
			{
				var optionObject = _option.OptionObject as GeneralOptionElement;

				if(txtDateTime.DataBindings["Text"] == null)
					txtDateTime.DataBindings.Add("Text", optionObject, "DateTimeProperty", true, DataSourceUpdateMode.OnValidation, string.Empty, "F");
				if(txtInt.DataBindings["Text"] == null)
					txtInt.DataBindings.Add("Text", optionObject, "Int32Property", true);
				if(txtText.DataBindings["Text"] == null)
					txtText.DataBindings.Add("Text", optionObject, "TextProperty");

				_bs = new BindingSource(optionObject.Cmdlets.ToArray(), "");
				grdCmdlets.DataSource = _bs;
			}

			base.OnLoad(e);
		}
		#endregion
	}
}
