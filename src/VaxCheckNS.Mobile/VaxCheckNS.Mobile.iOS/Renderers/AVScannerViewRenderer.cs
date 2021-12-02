using System;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using System.Reflection;
using Foundation;
using UIKit;
using VaxCheckNS.Mobile.iOS.Renderers;

[assembly: ExportRenderer(typeof(ZXingScannerView), typeof(AVScannerViewRenderer))]
namespace VaxCheckNS.Mobile.iOS.Renderers
{
	[Preserve(AllMembers = true)]
	public class AVScannerViewRenderer : ViewRenderer<ZXingScannerView, ZXing.Mobile.AVCaptureScannerView>
	{
		// No-op to be called from app to prevent linker from stripping this out    
		public static void Init()
		{
			var _ = DateTime.Now;
		}

		protected ZXingScannerView formsView;
		protected ZXing.Mobile.AVCaptureScannerView avView;

		protected override async void OnElementChanged(ElementChangedEventArgs<ZXingScannerView> e)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			formsView = Element;

			if (avView == null)
			{

				var cameraPermission = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.Camera>();
				if (cameraPermission != Xamarin.Essentials.PermissionStatus.Granted)
				{
					Console.WriteLine("Missing Camera Permission");
					return;
				}

				// Process requests for autofocus
				formsView.AutoFocusRequested += (x, y) =>
				{
					if (avView != null)
					{
						if (x < 0 && y < 0)
							avView.AutoFocus();
						else
							avView.AutoFocus(x, y);
					}
				};

				avView = new ZXing.Mobile.AVCaptureScannerView()
				{
					CustomOverlayView = new UIView()
				};
				avView.UseCustomOverlayView = true;
				avView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

				base.SetNativeControl(avView);

				if (formsView.IsScanning)
					avView.StartScanning(formsView.RaiseScanResult, formsView.Options);

				if (!formsView.IsAnalyzing)
					avView.PauseAnalysis();

				if (formsView.IsTorchOn)
					avView.Torch(formsView.IsTorchOn);
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (avView == null)
				return;

			switch (e.PropertyName)
			{
				case nameof(ZXingScannerView.IsTorchOn):
					avView.Torch(formsView.IsTorchOn);
					break;
				case nameof(ZXingScannerView.IsScanning):
					if (formsView.IsScanning)
						avView.StartScanning(formsView.RaiseScanResult, formsView.Options);
					else
						avView.StopScanning();
					break;
				case nameof(ZXingScannerView.IsAnalyzing):
					if (formsView.IsAnalyzing)
						avView.ResumeAnalysis();
					else
						avView.PauseAnalysis();
					break;
			}
		}

		public override void TouchesEnded(NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			avView?.AutoFocus();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			// Find the best guess at current orientation
			var o = UIApplication.SharedApplication.StatusBarOrientation;
			if (ViewController != null)
				o = ViewController.InterfaceOrientation;

			// Tell the native view to rotate
			avView?.DidRotate(o);
		}
	}
}
