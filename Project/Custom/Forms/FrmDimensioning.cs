using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Forms;
using Architexor.Request;
using Control = System.Windows.Forms.Control;
using Architexor.Controllers;

namespace Architexor.Forms
{
	public partial class FrmDimensioning : System.Windows.Forms.Form, IExternal
	{
		//	The dialog owns the handler and the event objects,
		//	but it is not a requirement. This may as well be static properties
		//	of the application
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;

		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		public FrmDimensioning(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			//	A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(Architexor.Custom.Application.thisApp, reqId, uiApp);

			//	External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			WakeUp();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			//	we own both the event and the handler
			//	we should dispose it before we are closed
			m_ExEvent.Dispose();
			m_ExEvent = null;
			m_Handler = null;

			//	do not forget to call the base class
			base.OnFormClosed(e);
		}

		private void EnableCommands(bool status)
		{
			foreach(Control ctrl in Controls)
			{
				ctrl.Enabled = status;
			}
		}

		public void DozeOff()
		{
			EnableCommands(false);
		}

		public ArchitexorRequestId GetRequestId()
		{
			if(m_Handler == null)
			{
				return ArchitexorRequestId.None;
			}
			return m_Handler.RequestId;
		}

		public void IClose()
		{
			Close();
		}

		public bool IIsDisposed()
		{
			return IsDisposed;
		}

		public void IShow()
		{
			Show();
		}

		public bool IVisible()
		{
			return Visible;
		}

		public void MakeRequest(ArchitexorRequestId request)
		{
			LastRequestId = request;

			m_Handler.Request.Make(request);
			m_ExEvent.Raise();
			DozeOff();
		}

		public void WakeUp(bool bFinish = false)
		{
			if(bFinish)
			{
				Close();
				return;
			}
			EnableCommands(true);

			Activate();
		}

		private void btnGetExteriorWalls_Click(object sender, EventArgs e)
		{
			Dimensioning controller = (Dimensioning)m_Handler.Instance;
			List<ViewPlan> views = new List<ViewPlan>();
			List<ViewPlan> allViews = controller.AllViews;
			foreach (string item in lstViews.CheckedItems)
			{
				views.Add(allViews.Find(x => item == x.ViewType.ToString() + " : " + x.Name));
			}
			controller.SetSelectedViews(views);

			MakeRequest(ArchitexorRequestId.DetectAndSelectExteriorWalls);
		}

		private void btnGroupExteriorWalls_Click(object sender, EventArgs e)
		{
		}

		private void btnSelectGroup_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectGroup);
		}

		private void btnGenerateDimensions_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.GenerateDimension);
		}

		private void FrmDimensioning_Load(object sender, EventArgs e)
		{
			Dimensioning controller = (Dimensioning)m_Handler.Instance;
			controller.Initialize();

			ElementId curViewId = null;
			if(controller.GetDocument().ActiveView != null)
			{
				curViewId = controller.GetDocument().ActiveView.Id;
			}
			foreach (ViewPlan view in controller.AllViews) {
				if(view.Id == curViewId)
					lstViews.Items.Add(view.ViewType.ToString() + " : " + view.Name, true);
				else
					lstViews.Items.Add(view.ViewType.ToString() + " : " + view.Name);
			}
		}
	}
}
